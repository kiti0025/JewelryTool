using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace JewelryTool
{
    public partial class PrintSettingForm : Form
    {
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "printSettings.json");
        private PrintConfig _config;
        public PageSettings ResultPageSettings { get; private set; }

        public PrintSettingForm(PageSettings currentSettings)
        {
            InitializeComponent();
            ResultPageSettings = (PageSettings)currentSettings.Clone();
            LoadConfig();
            BindPaperPresets();
            LoadCurrentSettings();
        }

        #region 配置管理
        private void LoadConfig()
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                _config = JsonConvert.DeserializeObject<PrintConfig>(json);
            }
            else
            {
                _config = new PrintConfig
                {
                    PaperPresets = new List<PaperItem>
                    {
                        new PaperItem{ Name = "二分复写纸", Width=241, Height=140 },
                        new PaperItem{ Name = "A4纸", Width=210, Height=297 }
                    }
                };
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(_configPath, json);
        }

        private void BindPaperPresets()
        {
            cboPaperPresets.DataSource = null;
            cboPaperPresets.DataSource = _config.PaperPresets;
            cboPaperPresets.DisplayMember = "Name";
            cboPaperPresets.ValueMember = "Name";
        }
        #endregion

        #region 加载当前设置
        private void LoadCurrentSettings()
        {
            radPortrait.Checked = !ResultPageSettings.Landscape;
            radLandscape.Checked = ResultPageSettings.Landscape;

            numMarginTop.Value = (decimal)(ResultPageSettings.Margins.Top * 0.254);
            numMarginBottom.Value = (decimal)(ResultPageSettings.Margins.Bottom * 0.254);
            numMarginLeft.Value = (decimal)(ResultPageSettings.Margins.Left * 0.254);
            numMarginRight.Value = (decimal)(ResultPageSettings.Margins.Right * 0.254);
        }
        #endregion

        #region 纸张预设操作
        private void cboPaperPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPaperPresets.SelectedItem is PaperItem paper)
            {
                txtPaperName.Text = paper.Name;
                txtPaperWidth.Text = paper.Width.ToString();
                txtPaperHeight.Text = paper.Height.ToString();
            }
        }

        // 保存预设（自定义名称）
        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPaperName.Text))
            {
                MessageBox.Show("请输入纸张名称！");
                return;
            }
            if (!decimal.TryParse(txtPaperWidth.Text, out decimal w) ||
                !decimal.TryParse(txtPaperHeight.Text, out decimal h) || w <= 0 || h <= 0)
            {
                MessageBox.Show("请输入有效的尺寸！");
                return;
            }

            string name = txtPaperName.Text.Trim();
            if (_config.PaperPresets.Exists(x => x.Name == name))
            {
                MessageBox.Show("该名称已存在！");
                return;
            }

            _config.PaperPresets.Add(new PaperItem { Name = name, Width = w, Height = h });
            SaveConfig();
            BindPaperPresets();
            cboPaperPresets.SelectedItem = _config.PaperPresets[_config.PaperPresets.Count - 1];
            MessageBox.Show("保存成功！");
        }

        // 删除预设
        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            // 修复C#7.3语法：替换is not为!(... is ...)
            if (!(cboPaperPresets.SelectedItem is PaperItem paper))
            {
                MessageBox.Show("请选择要删除的预设！");
                return;
            }

            // 禁止删除默认预设
            if (paper.Name == "二分复写纸" || paper.Name == "A4纸")
            {
                MessageBox.Show("默认预设不允许删除！");
                return;
            }

            if (MessageBox.Show($"确定删除【{paper.Name}】？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _config.PaperPresets.Remove(paper);
                SaveConfig();
                BindPaperPresets();
                txtPaperName.Clear();
                txtPaperWidth.Clear();
                txtPaperHeight.Clear();
                MessageBox.Show("删除成功！");
            }
        }
        #endregion

        #region 确定/取消
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtPaperWidth.Text, out decimal widthMm) ||
                    !decimal.TryParse(txtPaperHeight.Text, out decimal heightMm) || widthMm <= 0 || heightMm <= 0)
                {
                    MessageBox.Show("纸张尺寸输入错误！");
                    return;
                }

                int width = (int)Math.Round(widthMm / 0.254m);
                int height = (int)Math.Round(heightMm / 0.254m);
                PaperSize customPaper = new PaperSize("Custom", width, height);
                customPaper.RawKind = (int)PaperKind.Custom;
                ResultPageSettings.PaperSize = customPaper;

                // 方向逻辑修复：选择横向时Landscape为true
                ResultPageSettings.Landscape = radLandscape.Checked;

                int top = (int)Math.Round(numMarginTop.Value / 0.254m);
                int bottom = (int)Math.Round(numMarginBottom.Value / 0.254m);
                int left = (int)Math.Round(numMarginLeft.Value / 0.254m);
                int right = (int)Math.Round(numMarginRight.Value / 0.254m);
                ResultPageSettings.Margins = new Margins(left, right, top, bottom);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置失败：{ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion

        #region 实体类
        public class PrintConfig
        {
            public List<PaperItem> PaperPresets { get; set; } = new List<PaperItem>();
        }

        public class PaperItem
        {
            public string Name { get; set; }
            public decimal Width { get; set; }
            public decimal Height { get; set; }
        }
        #endregion
    }
}