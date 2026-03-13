using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace JewelryTool
{
    public partial class HistoryForm : Form
    {
        // 数据路径与主窗体完全一致，确保读取的是同一个文件
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string CustomersFile = Path.Combine(DataFolder, "customers.json");
        private static readonly string ProductsFile = Path.Combine(DataFolder, "products.json");
        private static readonly string OrdersFile = Path.Combine(DataFolder, "orders.json");

        // 全局数据，每次加载都会重新实例化，避免缓存旧数据
        private List<Customer> customers = new List<Customer>();
        private List<Product> products = new List<Product>();
        private List<Order> orders = new List<Order>();
        private Order selectedOrder = null;
        private MainForm mainForm;

        // 窗体加载完成标记
        private bool isFormLoaded = false;

        public HistoryForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            // 窗体打开时，强制从文件读取最新数据
            ReloadAllDataFromFile();
            BindAllControls();

            isFormLoaded = true;
            FilterOrders();
        }

        #region 核心数据加载（修复重点：强制从文件重新读取，彻底清除旧数据缓存）
        /// <summary>
        /// 从JSON文件重新加载所有最新数据，包括新增的客户、品类、单据
        /// </summary>
        private void ReloadAllDataFromFile()
        {
            try
            {
                // 先清空所有旧数据，避免缓存
                customers.Clear();
                products.Clear();
                orders.Clear();

                // 重新读取客户文件（核心修复：确保拿到主界面新增的客户）
                if (File.Exists(CustomersFile))
                {
                    string json = File.ReadAllText(CustomersFile);
                    customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
                }
                // 兜底：客户列表为空时，添加默认值
                if (customers.Count == 0)
                {
                    customers = new List<Customer>
                    {
                        new Customer { Id = 1, Name = "零售客户" },
                        new Customer { Id = 2, Name = "合作门店" }
                    };
                }

                // 重新读取品类文件
                if (File.Exists(ProductsFile))
                {
                    string json = File.ReadAllText(ProductsFile);
                    products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
                }

                // 重新读取单据文件
                if (File.Exists(OrdersFile))
                {
                    string json = File.ReadAllText(OrdersFile);
                    orders = JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载最新数据失败：{ex.Message}", "数据加载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 绑定所有下拉框控件，确保数据源是最新的
        /// </summary>
        private void BindAllControls()
        {
            BindFilterControls();
            BindEditControls();
        }
        #endregion

        #region 控件绑定逻辑（修复重点：彻底重置控件数据源，避免旧数据残留）
        /// <summary>
        /// 绑定顶部筛选栏的客户下拉框
        /// </summary>
        private void BindFilterControls()
        {
            // 先彻底解绑事件和数据源，避免旧数据残留
            cbFilterCustomer.SelectedIndexChanged -= Filter_Changed;
            cbFilterCustomer.DataSource = null;
            cbFilterType.SelectedIndexChanged -= Filter_Changed;
            cbFilterType.DataSource = null;
            dtpStartDate.ValueChanged -= Filter_Changed;
            dtpEndDate.ValueChanged -= Filter_Changed;

            // 重新绑定客户下拉框（包含全部客户+最新新增的客户）
            var customerFilterList = new List<Customer>
            {
                new Customer { Id = 0, Name = "全部客户" }
            };
            customerFilterList.AddRange(customers); // 加入最新的客户列表
            cbFilterCustomer.DataSource = customerFilterList;
            cbFilterCustomer.DisplayMember = "Name";
            cbFilterCustomer.ValueMember = "Id";

            // 绑定单据类型
            cbFilterType.DataSource = new List<string> { "全部类型", "入库", "出库" };

            // 日期范围默认值
            dtpStartDate.Value = DateTime.Now.AddDays(-30).Date;
            dtpEndDate.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            // 重新绑定事件
            cbFilterCustomer.SelectedIndexChanged += Filter_Changed;
            cbFilterType.SelectedIndexChanged += Filter_Changed;
            dtpStartDate.ValueChanged += Filter_Changed;
            dtpEndDate.ValueChanged += Filter_Changed;
        }

        /// <summary>
        /// 绑定底部修改区域的客户下拉框
        /// </summary>
        private void BindEditControls()
        {
            // 彻底重置数据源，确保显示最新客户
            cbEditCustomer.DataSource = null;
            cbEditCustomer.DataSource = customers;
            cbEditCustomer.DisplayMember = "Name";
            cbEditCustomer.ValueMember = "Id";

            // 绑定单据类型
            cbEditType.DataSource = null;
            cbEditType.DataSource = new List<string> { "入库", "出库" };

            // 日期格式设置
            dtpEditDate.Format = DateTimePickerFormat.Custom;
            dtpEditDate.CustomFormat = "yyyy-MM-dd HH:mm";
        }
        #endregion

        #region 单据筛选与列表加载
        private void FilterOrders()
        {
            if (!isFormLoaded) return;

            try
            {
                var filteredOrders = orders.AsQueryable();

                // 客户筛选
                if (cbFilterCustomer.SelectedValue != null && (int)cbFilterCustomer.SelectedValue != 0)
                {
                    int custId = (int)cbFilterCustomer.SelectedValue;
                    filteredOrders = filteredOrders.Where(o => o.CustomerId == custId);
                }

                // 单据类型筛选
                if (cbFilterType.SelectedItem != null && cbFilterType.SelectedItem.ToString() != "全部类型")
                {
                    string type = cbFilterType.SelectedItem.ToString();
                    filteredOrders = filteredOrders.Where(o => o.OrderType == type);
                }

                // 日期范围筛选
                filteredOrders = filteredOrders.Where(o => o.OrderDate >= dtpStartDate.Value && o.OrderDate <= dtpEndDate.Value);

                // 按日期倒序排序，最新单据在最上
                var orderList = filteredOrders.OrderByDescending(o => o.OrderDate).Select(o => new
                {
                    单据ID = o.Id,
                    单据日期 = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    客户名称 = o.CustomerName,
                    单据类型 = o.OrderType,
                    单据总额 = o.GrandTotal,
                    备注 = o.Remarks
                }).ToList();

                // 刷新单据列表
                dgvOrderList.SelectionChanged -= dgvOrderList_SelectionChanged;
                dgvOrderList.DataSource = null;
                dgvOrderList.DataSource = orderList;
                dgvOrderList.SelectionChanged += dgvOrderList_SelectionChanged;

                // 格式化金额列
                if (dgvOrderList.Columns.Contains("单据总额"))
                {
                    dgvOrderList.Columns["单据总额"].DefaultCellStyle.Format = "F2";
                }

                // 清空明细
                dgvOrderDetail.DataSource = null;
                selectedOrder = null;
                ClearEditControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"筛选单据失败：{ex.Message}", "筛选错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            FilterOrders();
        }

        private void dgvOrderList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrderList.SelectedRows.Count == 0)
            {
                selectedOrder = null;
                dgvOrderDetail.DataSource = null;
                ClearEditControls();
                return;
            }

            try
            {
                var idCell = dgvOrderList.SelectedRows[0].Cells["单据ID"].Value;
                if (idCell == null || !int.TryParse(idCell.ToString(), out int orderId))
                {
                    return;
                }

                selectedOrder = orders.FirstOrDefault(o => o.Id == orderId);
                if (selectedOrder == null) return;

                // 绑定明细到表格
                var detailList = selectedOrder.Items.Select(item => new
                {
                    序号 = item.Id,
                    品名 = item.ProductName,
                    毛重量 = item.GrossWeight,
                    净重量 = item.NetWeight,
                    成色 = item.Purity,
                    单价 = item.UnitPrice,
                    成色折价 = item.DiscountedPrice,
                    单项总价 = item.TotalPrice
                }).ToList();

                dgvOrderDetail.DataSource = null;
                dgvOrderDetail.DataSource = detailList;

                // 格式化数字列
                foreach (DataGridViewColumn col in dgvOrderDetail.Columns)
                {
                    if (col.Name != "序号" && col.Name != "品名")
                    {
                        col.DefaultCellStyle.Format = "F2";
                    }
                }

                // 回填单据信息到修改区域
                txtEditOrderId.Text = selectedOrder.Id.ToString();
                cbEditCustomer.SelectedValue = selectedOrder.CustomerId;
                dtpEditDate.Value = selectedOrder.OrderDate;
                cbEditType.SelectedItem = selectedOrder.OrderType;
                txtEditRemarks.Text = selectedOrder.Remarks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载单据明细失败：{ex.Message}", "加载错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearEditControls()
        {
            txtEditOrderId.Clear();
            txtEditRemarks.Clear();
        }
        #endregion

        #region 单据操作功能
        private void BtnSaveEdit_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("请先选择要修改的单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("确定要修改该单据吗？修改后将覆盖原数据，不可恢复！", "确认修改", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var editCustomer = cbEditCustomer.SelectedItem as Customer;
                if (editCustomer == null)
                {
                    MessageBox.Show("请选择有效客户", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedOrder.CustomerId = editCustomer.Id;
                selectedOrder.CustomerName = editCustomer.Name;
                selectedOrder.OrderDate = dtpEditDate.Value;
                selectedOrder.OrderType = cbEditType.SelectedItem?.ToString() ?? "入库";
                selectedOrder.Remarks = txtEditRemarks.Text;

                // 更新明细数据
                var newItems = new List<OrderItem>();
                foreach (DataGridViewRow row in dgvOrderDetail.Rows)
                {
                    newItems.Add(new OrderItem
                    {
                        Id = Convert.ToInt32(row.Cells["序号"].Value ?? 0),
                        ProductName = row.Cells["品名"].Value?.ToString() ?? "",
                        GrossWeight = Convert.ToDecimal(row.Cells["毛重量"].Value ?? 0),
                        NetWeight = Convert.ToDecimal(row.Cells["净重量"].Value ?? 0),
                        Purity = Convert.ToDecimal(row.Cells["成色"].Value ?? 0),
                        UnitPrice = Convert.ToDecimal(row.Cells["单价"].Value ?? 0),
                        DiscountedPrice = Convert.ToDecimal(row.Cells["成色折价"].Value ?? 0),
                        TotalPrice = Convert.ToDecimal(row.Cells["单项总价"].Value ?? 0)
                    });
                }
                selectedOrder.Items = newItems;
                selectedOrder.GrandTotal = newItems.Sum(i => i.TotalPrice);

                // 保存到JSON文件
                SaveOrdersToJson();

                // 刷新列表
                ReloadAllDataFromFile();
                BindAllControls();
                FilterOrders();

                MessageBox.Show("单据修改成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteOrder_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("请先选择要删除的单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"确定要删除单据号【{selectedOrder.Id}】吗？删除后不可恢复！", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                orders.Remove(selectedOrder);
                SaveOrdersToJson();

                // 刷新列表
                ReloadAllDataFromFile();
                FilterOrders();

                MessageBox.Show("单据删除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoadToMain_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("请先选择要调取的单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("确定要把该单据调取到主界面吗？主界面当前未保存的数据会被清空！", "确认调取", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (mainForm != null)
            {
                mainForm.LoadOrderToMainForm(selectedOrder);
            }

            this.Close();
        }

        /// <summary>
        /// 刷新按钮：强制重新读取所有最新数据，包括新增的客户
        /// </summary>
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            ReloadAllDataFromFile();
            BindAllControls();
            FilterOrders();
            MessageBox.Show("所有数据已刷新，新增客户/品类已同步！", "刷新完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveOrdersToJson()
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
                MessageBox.Show($"保存单据数据失败：{ex.Message}", "保存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvOrderDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var currentRow = dgvOrderDetail.Rows[e.RowIndex];

            try
            {
                // 重新计算成色折价
                decimal purity = 0, unitPrice = 0;
                if (decimal.TryParse(currentRow.Cells["成色"].Value?.ToString(), out purity)
                    && decimal.TryParse(currentRow.Cells["单价"].Value?.ToString(), out unitPrice))
                {
                    decimal discountPrice = purity * unitPrice;
                    currentRow.Cells["成色折价"].Value = Math.Truncate(discountPrice * 10) / 10;
                }

                // 重新计算单项总价
                decimal netWeight = 0, finalDiscountPrice = 0;
                if (decimal.TryParse(currentRow.Cells["净重量"].Value?.ToString(), out netWeight)
                    && decimal.TryParse(currentRow.Cells["成色折价"].Value?.ToString(), out finalDiscountPrice))
                {
                    currentRow.Cells["单项总价"].Value = netWeight * finalDiscountPrice;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"计算失败：{ex.Message}", "计算错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}