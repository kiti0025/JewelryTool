using System;
using System.Windows.Forms;
using System.Drawing;

namespace JewelryTool
{
    partial class PrintSettingForm
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

        #region Windows 窗体设计器生成的代码
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cboPaperPresets = new System.Windows.Forms.ComboBox();
            this.btnSavePreset = new System.Windows.Forms.Button();
            this.btnDeletePreset = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPaperName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPaperWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPaperHeight = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radLandscape = new System.Windows.Forms.RadioButton();
            this.radPortrait = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numMarginRight = new System.Windows.Forms.NumericUpDown();
            this.numMarginLeft = new System.Windows.Forms.NumericUpDown();
            this.numMarginBottom = new System.Windows.Forms.NumericUpDown();
            this.numMarginTop = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).BeginInit();
            this.SuspendLayout();

            // 窗体基础设置（高度增加以容纳新布局）
            this.AutoScaleDimensions = new SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new Size(480, 440);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "打印设置";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("微软雅黑", 9F);

            // label1（纸张预设）
            this.label1.AutoSize = true;
            this.label1.Location = new Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new Size(67, 15);
            this.label1.Text = "纸张预设：";

            // cboPaperPresets（纸张预设下拉）
            this.cboPaperPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPaperPresets.Location = new Point(90, 17);
            this.cboPaperPresets.Name = "cboPaperPresets";
            this.cboPaperPresets.Size = new Size(220, 23);
            this.cboPaperPresets.SelectedIndexChanged += new System.EventHandler(this.cboPaperPresets_SelectedIndexChanged);

            // ✅ 保存预设按钮（移到纸张名称下一行）
            this.btnSavePreset.Location = new Point(90, 90);
            this.btnSavePreset.Name = "btnSavePreset";
            this.btnSavePreset.Size = new Size(100, 25);
            this.btnSavePreset.Text = "保存预设";
            this.btnSavePreset.UseVisualStyleBackColor = true;
            this.btnSavePreset.Click += new System.EventHandler(this.btnSavePreset_Click);

            // ✅ 删除预设按钮（和保存预设同排）
            this.btnDeletePreset.Location = new Point(200, 90);
            this.btnDeletePreset.Name = "btnDeletePreset";
            this.btnDeletePreset.Size = new Size(100, 25);
            this.btnDeletePreset.Text = "删除预设";
            this.btnDeletePreset.UseVisualStyleBackColor = true;
            this.btnDeletePreset.Click += new System.EventHandler(this.btnDeletePreset_Click);

            // label8（纸张名称）
            this.label8.AutoSize = true;
            this.label8.Location = new Point(20, 60);
            this.label8.Name = "label8";
            this.label8.Size = new Size(60, 15);
            this.label8.Text = "纸张名称：";

            // txtPaperName（纸张名称输入框）
            this.txtPaperName.Location = new Point(90, 57);
            this.txtPaperName.Name = "txtPaperName";
            this.txtPaperName.Size = new Size(125, 23);
            this.txtPaperName.Text = "输入自定义名称";

            // label2（宽度）
            this.label2.AutoSize = true;
            this.label2.Location = new Point(220, 60);
            this.label2.Name = "label2";
            this.label2.Size = new Size(59, 15);
            this.label2.Text = "宽度:";

            // txtPaperWidth（宽度输入框）
            this.txtPaperWidth.Location = new Point(260, 57);
            this.txtPaperWidth.Name = "txtPaperWidth";
            this.txtPaperWidth.Size = new Size(60, 23);

            // label3（高度）
            this.label3.AutoSize = true;
            this.label3.Location = new Point(345, 60);
            this.label3.Name = "label3";
            this.label3.Size = new Size(59, 15);
            this.label3.Text = "高度:";

            // txtPaperHeight（高度输入框）
            this.txtPaperHeight.Location = new Point(385, 57);
            this.txtPaperHeight.Name = "txtPaperHeight";
            this.txtPaperHeight.Size = new Size(60, 23);

            // groupBox1（打印方向，下移避免和按钮重叠）
            this.groupBox1.Controls.Add(this.radLandscape);
            this.groupBox1.Controls.Add(this.radPortrait);
            this.groupBox1.Location = new Point(20, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(440, 70);
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "打印方向";

            // radPortrait（纵向）
            this.radPortrait.AutoSize = true;
            this.radPortrait.Checked = true;
            this.radPortrait.Location = new Point(30, 30);
            this.radPortrait.Name = "radPortrait";
            this.radPortrait.Size = new Size(47, 19);
            this.radPortrait.TabStop = true;
            this.radPortrait.Text = "纵向";
            this.radPortrait.UseVisualStyleBackColor = true;

            // radLandscape（横向）
            this.radLandscape.AutoSize = true;
            this.radLandscape.Location = new Point(150, 30);
            this.radLandscape.Name = "radLandscape";
            this.radLandscape.Size = new Size(47, 19);
            this.radLandscape.Text = "横向";
            this.radLandscape.UseVisualStyleBackColor = true;

            // groupBox2（页边距，同步下移）
            this.groupBox2.Controls.Add(this.numMarginRight);
            this.groupBox2.Controls.Add(this.numMarginLeft);
            this.groupBox2.Controls.Add(this.numMarginBottom);
            this.groupBox2.Controls.Add(this.numMarginTop);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new Point(20, 220);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(440, 100);
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "页边距(mm)";

            // numMarginTop（上边距）
            this.numMarginTop.Location = new Point(50, 30);
            this.numMarginTop.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMarginTop.Name = "numMarginTop";
            this.numMarginTop.Size = new Size(60, 23);
            this.numMarginTop.Value = new decimal(new int[] { 20, 0, 0, 0 });

            // label4（上）
            this.label4.AutoSize = true;
            this.label4.Location = new Point(20, 32);
            this.label4.Name = "label4";
            this.label4.Size = new Size(23, 15);
            this.label4.Text = "上";

            // numMarginBottom（下边距）
            this.numMarginBottom.Location = new Point(50, 65);
            this.numMarginBottom.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMarginBottom.Name = "numMarginBottom";
            this.numMarginBottom.Size = new Size(60, 23);
            this.numMarginBottom.Value = new decimal(new int[] { 20, 0, 0, 0 });

            // label5（下）
            this.label5.AutoSize = true;
            this.label5.Location = new Point(20, 67);
            this.label5.Name = "label5";
            this.label5.Size = new Size(23, 15);
            this.label5.Text = "下";

            // numMarginLeft（左边距）
            this.numMarginLeft.Location = new Point(180, 30);
            this.numMarginLeft.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMarginLeft.Name = "numMarginLeft";
            this.numMarginLeft.Size = new Size(60, 23);
            this.numMarginLeft.Value = new decimal(new int[] { 20, 0, 0, 0 });

            // label6（左）
            this.label6.AutoSize = true;
            this.label6.Location = new Point(150, 32);
            this.label6.Name = "label6";
            this.label6.Size = new Size(23, 15);
            this.label6.Text = "左";

            // numMarginRight（右边距）
            this.numMarginRight.Location = new Point(180, 65);
            this.numMarginRight.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numMarginRight.Name = "numMarginRight";
            this.numMarginRight.Size = new Size(60, 23);
            this.numMarginRight.Value = new decimal(new int[] { 20, 0, 0, 0 });

            // label7（右）
            this.label7.AutoSize = true;
            this.label7.Location = new Point(150, 67);
            this.label7.Name = "label7";
            this.label7.Size = new Size(23, 15);
            this.label7.Text = "右";

            // btnConfirm（确定，同步下移）
            this.btnConfirm.Location = new Point(250, 380);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new Size(90, 30);
            this.btnConfirm.Text = "确定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);

            // btnCancel（取消，同步下移）
            this.btnCancel.Location = new Point(360, 380);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(90, 30);
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // 控件添加到窗体
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboPaperPresets);
            this.Controls.Add(this.btnSavePreset);
            this.Controls.Add(this.btnDeletePreset);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtPaperName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPaperWidth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPaperHeight);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);

            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private Label label1;
        private ComboBox cboPaperPresets;
        private Button btnSavePreset;
        private Button btnDeletePreset;
        private Label label8;
        private TextBox txtPaperName;
        private Label label2;
        private TextBox txtPaperWidth;
        private Label label3;
        private TextBox txtPaperHeight;
        private GroupBox groupBox1;
        private RadioButton radLandscape;
        private RadioButton radPortrait;
        private GroupBox groupBox2;
        private NumericUpDown numMarginRight;
        private NumericUpDown numMarginLeft;
        private NumericUpDown numMarginBottom;
        private NumericUpDown numMarginTop;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Button btnConfirm;
        private Button btnCancel;
    }
}