using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JewelryTool
{
    public partial class MainForm : Form
    {
        // 数据文件路径
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string CustomersFile = Path.Combine(DataFolder, "customers.json");
        private static readonly string ProductsFile = Path.Combine(DataFolder, "products.json");
        private static readonly string OrdersFile = Path.Combine(DataFolder, "orders.json");

        // 全局数据
        private List<Customer> customers = new List<Customer>();
        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();

        // 打印配置
        private PageSettings printPageSettings = new PageSettings();

        // ====================== 新增：时间刷新定时器（仅1行，无修改） ======================
        private Timer timeRefreshTimer;

        public MainForm()
        {
            InitializeComponent();
            InitDataFolder();
            LoadAllData();
            BindControlEvents();
            InitPrintDefaultSetting();
        }

        #region 初始化
        private void InitDataFolder()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
        }

        private void LoadAllData()
        {
            // 加载客户
            if (File.Exists(CustomersFile))
            {
                string json = File.ReadAllText(CustomersFile);
                customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
            }
            else
            {
                customers = new List<Customer>
                {
                    new Customer { Id = 1, Name = "零售客户" },
                    new Customer { Id = 2, Name = "合作门店" }
                };
                SaveCustomersToJson();
            }

            // 加载品类
            if (File.Exists(ProductsFile))
            {
                string json = File.ReadAllText(ProductsFile);
                products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }
            else
            {
                products = new List<Product>
                {
                    new Product { Id = 1, Name = "黄金" },
                    new Product { Id = 2, Name = "18k" },
                    new Product { Id = 3, Name = "22k" },
                    new Product { Id = 4, Name = "pt900" },
                    new Product { Id = 5, Name = "pt950" },
                    new Product { Id = 6, Name = "pt990" },
                    new Product { Id = 7, Name = "pd990" }
                };
                SaveProductsToJson();
            }

            // 加载单据
            if (File.Exists(OrdersFile))
            {
                string json = File.ReadAllText(OrdersFile);
                orders = JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
            }
            else
            {
                orders = new List<Order>();
            }

            // 绑定客户下拉
            cbCustomer.DataSource = null;
            cbCustomer.DataSource = customers;
            cbCustomer.DisplayMember = "Name";
            cbCustomer.ValueMember = "Id";

            // ====================== 修复1：改回正确的ComboBoxStyle.DropDown（原笔误写成Drop） ======================
            cbCustomer.DropDownStyle = ComboBoxStyle.DropDown;
            cbCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;

            // 绑定表格品名下拉
            colProduct.DataSource = null;
            colProduct.DataSource = products;
            colProduct.DisplayMember = "Name";
            colProduct.ValueMember = "Id";

            // 日期格式
            dtpDate.Value = DateTime.Now;
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "yyyy-MM-dd HH:mm";
            // ====================== 新增：启动自动刷新时间（仅1行，无修改） ======================
            StartAutoRefreshTime();

            if (cbType.Items.Count > 0)
                cbType.SelectedIndex = 0;
        }

        private void BindControlEvents()
        {
            btnAddRow.Click += BtnAddRow_Click;
            btnDeleteRow.Click += BtnDeleteRow_Click;
            btnSave.Click += BtnSave_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnPrint.Click += BtnPrint_Click;
            btnPrintSetting.Click += BtnPrintSetting_Click;
            btnStats.Click += BtnStats_Click;
            btnAddCustomer.Click += BtnAddCustomer_Click;
            btnDeleteCustomer.Click += BtnDeleteCustomer_Click;
            btnAddProduct.Click += BtnAddProduct_Click;
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            btnHistory.Click += BtnHistory_Click;

            dgvItems.CellEndEdit += DgvItems_CellEndEdit;
            dgvItems.RowsAdded += (s, e) => RefreshRowNumber();

            cbZoom.SelectedIndexChanged += CbZoom_SelectedIndexChanged;
        }

        private void InitPrintDefaultSetting()
        {
            // 适配二分复写纸 241mm×139.7mm
            int paperWidth = (int)Math.Round(241 / 0.254);
            int paperHeight = (int)Math.Round(139.7 / 0.254);

            foreach (PaperSize paper in printPageSettings.PrinterSettings.PaperSizes)
            {
                if (paper.Width == paperWidth && paper.Height == paperHeight)
                {
                    printPageSettings.PaperSize = paper;
                    break;
                }
            }

            printPageSettings.Margins = new Margins(20, 20, 20, 20);
        }
        #endregion

        // ====================== 新增：每分钟自动刷新时间（独立方法，无修改） ======================
        private void StartAutoRefreshTime()
        {
            timeRefreshTimer = new Timer();
            timeRefreshTimer.Interval = 60000; // 1分钟 = 60000毫秒
            timeRefreshTimer.Tick += (s, e) => { dtpDate.Value = DateTime.Now; };
            timeRefreshTimer.Start();
        }

        #region 数据保存
        private void SaveCustomersToJson()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(customers, Formatting.Indented);
                File.WriteAllText(CustomersFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存客户数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProductsToJson()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(products, Formatting.Indented);
                File.WriteAllText(ProductsFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存品类数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveOrdersToJson()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(orders, Formatting.Indented);
                File.WriteAllText(OrdersFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存单据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RefreshAllData()
        {
            LoadAllData();
        }
        #endregion

        #region 界面操作
        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            string custName = Interaction.InputBox("请输入客户名称", "新增客户", "");
            if (string.IsNullOrWhiteSpace(custName)) return;

            int newId = customers.Count > 0 ? customers.Max(c => c.Id) + 1 : 1;
            customers.Add(new Customer { Id = newId, Name = custName });
            SaveCustomersToJson();

            cbCustomer.DataSource = null;
            cbCustomer.DataSource = customers;
            cbCustomer.DisplayMember = "Name";
            cbCustomer.ValueMember = "Id";

            MessageBox.Show("客户添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDeleteCustomer_Click(object sender, EventArgs e)
        {
            var selectedCust = cbCustomer.SelectedItem as Customer;
            if (selectedCust == null)
            {
                MessageBox.Show("请选择要删除的客户！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedCust.Name == "零售客户" || selectedCust.Name == "合作门店")
            {
                MessageBox.Show("系统默认客户，禁止删除！", "禁止操作", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isUsed = orders.Any(o => o.CustomerId == selectedCust.Id);
            if (isUsed)
            {
                MessageBox.Show("该客户已存在单据，禁止删除！", "数据保护", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"确定删除客户：{selectedCust.Name}？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                customers.Remove(selectedCust);
                SaveCustomersToJson();

                cbCustomer.DataSource = null;
                cbCustomer.DataSource = customers;
                cbCustomer.DisplayMember = "Name";
                cbCustomer.ValueMember = "Id";

                MessageBox.Show("客户删除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            string prodName = Interaction.InputBox("请输入品类名称", "新增品类", "");
            if (string.IsNullOrWhiteSpace(prodName)) return;

            int newId = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            products.Add(new Product { Id = newId, Name = prodName });
            SaveProductsToJson();

            colProduct.DataSource = null;
            colProduct.DataSource = products;
            colProduct.DisplayMember = "Name";
            colProduct.ValueMember = "Id";

            MessageBox.Show("品类添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (colProduct.DataSource == null) return;
            var productList = (List<Product>)colProduct.DataSource;
            if (productList.Count == 0)
            {
                MessageBox.Show("暂无品类可删除！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form selectForm = new Form())
            {
                selectForm.Text = "选择删除品类";
                selectForm.Size = new Size(300, 150);
                ComboBox cbo = new ComboBox() { Dock = DockStyle.Top, DataSource = productList, DisplayMember = "Name", ValueMember = "Id" };
                Button btnOk = new Button() { Text = "确认删除", Dock = DockStyle.Bottom };
                selectForm.Controls.Add(cbo);
                selectForm.Controls.Add(btnOk);
                selectForm.StartPosition = FormStartPosition.CenterParent;

                btnOk.Click += (s, ev) =>
                {
                    var selectedProd = cbo.SelectedItem as Product;
                    if (selectedProd == null) return;

                    var defaultProducts = new[] { "黄金", "18k", "22k", "pt900", "pt950", "pt990", "pd990" };
                    if (defaultProducts.Contains(selectedProd.Name))
                    {
                        MessageBox.Show("系统默认品类，禁止删除！", "禁止操作", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectForm.DialogResult = DialogResult.OK;
                        return;
                    }

                    bool isUsed = orders.SelectMany(o => o.Items).Any(i => i.ProductId == selectedProd.Id);
                    if (isUsed)
                    {
                        MessageBox.Show("该品类已存在单据，禁止删除！", "数据保护", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectForm.DialogResult = DialogResult.OK;
                        return;
                    }

                    if (MessageBox.Show($"确定删除品类：{selectedProd.Name}？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        products.Remove(selectedProd);
                        SaveProductsToJson();

                        colProduct.DataSource = null;
                        colProduct.DataSource = products;
                        colProduct.DisplayMember = "Name";
                        colProduct.ValueMember = "Id";

                        MessageBox.Show("品类删除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    selectForm.DialogResult = DialogResult.OK;
                };

                selectForm.ShowDialog();
            }
        }

        private void BtnAddRow_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvItems.Rows.Add();
            var newRow = dgvItems.Rows[rowIndex];
            newRow.Cells[colPurity.Name].Value = 1m;
            RefreshRowNumber();
        }

        private void BtnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0 || dgvItems.SelectedRows[0].IsNewRow)
            {
                MessageBox.Show("请选择要删除的行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvItems.Rows.RemoveAt(dgvItems.SelectedRows[0].Index);
            RefreshRowNumber();
            UpdateTotalPrice();
        }

        private void RefreshRowNumber()
        {
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                dgvItems.Rows[i].Cells[colIndex.Name].Value = i + 1;
            }
        }

        private void CbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbZoom.SelectedItem == null) return;
            string zoomText = cbZoom.SelectedItem.ToString().Replace("%", "");
            if (decimal.TryParse(zoomText, out decimal zoomRate))
            {
                float scale = (float)(zoomRate / 100m);
                ScaleDataGridView(dgvItems, scale);
            }
        }

        private void ScaleDataGridView(DataGridView dgv, float scale)
        {
            if (dgv == null) return;

            if (!dgv.Tag?.ToString().StartsWith("OriginalFontSize:") ?? true)
            {
                float originalSize = dgv.Font?.Size ?? 9f;
                dgv.Tag = $"OriginalFontSize:{originalSize}";
            }

            string tagStr = dgv.Tag?.ToString() ?? "";
            if (tagStr.StartsWith("OriginalFontSize:"))
            {
                if (float.TryParse(tagStr.Replace("OriginalFontSize:", ""), out float originalSize))
                {
                    float newSize = originalSize * scale;
                    if (newSize >= 6 && newSize <= 36)
                    {
                        dgv.Font = new Font(dgv.Font.FontFamily, newSize, dgv.Font.Style);

                        foreach (DataGridViewColumn column in dgv.Columns)
                        {
                            if (column.HeaderCell.Style.Font != null)
                            {
                                column.HeaderCell.Style.Font = new Font(
                                    column.HeaderCell.Style.Font.FontFamily,
                                    newSize,
                                    column.HeaderCell.Style.Font.Style);
                            }
                        }

                        int newRowHeight = (int)(dgv.RowTemplate.Height * scale);
                        if (newRowHeight >= 20 && newRowHeight <= 80)
                        {
                            dgv.RowTemplate.Height = newRowHeight;
                        }

                        foreach (DataGridViewColumn column in dgv.Columns)
                        {
                            int newWidth = (int)(column.Width * scale);
                            if (newWidth >= 30 && newWidth <= 300)
                            {
                                column.Width = newWidth;
                            }
                        }
                    }
                }
            }
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            using (HistoryForm historyForm = new HistoryForm(this))
            {
                historyForm.ShowDialog();
                LoadAllData();
            }
        }
        #endregion

        #region 自动计算
        private void DgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var currentRow = dgvItems.Rows[e.RowIndex];

            decimal purity = 0, unitPrice = 0;
            if (decimal.TryParse(currentRow.Cells[colPurity.Name].Value?.ToString(), out purity)
                && decimal.TryParse(currentRow.Cells[colUnitPrice.Name].Value?.ToString(), out unitPrice))
            {
                decimal discountPrice = purity * unitPrice;
                currentRow.Cells[colDiscountedPrice.Name].Value = Math.Truncate(discountPrice * 10) / 10;
            }

            decimal netWeight = 0, finalDiscountPrice = 0;
            if (decimal.TryParse(currentRow.Cells[colNetWeight.Name].Value?.ToString(), out netWeight)
                && decimal.TryParse(currentRow.Cells[colDiscountedPrice.Name].Value?.ToString(), out finalDiscountPrice))
            {
                decimal itemTotal = netWeight * finalDiscountPrice;
                currentRow.Cells[colTotalPrice.Name].Value = (int)Math.Truncate(itemTotal);
            }

            UpdateTotalPrice();
        }

        public void UpdateTotalPrice()
        {
            int total = 0;
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (int.TryParse(row.Cells[colTotalPrice.Name].Value?.ToString(), out int itemTotal))
                {
                    total += itemTotal;
                }
            }
            lblTotal.Text = $"单据总价：{total} 元";
        }
        #endregion

        #region 核心功能
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (dgvItems.Rows.Count == 0)
            {
                MessageBox.Show("请添加货品明细后保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedCust = cbCustomer.SelectedItem as Customer;
                if (selectedCust == null)
                {
                    MessageBox.Show("请选择客户", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int grandTotal = 0;
                int.TryParse(lblTotal.Text.Replace("单据总价：", "").Replace("元", "").Trim(), out grandTotal);

                int newOrderId = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
                Order newOrder = new Order
                {
                    Id = newOrderId,
                    CustomerId = selectedCust.Id,
                    CustomerName = selectedCust.Name,
                    OrderDate = dtpDate.Value,
                    OrderType = cbType.SelectedItem?.ToString() ?? "入库",
                    GrandTotal = grandTotal,
                    Remarks = txtRemarks.Text.Trim(),
                    Items = new List<OrderItem>()
                };

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    var product = products.FirstOrDefault(p => p.Id == Convert.ToInt32(row.Cells[colProduct.Name].Value ?? 0));

                    decimal netWeight = Convert.ToDecimal(row.Cells[colNetWeight.Name].Value ?? 0);
                    decimal unitPrice = Convert.ToDecimal(row.Cells[colUnitPrice.Name].Value ?? 0);
                    decimal statTotal = Math.Round(netWeight * unitPrice, 2);

                    newOrder.Items.Add(new OrderItem
                    {
                        Id = newOrder.Items.Count + 1,
                        ProductId = product?.Id ?? 0,
                        ProductName = product?.Name ?? "",
                        GrossWeight = Convert.ToDecimal(row.Cells[colGrossWeight.Name].Value ?? 0),
                        NetWeight = netWeight,
                        Purity = Convert.ToDecimal(row.Cells[colPurity.Name].Value ?? 1),
                        UnitPrice = unitPrice,
                        DiscountedPrice = Convert.ToDecimal(row.Cells[colDiscountedPrice.Name].Value ?? 0),
                        TotalPrice = Convert.ToInt32(row.Cells[colTotalPrice.Name].Value ?? 0),
                        StatisticTotalPrice = statTotal
                    });
                }

                orders.Add(newOrder);
                SaveOrdersToJson();

                MessageBox.Show($"单据保存成功！单据号：{newOrderId}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                dgvItems.Rows.Clear();
                txtRemarks.Clear();
                UpdateTotalPrice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel文件|*.xlsx";
                sfd.FileName = $"瑞华珠宝兑料单_{dtpDate.Value:yyyyMMddHHmmss}.xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var package = new ExcelPackage())
                    {
                        var sheet = package.Workbook.Worksheets.Add("兑料单");

                        sheet.Cells["A1"].Value = "瑞华珠宝兑料单";
                        sheet.Cells["A1:H1"].Merge = true;
                        sheet.Cells["A1"].Style.Font.Size = 16;
                        sheet.Cells["A1"].Style.Font.Bold = true;
                        sheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        sheet.Cells["A3"].Value = "客户：";
                        sheet.Cells["B3"].Value = (cbCustomer.SelectedItem as Customer)?.Name;
                        sheet.Cells["D3"].Value = "日期：";
                        sheet.Cells["E3"].Value = dtpDate.Value.ToString("yyyy-MM-dd HH:mm");
                        sheet.Cells["G3"].Value = "单据类型：";
                        sheet.Cells["H3"].Value = cbType.SelectedItem.ToString();

                        string[] headers = { "序号", "品名", "毛重量", "净重量", "成色", "单价", "成色折价", "单项总价" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            sheet.Cells[5, i + 1].Value = headers[i];
                            sheet.Cells[5, i + 1].Style.Font.Bold = true;
                        }

                        for (int row = 0; row < dgvItems.Rows.Count; row++)
                        {
                            for (int col = 0; col < dgvItems.Columns.Count; col++)
                            {
                                if (col == 1)
                                {
                                    var cellValue = dgvItems.Rows[row].Cells[col].Value;
                                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int productId))
                                    {
                                        var product = products.FirstOrDefault(p => p.Id == productId);
                                        sheet.Cells[row + 6, col + 1].Value = product?.Name ?? "";
                                    }
                                    else
                                    {
                                        sheet.Cells[row + 6, col + 1].Value = "";
                                    }
                                }
                                else
                                {
                                    sheet.Cells[row + 6, col + 1].Value = dgvItems.Rows[row].Cells[col].Value;
                                }
                            }
                        }

                        int totalRow = dgvItems.Rows.Count + 7;
                        sheet.Cells[totalRow, 1].Value = "单据总价：";
                        sheet.Cells[totalRow, 2].Value = lblTotal.Text;
                        sheet.Cells[totalRow, 2].Style.Font.Bold = true;
                        sheet.Cells[totalRow, 2].Style.Font.Color.SetColor(Color.DarkRed);

                        sheet.Cells[totalRow + 1, 1].Value = "备注：";
                        sheet.Cells[totalRow + 1, 2].Value = txtRemarks.Text;

                        sheet.Cells.AutoFitColumns();
                        package.SaveAs(new FileInfo(sfd.FileName));
                    }
                    MessageBox.Show("导出完成", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region 打印
        private void BtnPrintSetting_Click(object sender, EventArgs e)
        {
            try
            {
                using (PrintSettingForm settingForm = new PrintSettingForm(printPageSettings))
                {
                    if (settingForm.ShowDialog() == DialogResult.OK)
                    {
                        printPageSettings = settingForm.ResultPageSettings;
                        MessageBox.Show("打印设置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印设置异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvItems.Rows.Count == 0)
                {
                    MessageBox.Show("请添加货品后打印", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cbCustomer.SelectedItem == null)
                {
                    MessageBox.Show("请选择客户", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PrintDocument printDoc = new PrintDocument();
                printDoc.DefaultPageSettings = printPageSettings;
                printDoc.PrintPage += PrintDoc_PrintPage;
                printDoc.BeginPrint += PrintDoc_BeginPrint;

                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = printDoc;
                preview.WindowState = FormWindowState.Maximized;
                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印启动失败：{ex.Message}", "打印错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int _printCurrentRowIndex = 0;
        private void PrintDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            _printCurrentRowIndex = 0;
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Rectangle printArea = e.MarginBounds;
                int startX = printArea.Left;
                int startY = printArea.Top;

                Font titleFont = new Font("微软雅黑", 12, FontStyle.Bold);
                Font headFont = new Font("微软雅黑", 9);
                Font tableFont = new Font("微软雅黑", 8);
                Font totalFont = new Font("微软雅黑", 10, FontStyle.Bold);
                Font remarkFont = new Font("微软雅黑", 7);
                Brush blackBrush = Brushes.Black;
                Pen linePen = new Pen(Color.Black, 1);

                int lineHeight = 20;

                g.DrawString("瑞华珠宝兑料单", titleFont, blackBrush, startX + 100, startY);
                startY += 30;

                string custName = (cbCustomer.SelectedItem as Customer)?.Name ?? "未知客户";
                string custText = $"客户：{custName}";
                string dateText = $"日期：{dtpDate.Value:yyyy-MM-dd HH:mm}";
                string typeText = $"单据类型：{cbType.SelectedItem?.ToString() ?? "入库"}";

                g.DrawString(custText, headFont, blackBrush, startX, startY);
                g.DrawString(dateText, headFont, blackBrush, startX + 200, startY);
                g.DrawString(typeText, headFont, blackBrush, startX + 400, startY);
                startY += lineHeight * 2;

                string[] tableHeaders = { "序号", "品名", "毛重", "净重", "成色", "单价", "折价", "总价" };
                int[] colWidths = { 45, 90, 65, 65, 50, 70, 80, 90 };

                List<int> xList = new List<int> { startX };
                int currentX = startX;
                foreach (int w in colWidths)
                {
                    currentX += w;
                    xList.Add(currentX);
                }
                int tableRightX = xList.Last();

                currentX = startX;
                for (int i = 0; i < tableHeaders.Length; i++)
                {
                    g.DrawString(tableHeaders[i], headFont, blackBrush, currentX + 2, startY + 2);
                    currentX += colWidths[i];
                }
                int headerBottom = startY + lineHeight;
                startY = headerBottom;

                g.DrawLine(linePen, startX, headerBottom - lineHeight, tableRightX, headerBottom - lineHeight);
                g.DrawLine(linePen, startX, headerBottom, tableRightX, headerBottom);

                int tableBottom = headerBottom;
                int rowCount = dgvItems.Rows.Count;

                while (_printCurrentRowIndex < rowCount && (startY + lineHeight) < printArea.Bottom - 100)
                {
                    DataGridViewRow row = dgvItems.Rows[_printCurrentRowIndex];
                    currentX = startX;

                    for (int i = 0; i < colWidths.Length; i++)
                    {
                        string txt = "";
                        if (i == 1)
                        {
                            if (int.TryParse(row.Cells[i]?.Value?.ToString(), out int pid))
                            {
                                var p = products.FirstOrDefault(prod => prod.Id == pid);
                                txt = p?.Name ?? "";
                            }
                        }
                        else
                        {
                            txt = row.Cells[i]?.Value?.ToString() ?? "";
                        }

                        g.DrawString(txt, tableFont, blackBrush, currentX + 2, startY + 2);
                        currentX += colWidths[i];
                    }

                    startY += lineHeight;
                    tableBottom = startY;
                    g.DrawLine(linePen, startX, startY, tableRightX, startY);

                    _printCurrentRowIndex++;
                }

                foreach (int x in xList)
                {
                    g.DrawLine(linePen, x, headerBottom - lineHeight, x, tableBottom);
                }

                if (_printCurrentRowIndex >= rowCount)
                {
                    startY += lineHeight;
                    g.DrawString(lblTotal.Text, totalFont, Brushes.DarkRed, startX, startY);
                    startY += lineHeight * 2;

                    g.DrawString($"备注：{txtRemarks.Text}", headFont, blackBrush, startX, startY);
                    startY += lineHeight * 2;

                    g.DrawString("备注：1.货品重量和货款已核对无误，如有错账年内可核查，过期无效。", remarkFont, blackBrush, startX, startY);
                    startY += 15;
                    g.DrawString("2.买方承诺以上物料是合法所有，不是赃物或违法所得，如属赃物或违法所得，卖方完全承担经济和法律责任。", remarkFont, blackBrush, startX, startY);
                }

                e.HasMorePages = _printCurrentRowIndex < rowCount;

                titleFont.Dispose();
                headFont.Dispose();
                tableFont.Dispose();
                totalFont.Dispose();
                remarkFont.Dispose();
                linePen.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void BtnStats_Click(object sender, EventArgs e)
        {
            using (StatsForm statsForm = new StatsForm())
            {
                statsForm.ShowDialog();
            }
        }

        public void LoadOrderToMainForm(Order order)
        {
            if (order == null) return;

            cbCustomer.SelectedValue = order.CustomerId;
            dtpDate.Value = order.OrderDate;
            cbType.SelectedItem = order.OrderType;
            txtRemarks.Text = order.Remarks;

            dgvItems.Rows.Clear();
            foreach (var item in order.Items)
            {
                int rowIndex = dgvItems.Rows.Add();
                var row = dgvItems.Rows[rowIndex];
                row.Cells[colIndex.Name].Value = item.Id;
                row.Cells[colProduct.Name].Value = item.ProductId;
                row.Cells[colGrossWeight.Name].Value = item.GrossWeight;
                row.Cells[colNetWeight.Name].Value = item.NetWeight;
                row.Cells[colPurity.Name].Value = item.Purity;
                row.Cells[colUnitPrice.Name].Value = item.UnitPrice;
                row.Cells[colDiscountedPrice.Name].Value = item.DiscountedPrice;
                row.Cells[colTotalPrice.Name].Value = item.TotalPrice;
            }

            UpdateTotalPrice();
        }
        #endregion
    }

    #region 实体类
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public int GrandTotal { get; set; }
        public string Remarks { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        // ====================== 修复2：改回正确的{ get; set; }（原笔误写成revert back） ======================
        public string ProductName { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal Purity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int TotalPrice { get; set; }
        public decimal StatisticTotalPrice { get; set; }
    }
    #endregion
}