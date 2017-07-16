
namespace HaiFeng
{
	partial class FormTest
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.dataGridViewdReport = new System.Windows.Forms.DataGridView();
			this.统计指标 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.全部交易 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.多头 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.空头 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.dataGridViewDetail = new System.Windows.Forms.DataGridView();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.chartDB1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonZoomIn = new System.Windows.Forms.Button();
			this.buttonZoonOut = new System.Windows.Forms.Button();
			this.numericUpDownMALen = new System.Windows.Forms.NumericUpDown();
			this.checkBoxMA = new System.Windows.Forms.CheckBox();
			this.comboBoxChartItem = new System.Windows.Forms.ComboBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.dataGridViewdRate = new System.Windows.Forms.DataGridView();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripComboBoxStrategy = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tabControl1.SuspendLayout();
			this.tabPage4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewdReport)).BeginInit();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetail)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartDB1)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMALen)).BeginInit();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewdRate)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(0, 28);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(616, 542);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.dataGridViewdReport);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(608, 516);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "综合报告";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// dataGridViewdReport
			// 
			this.dataGridViewdReport.AllowUserToAddRows = false;
			this.dataGridViewdReport.AllowUserToDeleteRows = false;
			this.dataGridViewdReport.AllowUserToOrderColumns = true;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.dataGridViewdReport.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridViewdReport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewdReport.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
			this.dataGridViewdReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewdReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.统计指标,
            this.全部交易,
            this.多头,
            this.空头});
			this.dataGridViewdReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewdReport.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewdReport.Name = "dataGridViewdReport";
			this.dataGridViewdReport.ReadOnly = true;
			this.dataGridViewdReport.RowHeadersWidth = 6;
			this.dataGridViewdReport.RowTemplate.Height = 23;
			this.dataGridViewdReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewdReport.ShowCellToolTips = false;
			this.dataGridViewdReport.Size = new System.Drawing.Size(602, 510);
			this.dataGridViewdReport.TabIndex = 0;
			// 
			// 统计指标
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.统计指标.DefaultCellStyle = dataGridViewCellStyle2;
			this.统计指标.HeaderText = "统计指标";
			this.统计指标.Name = "统计指标";
			this.统计指标.ReadOnly = true;
			// 
			// 全部交易
			// 
			dataGridViewCellStyle3.Format = "N2";
			dataGridViewCellStyle3.NullValue = "-";
			this.全部交易.DefaultCellStyle = dataGridViewCellStyle3;
			this.全部交易.HeaderText = "全部交易";
			this.全部交易.Name = "全部交易";
			this.全部交易.ReadOnly = true;
			// 
			// 多头
			// 
			dataGridViewCellStyle4.Format = "N2";
			dataGridViewCellStyle4.NullValue = "-";
			this.多头.DefaultCellStyle = dataGridViewCellStyle4;
			this.多头.HeaderText = "多头";
			this.多头.Name = "多头";
			this.多头.ReadOnly = true;
			// 
			// 空头
			// 
			dataGridViewCellStyle5.Format = "N2";
			dataGridViewCellStyle5.NullValue = "-";
			this.空头.DefaultCellStyle = dataGridViewCellStyle5;
			this.空头.HeaderText = "空头";
			this.空头.Name = "空头";
			this.空头.ReadOnly = true;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.dataGridViewDetail);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(608, 516);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "交易记录";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// dataGridViewDetail
			// 
			this.dataGridViewDetail.AllowUserToAddRows = false;
			this.dataGridViewDetail.AllowUserToDeleteRows = false;
			this.dataGridViewDetail.AllowUserToOrderColumns = true;
			dataGridViewCellStyle6.BackColor = System.Drawing.Color.WhiteSmoke;
			this.dataGridViewDetail.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
			this.dataGridViewDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dataGridViewDetail.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
			this.dataGridViewDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewDetail.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewDetail.Name = "dataGridViewDetail";
			this.dataGridViewDetail.ReadOnly = true;
			this.dataGridViewDetail.RowHeadersWidth = 6;
			this.dataGridViewDetail.RowTemplate.Height = 23;
			this.dataGridViewDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewDetail.ShowCellToolTips = false;
			this.dataGridViewDetail.Size = new System.Drawing.Size(602, 510);
			this.dataGridViewDetail.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.chartDB1);
			this.tabPage2.Controls.Add(this.panel1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(608, 516);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "分析图表";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// chartDB1
			// 
			this.chartDB1.BackColor = System.Drawing.Color.Transparent;
			chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea1.AxisX.IsStartedFromZero = false;
			chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightCyan;
			chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea1.AxisX.MajorTickMark.Enabled = false;
			chartArea1.AxisY.IsInterlaced = true;
			chartArea1.AxisY.IsStartedFromZero = false;
			chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkRed;
			chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
			chartArea1.AxisY.ScaleView.Zoomable = false;
			chartArea1.AxisY.ScrollBar.Enabled = false;
			chartArea1.BackColor = System.Drawing.Color.DarkGray;
			chartArea1.Name = "ChartArea1";
			this.chartDB1.ChartAreas.Add(chartArea1);
			this.chartDB1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartDB1.Location = new System.Drawing.Point(3, 26);
			this.chartDB1.Name = "chartDB1";
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.Color = System.Drawing.Color.Red;
			series1.IsXValueIndexed = true;
			series1.LabelToolTip = "#VALX #VAL{N2}";
			series1.Legend = "Legend1";
			series1.MarkerBorderColor = System.Drawing.Color.Red;
			series1.MarkerColor = System.Drawing.Color.White;
			series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
			series1.Name = "0";
			series1.ToolTip = "#VALX #VAL{N2}";
			series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
			this.chartDB1.Series.Add(series1);
			this.chartDB1.Size = new System.Drawing.Size(602, 487);
			this.chartDB1.TabIndex = 2;
			this.chartDB1.Text = "chartDB1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonZoomIn);
			this.panel1.Controls.Add(this.buttonZoonOut);
			this.panel1.Controls.Add(this.numericUpDownMALen);
			this.panel1.Controls.Add(this.checkBoxMA);
			this.panel1.Controls.Add(this.comboBoxChartItem);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(602, 23);
			this.panel1.TabIndex = 1;
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonZoomIn.Location = new System.Drawing.Point(346, 0);
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(75, 23);
			this.buttonZoomIn.TabIndex = 3;
			this.buttonZoomIn.Text = "缩小";
			this.buttonZoomIn.UseVisualStyleBackColor = true;
			this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
			// 
			// buttonZoonOut
			// 
			this.buttonZoonOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonZoonOut.Location = new System.Drawing.Point(265, 0);
			this.buttonZoonOut.Name = "buttonZoonOut";
			this.buttonZoonOut.Size = new System.Drawing.Size(75, 23);
			this.buttonZoonOut.TabIndex = 3;
			this.buttonZoonOut.Text = "放大";
			this.buttonZoonOut.UseVisualStyleBackColor = true;
			this.buttonZoonOut.Click += new System.EventHandler(this.buttonZoonOut_Click);
			// 
			// numericUpDownMALen
			// 
			this.numericUpDownMALen.Enabled = false;
			this.numericUpDownMALen.Location = new System.Drawing.Point(178, -1);
			this.numericUpDownMALen.Name = "numericUpDownMALen";
			this.numericUpDownMALen.Size = new System.Drawing.Size(53, 21);
			this.numericUpDownMALen.TabIndex = 2;
			this.numericUpDownMALen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDownMALen.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericUpDownMALen.ValueChanged += new System.EventHandler(this.numericUpDownMALen_ValueChanged);
			// 
			// checkBoxMA
			// 
			this.checkBoxMA.AutoSize = true;
			this.checkBoxMA.Location = new System.Drawing.Point(141, 3);
			this.checkBoxMA.Name = "checkBoxMA";
			this.checkBoxMA.Size = new System.Drawing.Size(36, 16);
			this.checkBoxMA.TabIndex = 1;
			this.checkBoxMA.Text = "MA";
			this.checkBoxMA.UseVisualStyleBackColor = true;
			this.checkBoxMA.CheckedChanged += new System.EventHandler(this.checkBoxMA_CheckedChanged);
			// 
			// comboBoxChartItem
			// 
			this.comboBoxChartItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChartItem.FormattingEnabled = true;
			this.comboBoxChartItem.Items.AddRange(new object[] {
            "平仓权益",
            "动态权益",
            "月度盈亏",
            "盈亏分布"});
			this.comboBoxChartItem.Location = new System.Drawing.Point(0, 0);
			this.comboBoxChartItem.Name = "comboBoxChartItem";
			this.comboBoxChartItem.Size = new System.Drawing.Size(121, 20);
			this.comboBoxChartItem.TabIndex = 0;
			this.comboBoxChartItem.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.dataGridViewdRate);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(608, 516);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "费率设置";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// dataGridViewdRate
			// 
			this.dataGridViewdRate.AllowUserToAddRows = false;
			this.dataGridViewdRate.AllowUserToDeleteRows = false;
			this.dataGridViewdRate.AllowUserToOrderColumns = true;
			dataGridViewCellStyle7.BackColor = System.Drawing.Color.WhiteSmoke;
			this.dataGridViewdRate.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
			this.dataGridViewdRate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dataGridViewdRate.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
			this.dataGridViewdRate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewdRate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewdRate.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewdRate.Name = "dataGridViewdRate";
			this.dataGridViewdRate.ReadOnly = true;
			this.dataGridViewdRate.RowHeadersWidth = 6;
			this.dataGridViewdRate.RowTemplate.Height = 23;
			this.dataGridViewdRate.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewdRate.ShowCellToolTips = false;
			this.dataGridViewdRate.Size = new System.Drawing.Size(602, 510);
			this.dataGridViewdRate.TabIndex = 0;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBoxStrategy,
            this.toolStripSeparator1});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(616, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(31, 22);
			this.toolStripLabel1.Text = "策略";
			// 
			// toolStripComboBoxStrategy
			// 
			this.toolStripComboBoxStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBoxStrategy.Name = "toolStripComboBoxStrategy";
			this.toolStripComboBoxStrategy.Size = new System.Drawing.Size(200, 25);
			this.toolStripComboBoxStrategy.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// FormTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(616, 570);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.tabControl1);
			this.DoubleBuffered = true;
			this.Name = "FormTest";
			this.Text = "测试报告";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTest_FormClosed);
			this.Load += new System.EventHandler(this.FormTest_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewdReport)).EndInit();
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetail)).EndInit();
			this.tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartDB1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMALen)).EndInit();
			this.tabPage3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewdRate)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox comboBoxChartItem;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBoxStrategy;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.DataGridView dataGridViewDetail;
		private System.Windows.Forms.DataGridView dataGridViewdRate;
		private System.Windows.Forms.CheckBox checkBoxMA;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.DataGridView dataGridViewdReport;
		private System.Windows.Forms.DataGridViewTextBoxColumn 统计指标;
		private System.Windows.Forms.DataGridViewTextBoxColumn 全部交易;
		private System.Windows.Forms.DataGridViewTextBoxColumn 多头;
		private System.Windows.Forms.DataGridViewTextBoxColumn 空头;
		private System.Windows.Forms.NumericUpDown numericUpDownMALen;
		private System.Windows.Forms.Button buttonZoomIn;
		private System.Windows.Forms.Button buttonZoonOut;
		private System.Windows.Forms.DataVisualization.Charting.Chart chartDB1;
	}
}