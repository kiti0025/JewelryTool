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
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string CustomersFilePath = Path.Combine(DataFolder, "customers.json");
        private static readonly string OrdersFilePath = Path.Combine(DataFolder, "orders.json");

        private List<Customer> _customerList = new List<Customer>();
        private List<Order> _orderList = new List<Order>();
        private Order _currentSelectedOrder = null;
        private readonly MainForm _mainFormInstance;

        private bool _formInitialized = false;
        private FileSystemWatcher _ordersWatcher;
        private Timer _reloadTimer;

        public HistoryForm(MainForm mainForm)
        {
            InitializeComponent();
            _mainFormInstance = mainForm;
            ForceReloadAllDataFromDisk();
            InitializeOrdersFileWatcher();
        }

        private void InitializeOrdersFileWatcher()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                _reloadTimer = new Timer();
                _reloadTimer.Interval = 300;
                _reloadTimer.Tick += ReloadTimer_Tick;

                _ordersWatcher = new FileSystemWatcher(Path.GetDirectoryName(OrdersFilePath) ?? DataFolder, Path.GetFileName(OrdersFilePath));
                _ordersWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
                _ordersWatcher.Changed += OnOrdersFileChanged;
                _ordersWatcher.Created += OnOrdersFileChanged;
                _ordersWatcher.Renamed += OnOrdersFileChanged;
                _ordersWatcher.Deleted += OnOrdersFileChanged;
                _ordersWatcher.EnableRaisingEvents = true;
            }
            catch { }
        }

        private void OnOrdersFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (IsHandleCreated)
                {
                    BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            _reloadTimer?.Stop();
                            _reloadTimer?.Start();
                        }
                        catch { }
                    }));
                }
            }
            catch { }
        }

        private void ReloadTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _reloadTimer?.Stop();
                ForceReloadAllDataFromDisk();
                if (_formInitialized)
                    RefreshOrderListWithFilter();
            }
            catch { }
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            BindAllDropdownControls();
            _formInitialized = true;
            RefreshOrderListWithFilter();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            try
            {
                if (_ordersWatcher != null)
                {
                    _ordersWatcher.EnableRaisingEvents = false;
                    _ordersWatcher.Changed -= OnOrdersFileChanged;
                    _ordersWatcher.Created -= OnOrdersFileChanged;
                    _ordersWatcher.Renamed -= OnOrdersFileChanged;
                    _ordersWatcher.Deleted -= OnOrdersFileChanged;
                    _ordersWatcher.Dispose();
                    _ordersWatcher = null;
                }

                if (_reloadTimer != null)
                {
                    _reloadTimer.Tick -= ReloadTimer_Tick;
                    _reloadTimer.Stop();
                    _reloadTimer.Dispose();
                    _reloadTimer = null;
                }
            }
            catch { }
        }

        #region 数据加载
        private void ForceReloadAllDataFromDisk()
        {
            try
            {
                _customerList.Clear();
                _orderList.Clear();

                if (File.Exists(CustomersFilePath))
                {
                    string customerJson = File.ReadAllText(CustomersFilePath);
                    _customerList = JsonConvert.DeserializeObject<List<Customer>>(customerJson) ?? new List<Customer>();
                }

                if (_customerList.Count == 0)
                {
                    _customerList = new List<Customer>
                    {
                        new Customer { Id = 1, Name = "零售客户" },
                        new Customer { Id = 2, Name = "合作门店" }
                    };
                }

                if (File.Exists(OrdersFilePath))
                {
                    string orderJson = File.ReadAllText(OrdersFilePath);
                    _orderList = JsonConvert.DeserializeObject<List<Order>>(orderJson) ?? new List<Order>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 下拉框绑定
        private void BindAllDropdownControls()
        {
            cbFilterCustomer.SelectedIndexChanged -= OnFilterConditionChanged;
            cbFilterCustomer.DataSource = null;

            var filterCustomerList = new List<Customer>
            {
                new Customer { Id = 0, Name = "全部客户" }
            };
            filterCustomerList.AddRange(_customerList);

            cbFilterCustomer.DataSource = filterCustomerList;
            cbFilterCustomer.DisplayMember = nameof(Customer.Name);
            cbFilterCustomer.ValueMember = nameof(Customer.Id);
            cbFilterCustomer.SelectedIndexChanged += OnFilterConditionChanged;

            cbFilterType.SelectedIndexChanged -= OnFilterConditionChanged;
            cbFilterType.DataSource = null;
            cbFilterType.DataSource = new List<string> { "全部类型", "入库", "出库" };
            cbFilterType.SelectedIndexChanged += OnFilterConditionChanged;

            dtpStartDate.ValueChanged -= OnFilterConditionChanged;
            dtpEndDate.ValueChanged -= OnFilterConditionChanged;
            dtpStartDate.Value = DateTime.Now.AddDays(-30).Date;
            dtpEndDate.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            dtpStartDate.ValueChanged += OnFilterConditionChanged;
            dtpEndDate.ValueChanged += OnFilterConditionChanged;

            cbEditCustomer.DataSource = null;
            cbEditCustomer.DataSource = _customerList;
            cbEditCustomer.DisplayMember = nameof(Customer.Name);
            cbEditCustomer.ValueMember = nameof(Customer.Id);

            cbEditType.DataSource = null;
            cbEditType.DataSource = new List<string> { "入库", "出库" };

            dtpEditDate.Format = DateTimePickerFormat.Custom;
            dtpEditDate.CustomFormat = "yyyy-MM-dd HH:mm";
        }
        #endregion

        #region 筛选与列表
        private void OnFilterConditionChanged(object sender, EventArgs e)
        {
            if (!_formInitialized) return;
            RefreshOrderListWithFilter();
        }

        private void RefreshOrderListWithFilter()
        {
            try
            {
                int filterCustomerId = 0;
                string filterCustomerName = null;

                if (cbFilterCustomer.SelectedItem is Customer selCust)
                {
                    filterCustomerId = selCust.Id;
                    filterCustomerName = selCust.Name;
                }
                else if (cbFilterCustomer.SelectedValue != null && int.TryParse(cbFilterCustomer.SelectedValue.ToString(), out int custId))
                {
                    filterCustomerId = custId;
                    var found = _customerList.FirstOrDefault(c => c.Id == custId);
                    if (found != null)
                        filterCustomerName = found.Name;
                }
                else if (cbFilterCustomer.SelectedItem != null)
                {
                    filterCustomerName = cbFilterCustomer.SelectedItem.ToString();
                }

                string filterOrderType = cbFilterType.SelectedItem?.ToString()?.Trim() ?? "全部类型";
                DateTime filterStartDate = dtpStartDate.Value.Date;
                DateTime filterEndDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);

                var filteredOrders = _orderList.Where(order =>
                {
                    if (filterCustomerId != 0)
                    {
                        bool matchById = order.CustomerId == filterCustomerId;
                        bool matchByName = false;
                        if (!matchById && !string.IsNullOrEmpty(filterCustomerName) && !string.IsNullOrEmpty(order.CustomerName))
                        {
                            matchByName = string.Equals(order.CustomerName.Trim(), filterCustomerName.Trim(), StringComparison.Ordinal);
                        }
                        if (!matchById && !matchByName)
                            return false;
                    }

                    if (filterOrderType != "全部类型")
                    {
                        var ordType = order.OrderType?.Trim() ?? string.Empty;
                        if (!string.Equals(ordType, filterOrderType, StringComparison.Ordinal))
                            return false;
                    }

                    if (order.OrderDate < filterStartDate || order.OrderDate > filterEndDate)
                        return false;

                    return true;
                }).OrderByDescending(order => order.OrderDate).ToList();

                var displayList = filteredOrders.Select((order, index) => new
                {
                    行号 = index + 1,
                    单据ID = order.Id,
                    单据日期 = order.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    客户名称 = order.CustomerName,
                    单据类型 = order.OrderType,
                    单据总额 = order.GrandTotal,
                    备注 = order.Remarks
                }).ToList();

                dgvOrderList.SelectionChanged -= OnOrderSelectionChanged;
                dgvOrderList.DataSource = null;
                dgvOrderList.DataSource = displayList;
                dgvOrderList.SelectionChanged += OnOrderSelectionChanged;

                // 移除单据总额的小数格式（改为整数）
                if (dgvOrderList.Columns.Contains("单据总额"))
                {
                    dgvOrderList.Columns["单据总额"].DefaultCellStyle.Format = "";
                }

                dgvOrderList.Columns["行号"].Width = 60;
                dgvOrderList.Columns["单据ID"].Width = 80;
                dgvOrderList.Columns["单据日期"].Width = 160;
                dgvOrderList.Columns["客户名称"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvOrderList.Columns["单据类型"].Width = 80;
                dgvOrderList.Columns["单据总额"].Width = 120;

                ClearOrderDetailAndEditArea();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新列表失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnOrderSelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrderList.SelectedRows.Count == 0)
            {
                ClearOrderDetailAndEditArea();
                return;
            }

            try
            {
                var orderIdCell = dgvOrderList.SelectedRows[0].Cells["单据ID"].Value;
                if (orderIdCell == null || !int.TryParse(orderIdCell.ToString(), out int selectedOrderId))
                {
                    ClearOrderDetailAndEditArea();
                    return;
                }

                _currentSelectedOrder = _orderList.FirstOrDefault(o => o.Id == selectedOrderId);
                if (_currentSelectedOrder == null)
                {
                    ClearOrderDetailAndEditArea();
                    return;
                }

                LoadOrderDetailToGrid(_currentSelectedOrder);
                FillEditAreaWithOrderData(_currentSelectedOrder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载明细失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrderDetailToGrid(Order order)
        {
            if (order?.Items == null)
            {
                dgvOrderDetail.DataSource = null;
                return;
            }

            var detailList = order.Items.Select(item => new
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

            foreach (DataGridViewColumn col in dgvOrderDetail.Columns)
            {
                if (string.IsNullOrEmpty(col.Name))
                    col.Name = col.HeaderText;

                // 仅重量/单价保留小数，单项总价移除小数格式
                if (col.Name != "序号" && col.Name != "品名" && col.Name != "单项总价")
                {
                    col.DefaultCellStyle.Format = "F2";
                }
                else
                {
                    col.DefaultCellStyle.Format = "";
                }
            }
        }

        private void FillEditAreaWithOrderData(Order order)
        {
            txtEditOrderId.Text = order.Id.ToString();
            cbEditCustomer.SelectedValue = order.CustomerId;
            dtpEditDate.Value = order.OrderDate;
            cbEditType.SelectedItem = order.OrderType;
            txtEditRemarks.Text = order.Remarks;
        }

        private void ClearOrderDetailAndEditArea()
        {
            _currentSelectedOrder = null;
            dgvOrderDetail.DataSource = null;
            txtEditOrderId.Clear();
            txtEditRemarks.Clear();
        }
        #endregion

        #region 按钮事件
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            ForceReloadAllDataFromDisk();
            BindAllDropdownControls();
            RefreshOrderListWithFilter();
            MessageBox.Show("数据已刷新", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSaveEdit_Click(object sender, EventArgs e)
        {
            if (_currentSelectedOrder == null)
            {
                MessageBox.Show("请选择要修改的单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("确定修改？修改后无法恢复", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var selectedCustomer = cbEditCustomer.SelectedItem as Customer;
                if (selectedCustomer == null)
                {
                    MessageBox.Show("请选择客户", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _currentSelectedOrder.CustomerId = selectedCustomer.Id;
                _currentSelectedOrder.CustomerName = selectedCustomer.Name;
                _currentSelectedOrder.OrderDate = dtpEditDate.Value;
                _currentSelectedOrder.OrderType = cbEditType.SelectedItem?.ToString() ?? "入库";
                _currentSelectedOrder.Remarks = txtEditRemarks.Text;

                var newItemList = new List<OrderItem>();
                foreach (DataGridViewRow row in dgvOrderDetail.Rows)
                {
                    if (row.IsNewRow) continue;

                    int itemId = 0;
                    if (row.Cells["序号"].Value != null && int.TryParse(row.Cells["序号"].Value.ToString(), out int parsedId))
                        itemId = parsedId;

                    string productName = row.Cells["品名"].Value?.ToString() ?? string.Empty;

                    decimal grossWeight = TryParseDecimalCell(row, "毛重量");
                    decimal netWeight = TryParseDecimalCell(row, "净重量");
                    decimal purity = TryParseDecimalCell(row, "成色");
                    decimal unitPrice = TryParseDecimalCell(row, "单价");
                    decimal discountedPrice = TryParseDecimalCell(row, "成色折价");
                    // 核心修改：单项总价转为整数（截断小数）
                    int totalPrice = (int)Math.Truncate(TryParseDecimalCell(row, "单项总价"));

                    if (itemId <= 0)
                        itemId = newItemList.Count + 1;

                    newItemList.Add(new OrderItem
                    {
                        Id = itemId,
                        ProductName = productName,
                        GrossWeight = grossWeight,
                        NetWeight = netWeight,
                        Purity = purity,
                        UnitPrice = unitPrice,
                        DiscountedPrice = discountedPrice,
                        TotalPrice = totalPrice
                    });
                }

                _currentSelectedOrder.Items = newItemList;
                _currentSelectedOrder.GrandTotal = newItemList.Sum(item => item.TotalPrice);
                SaveOrdersToFile();

                ForceReloadAllDataFromDisk();
                RefreshOrderListWithFilter();

                MessageBox.Show("修改成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteOrder_Click(object sender, EventArgs e)
        {
            if (_currentSelectedOrder == null)
            {
                MessageBox.Show("请选择要删除的单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"确定删除单据【{_currentSelectedOrder.Id}】？删除后无法恢复", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _orderList.Remove(_currentSelectedOrder);
                SaveOrdersToFile();
                ForceReloadAllDataFromDisk();
                RefreshOrderListWithFilter();
                MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoadToMain_Click(object sender, EventArgs e)
        {
            if (_currentSelectedOrder == null)
            {
                MessageBox.Show("请选择要调取的单据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("确定调取到主界面？当前未保存数据将清空", "确认调取", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _mainFormInstance?.LoadOrderToMainForm(_currentSelectedOrder);
            Close();
        }

        private void DgvOrderDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var currentRow = dgvOrderDetail.Rows[e.RowIndex];

            try
            {
                if (decimal.TryParse(currentRow.Cells["成色"].Value?.ToString(), out decimal purity)
                    && decimal.TryParse(currentRow.Cells["单价"].Value?.ToString(), out decimal unitPrice))
                {
                    decimal discountPrice = purity * unitPrice;
                    currentRow.Cells["成色折价"].Value = Math.Truncate(discountPrice * 10) / 10;
                }

                if (decimal.TryParse(currentRow.Cells["净重量"].Value?.ToString(), out decimal netWeight)
                    && decimal.TryParse(currentRow.Cells["成色折价"].Value?.ToString(), out decimal finalDiscountPrice))
                {
                    // 核心修改：单项总价计算后截断小数，赋值整数
                    decimal itemTotal = netWeight * finalDiscountPrice;
                    currentRow.Cells["单项总价"].Value = (int)Math.Truncate(itemTotal);
                }
            }
            catch { }
        }
        #endregion

        #region 辅助方法
        private decimal TryParseDecimalCell(DataGridViewRow row, string columnName)
        {
            if (row == null) return 0m;
            try
            {
                object cellVal = row.Cells[columnName].Value;
                if (cellVal == null) return 0m;
                if (decimal.TryParse(cellVal.ToString(), out decimal result))
                    return result;
            }
            catch { }
            return 0m;
        }

        private void SaveOrdersToFile()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(_orderList, Formatting.Indented);
                File.WriteAllText(OrdersFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}