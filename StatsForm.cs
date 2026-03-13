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
        // 数据文件路径（和主窗体保持一致）
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string ProductsFile = Path.Combine(DataFolder, "products.json");
        private static readonly string OrdersFile = Path.Combine(DataFolder, "orders.json");

        // 全局数据
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
            // 加载品类
            if (File.Exists(ProductsFile))
            {
                string json = File.ReadAllText(ProductsFile);
                products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }

            // 加载单据
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

        /// <summary>
        /// 1. 品类库存&均价统计
        /// </summary>
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
                            库存均价 = g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.TotalPrice ?? 0)
                                        / (g.Where(x => x != null && orders.First(o => o.Items.Contains(x)).OrderType == "入库").Sum(x => x?.NetWeight ?? 0) + 0.0001m)
                        };

            dgvProductStats.DataSource = stats.ToList();

            // 格式化数字列
            foreach (DataGridViewColumn col in dgvProductStats.Columns)
            {
                if (col.Name.Contains("重量") || col.Name.Contains("金额") || col.Name.Contains("均价"))
                {
                    col.DefaultCellStyle.Format = "F2";
                }
            }
        }

        /// <summary>
        /// 2. 每百克阶梯均价（按客户要求的阶梯）
        /// </summary>
        private void CalculatePriceStats()
        {
            // 定义阶梯（示例：0-100g, 100-500g, 500g以上）
            var tiers = new[]
            {
                new { Min = 0m, Max = 100m, Name = "0-100克" },
                new { Min = 100m, Max = 500m, Name = "100-500克" },
                new { Min = 500m, Max = decimal.MaxValue, Name = "500克以上" }
            };

            var stats = from tier in tiers
                        let inItems = orders.Where(o => o.OrderType == "入库").SelectMany(o => o.Items)
                        let tierItems = inItems.Where(x => x.NetWeight >= tier.Min && x.NetWeight < tier.Max)
                        select new
                        {
                            重量阶梯 = tier.Name,
                            入库总重量 = tierItems.Sum(x => x.NetWeight),
                            入库总金额 = tierItems.Sum(x => x.TotalPrice),
                            每百克均价 = tierItems.Sum(x => x.TotalPrice) / (tierItems.Sum(x => x.NetWeight) + 0.0001m) * 100
                        };

            dgvPriceStats.DataSource = stats.ToList();

            // 格式化数字列
            foreach (DataGridViewColumn col in dgvPriceStats.Columns)
            {
                if (col.Name != "重量阶梯")
                {
                    col.DefaultCellStyle.Format = "F2";
                }
            }
        }

        /// <summary>
        /// 3. 每日出入库统计
        /// </summary>
        private void CalculateDailyStats()
        {
            var stats = from o in orders
                        group o by o.OrderDate.Date into g
                        orderby g.Key descending
                        select new
                        {
                            日期 = g.Key.ToString("yyyy-MM-dd"),
                            入库总重量 = g.Where(x => x.OrderType == "入库").SelectMany(x => x.Items).Sum(x => x.NetWeight),
                            入库总金额 = g.Where(x => x.OrderType == "入库").Sum(x => x.GrandTotal),
                            出库总重量 = g.Where(x => x.OrderType == "出库").SelectMany(x => x.Items).Sum(x => x.NetWeight),
                            出库总金额 = g.Where(x => x.OrderType == "出库").Sum(x => x.GrandTotal),
                            当日净库存 = g.Where(x => x.OrderType == "入库").SelectMany(x => x.Items).Sum(x => x.NetWeight)
                                         - g.Where(x => x.OrderType == "出库").SelectMany(x => x.Items).Sum(x => x.NetWeight)
                        };

            dgvDailyStats.DataSource = stats.ToList();

            // 格式化数字列
            foreach (DataGridViewColumn col in dgvDailyStats.Columns)
            {
                if (col.Name != "日期")
                {
                    col.DefaultCellStyle.Format = "F2";
                }
            }
        }
        #endregion

        #region 按钮事件
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllData();
            CalculateAllStats();
            MessageBox.Show("数据已刷新！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        // 1. 导出品类库存&均价
                        var sheet1 = package.Workbook.Worksheets.Add("品类库存&均价");
                        ExportGridViewToSheet(dgvProductStats, sheet1);

                        // 2. 导出每百克阶梯均价
                        var sheet2 = package.Workbook.Worksheets.Add("每百克阶梯均价");
                        ExportGridViewToSheet(dgvPriceStats, sheet2);

                        // 3. 导出每日出入库
                        var sheet3 = package.Workbook.Worksheets.Add("每日出入库统计");
                        ExportGridViewToSheet(dgvDailyStats, sheet3);

                        package.SaveAs(new FileInfo(sfd.FileName));
                    }
                    MessageBox.Show("统计报表导出成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 通用导出方法
        /// </summary>
        private void ExportGridViewToSheet(DataGridView dgv, ExcelWorksheet sheet)
        {
            // 表头
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = dgv.Columns[i].HeaderText;
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // 数据
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