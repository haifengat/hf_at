
namespace HaiFeng
{
	partial class FormWorkSpace
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWorkSpace));
			System.Windows.Forms.DataVisualization.Charting.ArrowAnnotation arrowAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.ArrowAnnotation();
			System.Windows.Forms.DataVisualization.Charting.LineAnnotation lineAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.LineAnnotation();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.430555555555D, "80,70,60,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.416666666664D, "80,70,60,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.409722222219D, "80,70,60,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.402777777781D, "80,70,60,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.395833333336D, "80,70,60,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.388888888891D, "80,70,60,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint7 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.381944444445D, "80,50,70,65");
			System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint8 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(41032.375D, "80,70,60,65");
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabelInstrument = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabelInterval = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabelDT = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabelH = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabelL = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabelO = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabelC = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabelUpdateTime = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonBlackBack = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 2000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(35, 22);
			this.toolStripButton2.Text = "放大";
			this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(35, 22);
			this.toolStripButton3.Text = "缩小";
			this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(35, 22);
			this.toolStripButton4.Text = "美线";
			this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabelInstrument
			// 
			this.toolStripLabelInstrument.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.toolStripLabelInstrument.Name = "toolStripLabelInstrument";
			this.toolStripLabelInstrument.Size = new System.Drawing.Size(31, 22);
			this.toolStripLabelInstrument.Text = "合约";
			// 
			// toolStripLabelInterval
			// 
			this.toolStripLabelInterval.ForeColor = System.Drawing.Color.Indigo;
			this.toolStripLabelInterval.Name = "toolStripLabelInterval";
			this.toolStripLabelInterval.Size = new System.Drawing.Size(31, 22);
			this.toolStripLabelInterval.Text = "周期";
			// 
			// toolStripLabelDT
			// 
			this.toolStripLabelDT.ForeColor = System.Drawing.Color.Purple;
			this.toolStripLabelDT.Name = "toolStripLabelDT";
			this.toolStripLabelDT.Size = new System.Drawing.Size(50, 22);
			this.toolStripLabelDT.Text = "K线时间";
			// 
			// toolStripLabelH
			// 
			this.toolStripLabelH.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripLabelH.Name = "toolStripLabelH";
			this.toolStripLabelH.Size = new System.Drawing.Size(19, 22);
			this.toolStripLabelH.Text = "高";
			// 
			// toolStripLabelL
			// 
			this.toolStripLabelL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripLabelL.Name = "toolStripLabelL";
			this.toolStripLabelL.Size = new System.Drawing.Size(19, 22);
			this.toolStripLabelL.Text = "低";
			// 
			// toolStripLabelO
			// 
			this.toolStripLabelO.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripLabelO.Name = "toolStripLabelO";
			this.toolStripLabelO.Size = new System.Drawing.Size(19, 22);
			this.toolStripLabelO.Text = "开";
			// 
			// toolStripLabelC
			// 
			this.toolStripLabelC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.toolStripLabelC.Name = "toolStripLabelC";
			this.toolStripLabelC.Size = new System.Drawing.Size(19, 22);
			this.toolStripLabelC.Text = "收";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabelUpdateTime
			// 
			this.toolStripLabelUpdateTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.toolStripLabelUpdateTime.Name = "toolStripLabelUpdateTime";
			this.toolStripLabelUpdateTime.Size = new System.Drawing.Size(50, 22);
			this.toolStripLabelUpdateTime.Text = "tick时间";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonBlackBack
			// 
			this.toolStripButtonBlackBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonBlackBack.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBlackBack.Image")));
			this.toolStripButtonBlackBack.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonBlackBack.Name = "toolStripButtonBlackBack";
			this.toolStripButtonBlackBack.Size = new System.Drawing.Size(59, 22);
			this.toolStripButtonBlackBack.Text = "黑色背景";
			this.toolStripButtonBlackBack.Click += new System.EventHandler(this.toolStripButtonBlackBack_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.Color.DarkGray;
			this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSeparator4,
            this.toolStripLabelInstrument,
            this.toolStripLabelInterval,
            this.toolStripLabelDT,
            this.toolStripLabelH,
            this.toolStripLabelL,
            this.toolStripLabelO,
            this.toolStripLabelC,
            this.toolStripSeparator2,
            this.toolStripLabelUpdateTime,
            this.toolStripSeparator3,
            this.toolStripButtonBlackBack});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(715, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// chart1
			// 
			arrowAnnotation1.AllowTextEditing = true;
			arrowAnnotation1.AnchorDataPointName = "Series1\\r2";
			arrowAnnotation1.AxisXName = "ChartArea1\\rX";
			arrowAnnotation1.ClipToChartArea = "ChartArea1";
			arrowAnnotation1.Height = -5D;
			arrowAnnotation1.Name = "ArrowAnnotation1";
			arrowAnnotation1.Width = 0D;
			arrowAnnotation1.Y = 70D;
			arrowAnnotation1.YAxisName = "ChartArea1\\rY";
			lineAnnotation1.AnchorDataPointName = "Series1\\r4";
			lineAnnotation1.ClipToChartArea = "ChartArea1";
			lineAnnotation1.Height = 10D;
			lineAnnotation1.IsSizeAlwaysRelative = false;
			lineAnnotation1.LineColor = System.Drawing.Color.White;
			lineAnnotation1.Name = "LineAnnotation1";
			lineAnnotation1.Width = 2D;
			lineAnnotation1.Y = 70D;
			this.chart1.Annotations.Add(arrowAnnotation1);
			this.chart1.Annotations.Add(lineAnnotation1);
			this.chart1.BackColor = System.Drawing.Color.Transparent;
			chartArea1.AlignmentOrientation = ((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations)((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Vertical | System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal)));
			chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea1.AxisX.IsStartedFromZero = false;
			chartArea1.AxisX.LabelStyle.Format = "HH:mm";
			chartArea1.AxisX.LabelStyle.Interval = 0D;
			chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea1.AxisX.MajorTickMark.Enabled = false;
			chartArea1.AxisX.MajorTickMark.Interval = 0D;
			chartArea1.AxisX.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisX.ScrollBar.ButtonStyle = System.Windows.Forms.DataVisualization.Charting.ScrollBarButtonStyles.SmallScroll;
			chartArea1.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea1.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisY.IsInterlaced = true;
			chartArea1.AxisY.IsStartedFromZero = false;
			chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
			chartArea1.AxisY.MajorTickMark.Enabled = false;
			chartArea1.AxisY.ScaleView.Zoomable = false;
			chartArea1.AxisY.ScrollBar.Enabled = false;
			chartArea1.BackColor = System.Drawing.Color.WhiteSmoke;
			chartArea1.BorderColor = System.Drawing.Color.Maroon;
			chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea1.IsSameFontSizeForAllAxes = true;
			chartArea1.Name = "ChartArea1";
			chartArea1.Position.Auto = false;
			chartArea1.Position.Height = 99F;
			chartArea1.Position.Width = 99F;
			chartArea1.Position.X = 1F;
			chartArea1.Position.Y = 1F;
			chartArea2.AlignWithChartArea = "ChartArea1";
			chartArea2.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea2.AxisX.IsStartedFromZero = false;
			chartArea2.AxisX.LabelStyle.Enabled = false;
			chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea2.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea2.AxisX.MajorTickMark.Enabled = false;
			chartArea2.AxisX.ScrollBar.Enabled = false;
			chartArea2.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea2.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea2.AxisY.IsStartedFromZero = false;
			chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea2.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea2.AxisY.MajorTickMark.Enabled = false;
			chartArea2.AxisY.ScrollBar.Enabled = false;
			chartArea2.BackColor = System.Drawing.Color.WhiteSmoke;
			chartArea2.BorderColor = System.Drawing.Color.Maroon;
			chartArea2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea2.IsSameFontSizeForAllAxes = true;
			chartArea2.Name = "ChartArea2";
			chartArea2.Position.Auto = false;
			chartArea2.Position.Height = 25F;
			chartArea2.Position.Width = 99F;
			chartArea2.Position.X = 1F;
			chartArea2.Position.Y = 75F;
			chartArea2.Visible = false;
			chartArea3.AlignWithChartArea = "ChartArea1";
			chartArea3.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea3.AxisX.IsStartedFromZero = false;
			chartArea3.AxisX.LabelStyle.Enabled = false;
			chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea3.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea3.AxisX.MajorTickMark.Enabled = false;
			chartArea3.AxisX.ScrollBar.Enabled = false;
			chartArea3.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea3.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea3.AxisY.IsStartedFromZero = false;
			chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea3.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea3.AxisY.MajorTickMark.Enabled = false;
			chartArea3.AxisY.ScrollBar.Enabled = false;
			chartArea3.BackColor = System.Drawing.Color.WhiteSmoke;
			chartArea3.BorderColor = System.Drawing.Color.Maroon;
			chartArea3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea3.IsSameFontSizeForAllAxes = true;
			chartArea3.Name = "ChartArea3";
			chartArea3.Position.Auto = false;
			chartArea3.Position.Height = 25F;
			chartArea3.Position.Width = 99F;
			chartArea3.Position.X = 1F;
			chartArea3.Position.Y = 50F;
			chartArea3.Visible = false;
			chartArea4.AlignWithChartArea = "ChartArea1";
			chartArea4.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea4.AxisX.IsStartedFromZero = false;
			chartArea4.AxisX.LabelStyle.Enabled = false;
			chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea4.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea4.AxisX.MajorTickMark.Enabled = false;
			chartArea4.AxisX.ScrollBar.Enabled = false;
			chartArea4.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea4.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea4.AxisY.IsStartedFromZero = false;
			chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
			chartArea4.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			chartArea4.AxisY.MajorTickMark.Enabled = false;
			chartArea4.AxisY.ScrollBar.Enabled = false;
			chartArea4.BackColor = System.Drawing.Color.WhiteSmoke;
			chartArea4.BorderColor = System.Drawing.Color.Maroon;
			chartArea4.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea4.IsSameFontSizeForAllAxes = true;
			chartArea4.Name = "ChartArea4";
			chartArea4.Position.Auto = false;
			chartArea4.Position.Height = 25F;
			chartArea4.Position.Width = 99F;
			chartArea4.Position.X = 1F;
			chartArea4.Position.Y = 75F;
			chartArea4.Visible = false;
			this.chart1.ChartAreas.Add(chartArea1);
			this.chart1.ChartAreas.Add(chartArea2);
			this.chart1.ChartAreas.Add(chartArea3);
			this.chart1.ChartAreas.Add(chartArea4);
			this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chart1.Location = new System.Drawing.Point(0, 25);
			this.chart1.Margin = new System.Windows.Forms.Padding(0);
			this.chart1.Name = "chart1";
			this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
			series1.BorderColor = System.Drawing.Color.DimGray;
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
			series1.Color = System.Drawing.Color.SeaGreen;
			series1.CustomProperties = "PriceDownColor=SeaGreen, PriceUpColor=Maroon";
			series1.IsXValueIndexed = true;
			series1.Name = "Series1";
			series1.Points.Add(dataPoint1);
			series1.Points.Add(dataPoint2);
			series1.Points.Add(dataPoint3);
			series1.Points.Add(dataPoint4);
			series1.Points.Add(dataPoint5);
			series1.Points.Add(dataPoint6);
			series1.Points.Add(dataPoint7);
			series1.Points.Add(dataPoint8);
			series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
			series1.YValuesPerPoint = 4;
			series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			this.chart1.Series.Add(series1);
			this.chart1.Size = new System.Drawing.Size(715, 364);
			this.chart1.TabIndex = 3;
			this.chart1.Text = "chart1";
			this.chart1.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.chart1_GetToolTipText);
			// 
			// FormWorkSpace
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DarkGray;
			this.ClientSize = new System.Drawing.Size(715, 389);
			this.Controls.Add(this.chart1);
			this.Controls.Add(this.toolStrip1);
			this.DoubleBuffered = true;
			this.Name = "FormWorkSpace";
			this.Text = "工作区";
			this.TransparencyKey = System.Drawing.Color.Transparent;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormWorkSpace_FormClosed);
			this.Load += new System.EventHandler(this.FormWorkSpace_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripLabel toolStripLabelInstrument;
		private System.Windows.Forms.ToolStripLabel toolStripLabelInterval;
		private System.Windows.Forms.ToolStripLabel toolStripLabelDT;
		private System.Windows.Forms.ToolStripLabel toolStripLabelH;
		private System.Windows.Forms.ToolStripLabel toolStripLabelL;
		private System.Windows.Forms.ToolStripLabel toolStripLabelO;
		private System.Windows.Forms.ToolStripLabel toolStripLabelC;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripLabel toolStripLabelUpdateTime;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton toolStripButtonBlackBack;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
	}
}