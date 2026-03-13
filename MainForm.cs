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
        // 数据文件路径（固定，不用改）
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string CustomersFile = Path.Combine(DataFolder, "customers.json");
        private static readonly string ProductsFile = Path.Combine(DataFolder, "products.json");
        private static readonly string OrdersFile = Path.Combine(DataFolder, "orders.json");

        // 全局数据
        private List<Customer> customers = new List<Customer>();
        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();

        public MainForm()
        {
            InitializeComponent();
            InitDataFolder();
            LoadAllData();
            BindControlEvents();
        }

        #region 初始化数据
        /// <summary>
        /// 创建数据文件夹，确保目录存在
        /// </summary>
        private void InitDataFolder()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
        }

        /// <summary>
        /// 加载所有JSON数据
        /// </summary>
        /// <summary>
        /// 加载所有JSON数据
        /// </summary>
        private void LoadAllData()
        {
            // 加载客户列表
            if (File.Exists(CustomersFile))
            {
                string json = File.ReadAllText(CustomersFile);
                customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
            }
            else
            {
                // 初始化默认客户
                customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "零售客户" },
            new Customer { Id = 2, Name = "合作门店" }
        };
                SaveCustomersToJson();
            }

            // 加载品类列表
            if (File.Exists(ProductsFile))
            {
                string json = File.ReadAllText(ProductsFile);
                products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }
            else
            {
                // 初始化默认品类
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

            // 加载历史单据
            if (File.Exists(OrdersFile))
            {
                string json = File.ReadAllText(OrdersFile);
                orders = JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
            }
            else
            {
                orders = new List<Order>();
            }

            // 绑定下拉框数据
            cbCustomer.DataSource = customers;
            cbCustomer.DisplayMember = "Name";
            cbCustomer.ValueMember = "Id";

            // 【新增】开启客户搜索功能
            cbCustomer.DropDownStyle = ComboBoxStyle.DropDown; // 允许输入文字
            cbCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend; // 既弹出提示列表，又自动补全
            cbCustomer.AutoCompleteSource = AutoCompleteSource.ListItems; // 用已绑定的客户列表作为搜索数据源

            // 绑定表格的品名下拉
            colProduct.DataSource = products;
            colProduct.DisplayMember = "Name";
            colProduct.ValueMember = "Id";

            // 初始化默认值
            dtpDate.Value = DateTime.Now;
            if (cbType.Items.Count > 0)
                cbType.SelectedIndex = 0;
        }

        /// <summary>
        /// 绑定所有控件的事件
        /// </summary>
        private void BindControlEvents()
        {
            // 按钮事件
            btnAddRow.Click += BtnAddRow_Click;
            btnDeleteRow.Click += BtnDeleteRow_Click;
            btnSave.Click += BtnSave_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnPrint.Click += BtnPrint_Click;
            btnStats.Click += BtnStats_Click;
            btnAddCustomer.Click += BtnAddCustomer_Click;
            btnAddProduct.Click += BtnAddProduct_Click;

            // 表格事件
            dgvItems.CellEndEdit += DgvItems_CellEndEdit;
            dgvItems.RowsAdded += (s, e) => RefreshRowNumber();
        }
        #endregion
        #region JSON数据保存方法（带调试信息）
        private void SaveCustomersToJson()
        {
            try
            {
                // 确保目录存在
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(customers, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(CustomersFile, json);

                // 调试信息（可以删掉）
                Console.WriteLine($"客户数据已保存到：{CustomersFile}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存客户数据失败：{ex.Message}\n路径：{CustomersFile}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProductsToJson()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(ProductsFile, json);

                Console.WriteLine($"品类数据已保存到：{ProductsFile}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存品类数据失败：{ex.Message}\n路径：{ProductsFile}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveOrdersToJson()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(orders, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(OrdersFile, json);

                Console.WriteLine($"单据数据已保存到：{OrdersFile}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存单据数据失败：{ex.Message}\n路径：{OrdersFile}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 界面操作方法
        /// <summary>
        /// 新增客户
        /// </summary>
        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            string custName = Interaction.InputBox("请输入客户名称：", "新增客户", "");
            if (string.IsNullOrWhiteSpace(custName)) return;

            int newId = customers.Count > 0 ? customers.Max(c => c.Id) + 1 : 1;
            customers.Add(new Customer { Id = newId, Name = custName });
            SaveCustomersToJson();

            // 刷新下拉框
            cbCustomer.DataSource = null;
            cbCustomer.DataSource = customers;
            cbCustomer.DisplayMember = "Name";
            cbCustomer.ValueMember = "Id";

            MessageBox.Show("客户新增成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 新增品类
        /// </summary>
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            string prodName = Interaction.InputBox("请输入品类名称：", "新增品类", "");
            if (string.IsNullOrWhiteSpace(prodName)) return;

            int newId = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            products.Add(new Product { Id = newId, Name = prodName });
            SaveProductsToJson();

            // 刷新表格下拉
            colProduct.DataSource = null;
            colProduct.DataSource = products;
            colProduct.DisplayMember = "Name";
            colProduct.ValueMember = "Id";

            MessageBox.Show("品类新增成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 新增表格行
        /// </summary>
        private void BtnAddRow_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvItems.Rows.Add();
            var newRow = dgvItems.Rows[rowIndex];
            newRow.Cells[colPurity.Name].Value = 1m; // 默认成色1
            RefreshRowNumber();
        }

        /// <summary>
        /// 删除选中行
        /// </summary>
        private void BtnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0 || dgvItems.SelectedRows[0].IsNewRow)
            {
                MessageBox.Show("请先选中要删除的行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvItems.Rows.RemoveAt(dgvItems.SelectedRows[0].Index);
            RefreshRowNumber();
            UpdateTotalPrice();
        }

        /// <summary>
        /// 刷新序号列
        /// </summary>
        private void RefreshRowNumber()
        {
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                dgvItems.Rows[i].Cells[colIndex.Name].Value = i + 1;
            }
        }
        #endregion

        #region 自动计算逻辑
        /// <summary>
        /// 单元格编辑后自动计算
        /// </summary>
        private void DgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var currentRow = dgvItems.Rows[e.RowIndex];

            // 1. 计算成色折价 = 成色 * 单价（不四舍五入，直接保留1位小数）
            decimal purity = 0, unitPrice = 0;
            if (decimal.TryParse(currentRow.Cells[colPurity.Name].Value?.ToString(), out purity)
                && decimal.TryParse(currentRow.Cells[colUnitPrice.Name].Value?.ToString(), out unitPrice))
            {
                decimal discountPrice = purity * unitPrice;
                // 按需求：不四舍五入，直接截断小数
                currentRow.Cells[colDiscountedPrice.Name].Value = Math.Truncate(discountPrice * 10) / 10;
            }

            // 2. 计算单项总价 = 净重量 * 成色折价
            decimal netWeight = 0, finalDiscountPrice = 0;
            if (decimal.TryParse(currentRow.Cells[colNetWeight.Name].Value?.ToString(), out netWeight)
                && decimal.TryParse(currentRow.Cells[colDiscountedPrice.Name].Value?.ToString(), out finalDiscountPrice))
            {
                currentRow.Cells[colTotalPrice.Name].Value = netWeight * finalDiscountPrice;
            }

            // 3. 更新单据总价
            UpdateTotalPrice();
        }

        /// <summary>
        /// 更新单据总价
        /// </summary>
        private void UpdateTotalPrice()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (decimal.TryParse(row.Cells[colTotalPrice.Name].Value?.ToString(), out decimal itemTotal))
                {
                    total += itemTotal;
                }
            }
            lblTotal.Text = $"单据总价：{total:F2} 元";
        }
        #endregion

        #region 核心功能按钮
        /// <summary>
        /// 保存单据到JSON
        /// </summary>
        /// <summary>
        /// 保存单据到JSON（带详细提示）
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (dgvItems.Rows.Count == 0)
            {
                MessageBox.Show("请先添加明细数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedCust = cbCustomer.SelectedItem as Customer;
                if (selectedCust == null)
                {
                    MessageBox.Show("请先选择客户", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal grandTotal = 0;
                decimal.TryParse(lblTotal.Text.Replace("单据总价：", "").Replace("元", "").Trim(), out grandTotal);

                // 生成新单据
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

                // 填充明细
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    var product = products.FirstOrDefault(p => p.Id == Convert.ToInt32(row.Cells[colProduct.Name].Value));
                    newOrder.Items.Add(new OrderItem
                    {
                        Id = newOrder.Items.Count + 1,
                        ProductId = product?.Id ?? 0,
                        ProductName = product?.Name ?? "",
                        GrossWeight = Convert.ToDecimal(row.Cells[colGrossWeight.Name].Value ?? 0),
                        NetWeight = Convert.ToDecimal(row.Cells[colNetWeight.Name].Value ?? 0),
                        Purity = Convert.ToDecimal(row.Cells[colPurity.Name].Value ?? 1),
                        UnitPrice = Convert.ToDecimal(row.Cells[colUnitPrice.Name].Value ?? 0),
                        DiscountedPrice = Convert.ToDecimal(row.Cells[colDiscountedPrice.Name].Value ?? 0),
                        TotalPrice = Convert.ToDecimal(row.Cells[colTotalPrice.Name].Value ?? 0)
                    });
                }

                // 保存到JSON
                orders.Add(newOrder);
                SaveOrdersToJson();

                // 成功提示，包含文件路径
                MessageBox.Show(
                    $"单据保存成功！\n单据号：{newOrderId}\n\n数据文件保存在：\n{DataFolder}",
                    "成功",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 清空界面，方便下一次录入
                dgvItems.Rows.Clear();
                txtRemarks.Clear();
                UpdateTotalPrice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}\n\n堆栈跟踪：{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <summary>
        /// 导出Excel（已修复品名列显示数字的问题）
        /// </summary>
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

                        // 标题
                        sheet.Cells["A1"].Value = "瑞华珠宝兑料单";
                        sheet.Cells["A1:H1"].Merge = true;
                        sheet.Cells["A1"].Style.Font.Size = 16;
                        sheet.Cells["A1"].Style.Font.Bold = true;
                        sheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        // 单据头
                        sheet.Cells["A3"].Value = "客户：";
                        sheet.Cells["B3"].Value = (cbCustomer.SelectedItem as Customer)?.Name;
                        sheet.Cells["D3"].Value = "日期：";
                        sheet.Cells["E3"].Value = dtpDate.Value.ToString("yyyy-MM-dd");
                        sheet.Cells["G3"].Value = "单据类型：";
                        sheet.Cells["H3"].Value = cbType.SelectedItem.ToString();

                        // 表格表头
                        string[] headers = { "序号", "品名", "毛重量", "净重量", "成色", "单价", "成色折价", "单项总价" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            sheet.Cells[5, i + 1].Value = headers[i];
                            sheet.Cells[5, i + 1].Style.Font.Bold = true;
                        }

                        // 填充明细（已修复品名列）
                        for (int row = 0; row < dgvItems.Rows.Count; row++)
                        {
                            for (int col = 0; col < dgvItems.Columns.Count; col++)
                            {
                                // 特殊处理品名列（第2列，索引1）：根据ID查品名
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
                                    // 其他列正常导出
                                    sheet.Cells[row + 6, col + 1].Value = dgvItems.Rows[row].Cells[col].Value;
                                }
                            }
                        }

                        // 总价
                        int totalRow = dgvItems.Rows.Count + 7;
                        sheet.Cells[totalRow, 1].Value = "单据总价：";
                        sheet.Cells[totalRow, 2].Value = lblTotal.Text;
                        sheet.Cells[totalRow, 2].Style.Font.Bold = true;
                        sheet.Cells[totalRow, 2].Style.Font.Color.SetColor(Color.DarkRed);

                        // 备注
                        sheet.Cells[totalRow + 1, 1].Value = "备注：";
                        sheet.Cells[totalRow + 1, 2].Value = txtRemarks.Text;

                        // 自动适配列宽
                        sheet.Cells.AutoFitColumns();

                        // 保存文件
                        package.SaveAs(new FileInfo(sfd.FileName));
                    }
                    MessageBox.Show("Excel导出成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 打印功能（适配针式打印机）
        /// </summary>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDoc;
            preview.WindowState = FormWindowState.Maximized;
            preview.ShowDialog();
        }

        /// <summary>
        /// 打印内容绘制
        /// </summary>
        private void PrintDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("微软雅黑", 16, FontStyle.Bold);
            Font headFont = new Font("微软雅黑", 10);
            Font tableFont = new Font("微软雅黑", 9);
            Font totalFont = new Font("微软雅黑", 12, FontStyle.Bold);
            Brush blackBrush = Brushes.Black;

            int startX = 50;
            int startY = 50;
            int lineHeight = 25;

            // 标题
            g.DrawString("瑞华珠宝兑料单", titleFont, blackBrush, startX + 300, startY);
            startY += 40;

            // 单据头
            g.DrawString($"客户：{(cbCustomer.SelectedItem as Customer)?.Name}", headFont, blackBrush, startX, startY);
            g.DrawString($"日期：{dtpDate.Value:yyyy-MM-dd}", headFont, blackBrush, startX + 300, startY);
            g.DrawString($"单据类型：{cbType.SelectedItem.ToString()}", headFont, blackBrush, startX + 600, startY);
            startY += lineHeight * 2;

            // 表格表头
            string[] tableHeaders = { "序号", "品名", "毛重量", "净重量", "成色", "单价", "成色折价", "单项总价" };
            int[] colWidths = { 60, 100, 90, 90, 70, 90, 100, 120 };
            int currentX = startX;
            for (int i = 0; i < tableHeaders.Length; i++)
            {
                g.DrawString(tableHeaders[i], headFont, blackBrush, currentX, startY);
                currentX += colWidths[i];
            }
            startY += lineHeight;
            g.DrawLine(Pens.Black, startX, startY - 5, startX + colWidths.Sum(), startY - 5);

            // 表格明细
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                currentX = startX;
                for (int i = 0; i < dgvItems.Columns.Count; i++)
                {
                    object cellValue = row.Cells[i].Value;
                    g.DrawString(cellValue?.ToString() ?? "", tableFont, blackBrush, currentX, startY);
                    currentX += colWidths[i];
                }
                startY += lineHeight;
            }

            // 分隔线
            g.DrawLine(Pens.Black, startX, startY, startX + colWidths.Sum(), startY);
            startY += lineHeight;

            // 总价
            g.DrawString(lblTotal.Text, totalFont, Brushes.DarkRed, startX, startY);
            startY += lineHeight * 2;

            // 备注
            g.DrawString($"备注：{txtRemarks.Text}", headFont, blackBrush, startX, startY);

            // 底部备注说明
            startY += lineHeight * 2;
            g.DrawString("备注：1.货品重量和货款已核对无误，如有错账年内可核查，过期无效。", tableFont, blackBrush, startX, startY);
            startY += lineHeight;
            g.DrawString("2.买方承诺以上物料是合法所有，不是赃物或违法所得，如属赃物或违法所得，卖方完全承担经济和法律责任。", tableFont, blackBrush, startX, startY);
        }

        /// <summary>
        /// 统计中心（后续完善）
        /// </summary>
        /// <summary>
        /// 统计中心（已完善）
        /// </summary>
        private void BtnStats_Click(object sender, EventArgs e)
        {
            using (StatsForm statsForm = new StatsForm())
            {
                statsForm.ShowDialog();
            }
        }
        #endregion
    }

    #region 数据模型（全部写在这里，不会重复）
    /// <summary>
    /// 客户模型
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 品类模型
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 单据主表模型
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public decimal GrandTotal { get; set; }
        public string Remarks { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    /// <summary>
    /// 单据明细模型
    /// </summary>
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal Purity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    #endregion
}