using System.Drawing;

namespace JewelryTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.cbZoom = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnHistory = new System.Windows.Forms.Button();
            this.btnDeleteProduct = new System.Windows.Forms.Button();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.btnDeleteCustomer = new System.Windows.Forms.Button();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddCustomer = new System.Windows.Forms.Button();
            this.cbCustomer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProduct = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colGrossWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNetWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPurity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiscountedPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.btnPrintSetting = new System.Windows.Forms.Button();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnStats = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();

            // topPanel
            this.topPanel.Controls.Add(this.cbZoom);
            this.topPanel.Controls.Add(this.label5);
            this.topPanel.Controls.Add(this.btnHistory);
            this.topPanel.Controls.Add(this.btnDeleteProduct);
            this.topPanel.Controls.Add(this.btnAddProduct);
            this.topPanel.Controls.Add(this.btnDeleteCustomer);
            this.topPanel.Controls.Add(this.cbType);
            this.topPanel.Controls.Add(this.label3);
            this.topPanel.Controls.Add(this.dtpDate);
            this.topPanel.Controls.Add(this.label2);
            this.topPanel.Controls.Add(this.btnAddCustomer);
            this.topPanel.Controls.Add(this.cbCustomer);
            this.topPanel.Controls.Add(this.label1);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new Size(1280, 60);
            this.topPanel.TabIndex = 0;

            // label1 - 客户
            this.label1.AutoSize = true;
            this.label1.Location = new Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new Size(44, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "客户：";

            // cbCustomer - 客户下拉
            this.cbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomer.FormattingEnabled = true;
            this.cbCustomer.Location = new Point(65, 12);
            this.cbCustomer.Name = "cbCustomer";
            this.cbCustomer.Size = new Size(160, 25);
            this.cbCustomer.TabIndex = 1;

            // btnDeleteCustomer - 删除客户
            this.btnDeleteCustomer.Location = new Point(235, 10);
            this.btnDeleteCustomer.Name = "btnDeleteCustomer";
            this.btnDeleteCustomer.Size = new Size(80, 28);
            this.btnDeleteCustomer.TabIndex = 2;
            this.btnDeleteCustomer.Text = "删除客户";
            this.btnDeleteCustomer.UseVisualStyleBackColor = true;
            this.btnDeleteCustomer.ForeColor = System.Drawing.Color.Red;

            // btnAddCustomer - 新增客户
            this.btnAddCustomer.Location = new Point(320, 10);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new Size(80, 28);
            this.btnAddCustomer.TabIndex = 3;
            this.btnAddCustomer.Text = "新增客户";
            this.btnAddCustomer.UseVisualStyleBackColor = true;

            // label2 - 日期
            this.label2.AutoSize = true;
            this.label2.Location = new Point(410, 15);
            this.label2.Name = "label2";
            this.label2.Size = new Size(44, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "日期：";

            // dtpDate - 日期选择器
            this.dtpDate.Location = new Point(460, 12);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new Size(180, 25);
            this.dtpDate.TabIndex = 5;

            // label3 - 单据类型
            this.label3.AutoSize = true;
            this.label3.Location = new Point(650, 15);
            this.label3.Name = "label3";
            this.label3.Size = new Size(68, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "单据类型：";

            // cbType - 单据类型
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "入库",
            "出库"});
            this.cbType.Location = new Point(725, 12);
            this.cbType.Name = "cbType";
            this.cbType.Size = new Size(100, 25);
            this.cbType.TabIndex = 7;

            // btnDeleteProduct - 删除品类
            this.btnDeleteProduct.Location = new Point(835, 10);
            this.btnDeleteProduct.Name = "btnDeleteProduct";
            this.btnDeleteProduct.Size = new Size(80, 28);
            this.btnDeleteProduct.TabIndex = 8;
            this.btnDeleteProduct.Text = "删除品类";
            this.btnDeleteProduct.UseVisualStyleBackColor = true;
            this.btnDeleteProduct.ForeColor = System.Drawing.Color.Red;

            // btnAddProduct - 新增品类
            this.btnAddProduct.Location = new Point(920, 10);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new Size(80, 28);
            this.btnAddProduct.TabIndex = 9;
            this.btnAddProduct.Text = "新增品类";
            this.btnAddProduct.UseVisualStyleBackColor = true;

            // btnHistory - 历史单据
            this.btnHistory.BackColor = System.Drawing.Color.LightCyan;
            this.btnHistory.Location = new Point(1005, 10);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new Size(100, 28);
            this.btnHistory.TabIndex = 10;
            this.btnHistory.Text = "历史单据";
            this.btnHistory.UseVisualStyleBackColor = false;

            // label5 - 缩放
            this.label5.AutoSize = true;
            this.label5.Location = new Point(1115, 15);
            this.label5.Name = "label5";
            this.label5.Size = new Size(56, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "缩放：";

            // cbZoom - 缩放下拉
            this.cbZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbZoom.FormattingEnabled = true;
            this.cbZoom.Items.AddRange(new object[] {
            "50%",
            "75%",
            "100%",
            "125%",
            "150%",
            "175%",
            "200%"});
            this.cbZoom.Location = new Point(1175, 12);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new Size(80, 25);
            this.cbZoom.TabIndex = 12;
            this.cbZoom.Text = "100%";

            // dgvItems
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colProduct,
            this.colGrossWeight,
            this.colNetWeight,
            this.colPurity,
            this.colUnitPrice,
            this.colDiscountedPrice,
            this.colTotalPrice});
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new Point(0, 60);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new Size(1280, 511);
            this.dgvItems.TabIndex = 1;

            // colIndex
            this.colIndex.HeaderText = "序号";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Width = 60;

            // colProduct
            this.colProduct.HeaderText = "品名";
            this.colProduct.Name = "colProduct";
            this.colProduct.Width = 100;

            // colGrossWeight
            this.colGrossWeight.HeaderText = "毛重量";
            this.colGrossWeight.Name = "colGrossWeight";
            this.colGrossWeight.Width = 90;

            // colNetWeight
            this.colNetWeight.HeaderText = "净重量";
            this.colNetWeight.Name = "colNetWeight";
            this.colNetWeight.Width = 90;

            // colPurity
            this.colPurity.HeaderText = "成色";
            this.colPurity.Name = "colPurity";
            this.colPurity.Width = 70;

            // colUnitPrice
            this.colUnitPrice.HeaderText = "单价";
            this.colUnitPrice.Name = "colUnitPrice";
            this.colUnitPrice.Width = 90;

            // colDiscountedPrice
            this.colDiscountedPrice.HeaderText = "成色折价";
            this.colDiscountedPrice.Name = "colDiscountedPrice";
            this.colDiscountedPrice.ReadOnly = true;
            this.colDiscountedPrice.Width = 100;

            // colTotalPrice
            this.colTotalPrice.HeaderText = "单项总价";
            this.colTotalPrice.Name = "colTotalPrice";
            this.colTotalPrice.ReadOnly = true;
            this.colTotalPrice.Width = 120;

            // bottomPanel
            this.bottomPanel.Controls.Add(this.btnPrintSetting);
            this.bottomPanel.Controls.Add(this.txtRemarks);
            this.bottomPanel.Controls.Add(this.label4);
            this.bottomPanel.Controls.Add(this.lblTotal);
            this.bottomPanel.Controls.Add(this.btnStats);
            this.bottomPanel.Controls.Add(this.btnPrint);
            this.bottomPanel.Controls.Add(this.btnExportExcel);
            this.bottomPanel.Controls.Add(this.btnSave);
            this.bottomPanel.Controls.Add(this.btnDeleteRow);
            this.bottomPanel.Controls.Add(this.btnAddRow);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new Point(0, 571);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new Size(1280, 120);
            this.bottomPanel.TabIndex = 2;

            // btnPrintSetting
            this.btnPrintSetting.Location = new Point(590, 10);
            this.btnPrintSetting.Name = "btnPrintSetting";
            this.btnPrintSetting.Size = new Size(100, 35);
            this.btnPrintSetting.TabIndex = 9;
            this.btnPrintSetting.Text = "打印设置";
            this.btnPrintSetting.UseVisualStyleBackColor = true;

            // txtRemarks
            this.txtRemarks.Location = new Point(70, 87);
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new Size(800, 25);
            this.txtRemarks.TabIndex = 8;

            // label4
            this.label4.AutoSize = true;
            this.label4.Location = new Point(15, 90);
            this.label4.Name = "label4";
            this.label4.Size = new Size(44, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "备注：";

            // lblTotal
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.DarkRed;
            this.lblTotal.Location = new Point(15, 55);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new Size(147, 24);
            this.lblTotal.TabIndex = 6;
            this.lblTotal.Text = "单据总价：0.00 元";

            // btnStats
            this.btnStats.BackColor = System.Drawing.Color.LightBlue;
            this.btnStats.Location = new Point(950, 10);
            this.btnStats.Name = "btnStats";
            this.btnStats.Size = new Size(100, 35);
            this.btnStats.TabIndex = 5;
            this.btnStats.Text = "统计中心";
            this.btnStats.UseVisualStyleBackColor = false;

            // btnPrint
            this.btnPrint.Location = new Point(700, 10);
            this.btnPrint.Location = new Point(700, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new Size(110, 35);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "打印兑料单";
            this.btnPrint.UseVisualStyleBackColor = true;

            // btnExportExcel
            this.btnExportExcel.Location = new Point(355, 10);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new Size(100, 35);
            this.btnExportExcel.TabIndex = 3;
            this.btnExportExcel.Text = "导出Excel";
            this.btnExportExcel.UseVisualStyleBackColor = true;

            // btnSave
            this.btnSave.BackColor = System.Drawing.Color.LightGreen;
            this.btnSave.Location = new Point(240, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存单据";
            this.btnSave.UseVisualStyleBackColor = false;

            // btnDeleteRow
            this.btnDeleteRow.Location = new Point(115, 10);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new Size(110, 35);
            this.btnDeleteRow.TabIndex = 1;
            this.btnDeleteRow.Text = "删除选中行";
            this.btnDeleteRow.UseVisualStyleBackColor = true;

            // btnAddRow
            this.btnAddRow.Location = new Point(15, 10);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new Size(90, 35);
            this.btnAddRow.TabIndex = 0;
            this.btnAddRow.Text = "新增行";
            this.btnAddRow.UseVisualStyleBackColor = true;

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new Size(1280, 691);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "瑞华珠宝兑料单系统";
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnDeleteProduct;
        private System.Windows.Forms.Button btnDeleteCustomer;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddCustomer;
        private System.Windows.Forms.ComboBox cbCustomer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewComboBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGrossWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNetWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPurity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiscountedPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalPrice;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnStats;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.ComboBox cbZoom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnPrintSetting;
    }
}