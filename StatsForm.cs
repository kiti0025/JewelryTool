using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;
using Newtonsoft.Json;

namespace JewelryTool
{
    public partial class StatsForm : Form
    {
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string ProductsFile = Path.Combine(DataFolder, "products.json");
        private static readonly string OrdersFile = Path.Combine(DataFolder, "orders.json");

        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();

        public StatsForm()
        {
            InitializeComponent();
        }

        private void StatsForm_Load(object sender, EventArgs e)
        {
            LoadAllData();
            CalculateAllStats();
        }

        #region 数据加载
        private void LoadAllData()
        {
            if (File.Exists(ProductsFile))
            {
                string json = File.ReadAllText(ProductsFile);
                products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }

            if (File.Exists(OrdersFile))
            {
                string json = File.ReadAllText(OrdersFile);
                orders = JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
            }
        }
        #endregion

        #region 统计计算
        private void CalculateAllStats()
        {
            CalculateProductStats();
            CalculatePriceStats();
            CalculateDailyStats();
        }

        private void CalculateProductStats()
        {
            var stats = from p in products
                        join item in orders.SelectMany(o => o.Items) on p.Id equals item.ProductId into items
                        from item in items.DefaultIfEmpty()
                        group item by p into g
                        select new
                        {
                            品类 = g.Key.Name,
                            入库重量 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.NetWeight ?? 0),
                            出库重量 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "出库").Sum(x => x?.NetWeight ?? 0),
                            库存重量 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.NetWeight ?? 0)
                                        - g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "出库").Sum(x => x?.NetWeight ?? 0),
                            入库总金额 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.TotalPrice ?? 0),
                            出库总金额 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "出库").Sum(x => x?.TotalPrice ?? 0),
                            总库存每克均价 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.StatisticTotalPrice ?? 0)
                                        / (g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.NetWeight ?? 0) + 0.0001m)
                        };

            dgvProductStats.DataSource = stats.ToList();

            foreach (DataGridViewColumn col in dgvProductStats.Columns)
            {
                if (col.Name.Contains("重量") || col.Name.Contains("金额") || col.Name.Contains("均价"))
                {
                    col.DefaultCellStyle.Format = "F4";
                }
            }
        }

        private void CalculatePriceStats()
        {
            var tiers = new List<dynamic>();
            for (int i = 0; i < 50; i++)
            {
                tiers.Add(new { Min = i * 100m, Max = (i + 1) * 100m, Name = $"{i * 100}-{(i + 1) * 100}克" });
            }
            tiers.Add(new { Min = 5000m, Max = decimal.MaxValue, Name = "5000克以上" });

            var stats = from tier in tiers
                        let inItems = orders.Where(o => o.OrderType == "入库")
                   .SelectMany(o => o.Items)
                   .Where(x => x.ProductName == "黄金")  // 只统计黄金
                        let tierItems = inItems.Where(x => x.NetWeight >= tier.Min && x.NetWeight < tier.Max)
                        select new
                        {
                            重量阶梯 = tier.Name,
                            入库总重量 = tierItems.Sum(x => x.NetWeight),
                            入库总金额 = tierItems.Sum(x => x.StatisticTotalPrice),
                            每百克均价 = tierItems.Sum(x => x.StatisticTotalPrice) / (tierItems.Sum(x => x.NetWeight) + 0.0001m) * 100
                        };

            dgvPriceStats.DataSource = stats.ToList();
            dgvPriceStats.MultiSelect = true;
            dgvPriceStats.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (DataGridViewColumn col in dgvPriceStats.Columns)
            {
                if (col.Name != "重量阶梯")
                {
                    col.DefaultCellStyle.Format = "F4";
                }
            }
        }

        private void CalculateDailyStats()
        {
            var stats = from o in orders
                        group o by o.OrderDate.Date into g
                        orderby g.Key descending
                        let dayInItems = g.Where(x => x.OrderType == "入库").SelectMany(x => x.Items)
                        let dayOutItems = g.Where(x => x.OrderType == "出库").SelectMany(x => x.Items)
                        select new
                        {
                            日期 = g.Key.ToString("yyyy-MM-dd"),
                            入库总重量 = dayInItems.Sum(x => x.NetWeight),
                            入库总金额 = g.Where(x => x.OrderType == "入库").Sum(x => x.GrandTotal),
                            当日入库每克均价 = dayInItems.Sum(x => x.TotalPrice) / (dayInItems.Sum(x => x.NetWeight) + 0.0001m),
                            出库总重量 = dayOutItems.Sum(x => x.NetWeight),
                            出库总金额 = g.Where(x => x.OrderType == "出库").Sum(x => x.GrandTotal),
                            当日出库每克均价 = dayOutItems.Sum(x => x.TotalPrice) / (dayOutItems.Sum(x => x.NetWeight) + 0.0001m),
                            当日净库存变化 = dayInItems.Sum(x => x.NetWeight) - dayOutItems.Sum(x => x.NetWeight)
                        };

            dgvDailyStats.DataSource = stats.ToList();

            foreach (DataGridViewColumn col in dgvDailyStats.Columns)
            {
                if (col.Name != "日期")
                {
                    col.DefaultCellStyle.Format = "F4";
                }
            }
        }
        #endregion

        #region 按钮事件
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllData();
            CalculateAllStats();
            MessageBox.Show("数据刷新完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportStats_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel文件|*.xlsx";
                sfd.FileName = $"统计报表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var package = new ExcelPackage())
                    {
                        var sheet1 = package.Workbook.Worksheets.Add("总库存均价统计");
                        ExportGridViewToSheet(dgvProductStats, sheet1);

                        var sheet2 = package.Workbook.Worksheets.Add("每百克阶梯均价");
                        ExportGridViewToSheet(dgvPriceStats, sheet2);

                        var sheet3 = package.Workbook.Worksheets.Add("每日出入库统计");
                        ExportGridViewToSheet(dgvDailyStats, sheet3);

                        package.SaveAs(new FileInfo(sfd.FileName));
                    }
                    MessageBox.Show("报表导出成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvPriceStats_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPriceStats.SelectedRows.Count == 0)
            {
                lblSelectedStats.Text = "选择重量阶梯可查看合并均价";
                return;
            }

            decimal totalWeight = 0;
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgvPriceStats.SelectedRows)
            {
                totalWeight += Convert.ToDecimal(row.Cells["入库总重量"].Value ?? 0);
                totalAmount += Convert.ToDecimal(row.Cells["入库总金额"].Value ?? 0);
            }

            decimal avgPer100g = totalAmount / (totalWeight + 0.0001m) * 100;

            lblSelectedStats.Text = $"已选 {dgvPriceStats.SelectedRows.Count} 项 | 总重量：{totalWeight:F2}g | 总金额：{totalAmount:F2}元 | 合并每百克均价：{avgPer100g:F2}元";
        }

        private void ExportGridViewToSheet(DataGridView dgv, ExcelWorksheet sheet)
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = dgv.Columns[i].HeaderText;
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            for (int row = 0; row < dgv.Rows.Count; row++)
            {
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    sheet.Cells[row + 2, col + 1].Value = dgv.Rows[row].Cells[col].Value;
                }
            }

            sheet.Cells.AutoFitColumns();
        }
        #endregion
    }
}