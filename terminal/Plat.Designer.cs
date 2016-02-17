namespace HaiFeng
{
	partial class Plat
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageDB = new System.Windows.Forms.TabPage();
			this.textBoxPwd = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBoxServer = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPageTxt = new System.Windows.Forms.TabPage();
			this.textBoxDataSourceReal = new System.Windows.Forms.TextBox();
			this.textBoxDataSourceTick = new System.Windows.Forms.TextBox();
			this.TextBoxDataSourceK = new System.Windows.Forms.TextBox();
			this.buttonDataSourceReal = new System.Windows.Forms.Button();
			this.buttonDataSourceTick = new System.Windows.Forms.Button();
			this.ButtonDataSource = new System.Windows.Forms.Button();
			this.comboBoxStrategyFile = new System.Windows.Forms.ComboBox();
			this.buttonStrategyFile = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonAddStra = new System.Windows.Forms.Button();
			this.buttonDel = new System.Windows.Forms.Button();
			this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
			this.dateTimePickerBegin = new System.Windows.Forms.DateTimePicker();
			this.groupBoxLoad = new System.Windows.Forms.GroupBox();
			this.radioButtonK = new System.Windows.Forms.RadioButton();
			this.radioButtonT = new System.Windows.Forms.RadioButton();
			this.buttonLoadStra = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBoxInterval = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxInst = new System.Windows.Forms.ComboBox();
			this.ComboBoxType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.DataGridViewStrategies = new System.Windows.Forms.DataGridView();
			this.StraName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Param = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Instrument = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Interval = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BeginDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Loaded = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Order = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.UpdateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ExcStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.report = new System.Windows.Forms.DataGridViewButtonColumn();
			this.Graphics = new System.Windows.Forms.DataGridViewButtonColumn();
			this.DataGridViewOrders = new System.Windows.Forms.DataGridView();
			this.panel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageDB.SuspendLayout();
			this.tabPageTxt.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBoxLoad.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DataGridViewStrategies)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DataGridViewOrders)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tabControl1);
			this.panel1.Controls.Add(this.comboBoxStrategyFile);
			this.panel1.Controls.Add(this.buttonStrategyFile);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1125, 60);
			this.panel1.TabIndex = 29;
			// 
			// tabControl1
			// 
			this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Right;
			this.tabControl1.Controls.Add(this.tabPageDB);
			this.tabControl1.Controls.Add(this.tabPageTxt);
			this.tabControl1.Location = new System.Drawing.Point(10, 1);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(965, 30);
			this.tabControl1.TabIndex = 29;
			// 
			// tabPageDB
			// 
			this.tabPageDB.Controls.Add(this.textBoxPwd);
			this.tabPageDB.Controls.Add(this.label8);
			this.tabPageDB.Controls.Add(this.textBoxUser);
			this.tabPageDB.Controls.Add(this.label7);
			this.tabPageDB.Controls.Add(this.textBoxServer);
			this.tabPageDB.Controls.Add(this.label1);
			this.tabPageDB.Location = new System.Drawing.Point(4, 4);
			this.tabPageDB.Name = "tabPageDB";
			this.tabPageDB.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDB.Size = new System.Drawing.Size(921, 22);
			this.tabPageDB.TabIndex = 1;
			this.tabPageDB.Text = "Data";
			this.tabPageDB.UseVisualStyleBackColor = true;
			// 
			// textBoxPwd
			// 
			this.textBoxPwd.Location = new System.Drawing.Point(474, -1);
			this.textBoxPwd.Name = "textBoxPwd";
			this.textBoxPwd.PasswordChar = '*';
			this.textBoxPwd.Size = new System.Drawing.Size(120, 21);
			this.textBoxPwd.TabIndex = 43;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(430, 4);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(29, 12);
			this.label8.TabIndex = 42;
			this.label8.Text = "密码";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxUser
			// 
			this.textBoxUser.Location = new System.Drawing.Point(301, -1);
			this.textBoxUser.Name = "textBoxUser";
			this.textBoxUser.Size = new System.Drawing.Size(120, 21);
			this.textBoxUser.TabIndex = 43;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(267, 3);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(29, 12);
			this.label7.TabIndex = 42;
			this.label7.Text = "帐号";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxServer
			// 
			this.textBoxServer.Location = new System.Drawing.Point(50, 0);
			this.textBoxServer.Name = "textBoxServer";
			this.textBoxServer.Size = new System.Drawing.Size(205, 21);
			this.textBoxServer.TabIndex = 43;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 42;
			this.label1.Text = "服务器";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabPageTxt
			// 
			this.tabPageTxt.Controls.Add(this.textBoxDataSourceReal);
			this.tabPageTxt.Controls.Add(this.textBoxDataSourceTick);
			this.tabPageTxt.Controls.Add(this.TextBoxDataSourceK);
			this.tabPageTxt.Controls.Add(this.buttonDataSourceReal);
			this.tabPageTxt.Controls.Add(this.buttonDataSourceTick);
			this.tabPageTxt.Controls.Add(this.ButtonDataSource);
			this.tabPageTxt.Location = new System.Drawing.Point(4, 4);
			this.tabPageTxt.Name = "tabPageTxt";
			this.tabPageTxt.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageTxt.Size = new System.Drawing.Size(921, 22);
			this.tabPageTxt.TabIndex = 0;
			this.tabPageTxt.Text = "Text";
			this.tabPageTxt.UseVisualStyleBackColor = true;
			// 
			// textBoxDataSourceReal
			// 
			this.textBoxDataSourceReal.Location = new System.Drawing.Point(555, 1);
			this.textBoxDataSourceReal.Name = "textBoxDataSourceReal";
			this.textBoxDataSourceReal.Size = new System.Drawing.Size(202, 21);
			this.textBoxDataSourceReal.TabIndex = 26;
			this.textBoxDataSourceReal.Text = "Z:\\";
			this.textBoxDataSourceReal.WordWrap = false;
			// 
			// textBoxDataSourceTick
			// 
			this.textBoxDataSourceTick.Location = new System.Drawing.Point(279, 1);
			this.textBoxDataSourceTick.Name = "textBoxDataSourceTick";
			this.textBoxDataSourceTick.Size = new System.Drawing.Size(202, 21);
			this.textBoxDataSourceTick.TabIndex = 27;
			this.textBoxDataSourceTick.Text = "Z:\\";
			this.textBoxDataSourceTick.WordWrap = false;
			// 
			// TextBoxDataSourceK
			// 
			this.TextBoxDataSourceK.Location = new System.Drawing.Point(2, 1);
			this.TextBoxDataSourceK.Name = "TextBoxDataSourceK";
			this.TextBoxDataSourceK.Size = new System.Drawing.Size(202, 21);
			this.TextBoxDataSourceK.TabIndex = 28;
			this.TextBoxDataSourceK.Text = "Z:\\";
			this.TextBoxDataSourceK.WordWrap = false;
			// 
			// buttonDataSourceReal
			// 
			this.buttonDataSourceReal.Location = new System.Drawing.Point(757, -1);
			this.buttonDataSourceReal.Name = "buttonDataSourceReal";
			this.buttonDataSourceReal.Size = new System.Drawing.Size(74, 25);
			this.buttonDataSourceReal.TabIndex = 29;
			this.buttonDataSourceReal.Text = "实时数据源";
			// 
			// buttonDataSourceTick
			// 
			this.buttonDataSourceTick.Location = new System.Drawing.Point(481, -1);
			this.buttonDataSourceTick.Name = "buttonDataSourceTick";
			this.buttonDataSourceTick.Size = new System.Drawing.Size(74, 25);
			this.buttonDataSourceTick.TabIndex = 30;
			this.buttonDataSourceTick.Text = "Tick数据源";
			// 
			// ButtonDataSource
			// 
			this.ButtonDataSource.Location = new System.Drawing.Point(204, -1);
			this.ButtonDataSource.Name = "ButtonDataSource";
			this.ButtonDataSource.Size = new System.Drawing.Size(75, 25);
			this.ButtonDataSource.TabIndex = 31;
			this.ButtonDataSource.Text = "K线数据源";
			// 
			// comboBoxStrategyFile
			// 
			this.comboBoxStrategyFile.FormattingEnabled = true;
			this.comboBoxStrategyFile.Location = new System.Drawing.Point(11, 34);
			this.comboBoxStrategyFile.Name = "comboBoxStrategyFile";
			this.comboBoxStrategyFile.Size = new System.Drawing.Size(682, 20);
			this.comboBoxStrategyFile.TabIndex = 28;
			// 
			// buttonStrategyFile
			// 
			this.buttonStrategyFile.Location = new System.Drawing.Point(697, 33);
			this.buttonStrategyFile.Name = "buttonStrategyFile";
			this.buttonStrategyFile.Size = new System.Drawing.Size(75, 23);
			this.buttonStrategyFile.TabIndex = 27;
			this.buttonStrategyFile.Text = "策略文件";
			this.buttonStrategyFile.UseVisualStyleBackColor = true;
			this.buttonStrategyFile.Click += new System.EventHandler(this.buttonLoadStrategy_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.buttonAddStra);
			this.groupBox1.Controls.Add(this.buttonDel);
			this.groupBox1.Controls.Add(this.dateTimePickerEnd);
			this.groupBox1.Controls.Add(this.dateTimePickerBegin);
			this.groupBox1.Controls.Add(this.groupBoxLoad);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.comboBoxInterval);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.comboBoxInst);
			this.groupBox1.Controls.Add(this.ComboBoxType);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 60);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1125, 35);
			this.groupBox1.TabIndex = 37;
			this.groupBox1.TabStop = false;
			// 
			// buttonAddStra
			// 
			this.buttonAddStra.Location = new System.Drawing.Point(691, 9);
			this.buttonAddStra.Name = "buttonAddStra";
			this.buttonAddStra.Size = new System.Drawing.Size(40, 23);
			this.buttonAddStra.TabIndex = 54;
			this.buttonAddStra.Text = "添加";
			this.buttonAddStra.UseVisualStyleBackColor = true;
			// 
			// buttonDel
			// 
			this.buttonDel.Location = new System.Drawing.Point(737, 9);
			this.buttonDel.Name = "buttonDel";
			this.buttonDel.Size = new System.Drawing.Size(40, 23);
			this.buttonDel.TabIndex = 53;
			this.buttonDel.Text = "删除";
			this.buttonDel.UseVisualStyleBackColor = true;
			// 
			// dateTimePickerEnd
			// 
			this.dateTimePickerEnd.Checked = false;
			this.dateTimePickerEnd.Location = new System.Drawing.Point(559, 10);
			this.dateTimePickerEnd.Name = "dateTimePickerEnd";
			this.dateTimePickerEnd.ShowCheckBox = true;
			this.dateTimePickerEnd.Size = new System.Drawing.Size(126, 21);
			this.dateTimePickerEnd.TabIndex = 49;
			// 
			// dateTimePickerBegin
			// 
			this.dateTimePickerBegin.Location = new System.Drawing.Point(423, 10);
			this.dateTimePickerBegin.Name = "dateTimePickerBegin";
			this.dateTimePickerBegin.Size = new System.Drawing.Size(107, 21);
			this.dateTimePickerBegin.TabIndex = 50;
			// 
			// groupBoxLoad
			// 
			this.groupBoxLoad.Controls.Add(this.radioButtonK);
			this.groupBoxLoad.Controls.Add(this.radioButtonT);
			this.groupBoxLoad.Controls.Add(this.buttonLoadStra);
			this.groupBoxLoad.Location = new System.Drawing.Point(812, 0);
			this.groupBoxLoad.Name = "groupBoxLoad";
			this.groupBoxLoad.Size = new System.Drawing.Size(151, 34);
			this.groupBoxLoad.TabIndex = 52;
			this.groupBoxLoad.TabStop = false;
			// 
			// radioButtonK
			// 
			this.radioButtonK.AutoSize = true;
			this.radioButtonK.Checked = true;
			this.radioButtonK.Location = new System.Drawing.Point(15, 13);
			this.radioButtonK.Name = "radioButtonK";
			this.radioButtonK.Size = new System.Drawing.Size(41, 16);
			this.radioButtonK.TabIndex = 45;
			this.radioButtonK.TabStop = true;
			this.radioButtonK.Text = "K线";
			this.radioButtonK.UseVisualStyleBackColor = true;
			// 
			// radioButtonT
			// 
			this.radioButtonT.AutoSize = true;
			this.radioButtonT.Enabled = false;
			this.radioButtonT.Location = new System.Drawing.Point(57, 13);
			this.radioButtonT.Name = "radioButtonT";
			this.radioButtonT.Size = new System.Drawing.Size(47, 16);
			this.radioButtonT.TabIndex = 44;
			this.radioButtonT.Text = "Tick";
			this.radioButtonT.UseVisualStyleBackColor = true;
			// 
			// buttonLoadStra
			// 
			this.buttonLoadStra.Location = new System.Drawing.Point(106, 10);
			this.buttonLoadStra.Name = "buttonLoadStra";
			this.buttonLoadStra.Size = new System.Drawing.Size(40, 23);
			this.buttonLoadStra.TabIndex = 43;
			this.buttonLoadStra.Text = "加载";
			this.buttonLoadStra.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(394, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(29, 12);
			this.label5.TabIndex = 39;
			this.label5.Text = "开始";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxInterval
			// 
			this.comboBoxInterval.FormattingEnabled = true;
			this.comboBoxInterval.Location = new System.Drawing.Point(333, 10);
			this.comboBoxInterval.Name = "comboBoxInterval";
			this.comboBoxInterval.Size = new System.Drawing.Size(60, 20);
			this.comboBoxInterval.TabIndex = 48;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(530, 14);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(29, 12);
			this.label6.TabIndex = 38;
			this.label6.Text = "结束";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxInst
			// 
			this.comboBoxInst.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.comboBoxInst.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboBoxInst.FormattingEnabled = true;
			this.comboBoxInst.Location = new System.Drawing.Point(230, 10);
			this.comboBoxInst.Name = "comboBoxInst";
			this.comboBoxInst.Size = new System.Drawing.Size(74, 20);
			this.comboBoxInst.TabIndex = 47;
			// 
			// ComboBoxType
			// 
			this.ComboBoxType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.ComboBoxType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.ComboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxType.DropDownWidth = 200;
			this.ComboBoxType.Location = new System.Drawing.Point(37, 10);
			this.ComboBoxType.Name = "ComboBoxType";
			this.ComboBoxType.Size = new System.Drawing.Size(164, 20);
			this.ComboBoxType.TabIndex = 37;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(304, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(29, 12);
			this.label4.TabIndex = 40;
			this.label4.Text = "周期";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(201, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 12);
			this.label3.TabIndex = 41;
			this.label3.Text = "合约";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 12);
			this.label2.TabIndex = 42;
			this.label2.Text = "策略";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 95);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.propertyGrid1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
			this.splitContainer2.Size = new System.Drawing.Size(1125, 458);
			this.splitContainer2.SplitterDistance = 195;
			this.splitContainer2.TabIndex = 38;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(195, 458);
			this.propertyGrid1.TabIndex = 27;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.DataGridViewStrategies);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.DataGridViewOrders);
			this.splitContainer1.Size = new System.Drawing.Size(926, 458);
			this.splitContainer1.SplitterDistance = 123;
			this.splitContainer1.TabIndex = 30;
			// 
			// DataGridViewStrategies
			// 
			this.DataGridViewStrategies.AllowUserToAddRows = false;
			this.DataGridViewStrategies.AllowUserToDeleteRows = false;
			this.DataGridViewStrategies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DataGridViewStrategies.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.DataGridViewStrategies.ColumnHeadersHeight = 27;
			this.DataGridViewStrategies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.DataGridViewStrategies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StraName,
            this.Type,
            this.Param,
            this.Instrument,
            this.Interval,
            this.BeginDate,
            this.EndDate,
            this.Loaded,
            this.Order,
            this.UpdateTime,
            this.ExcStatus,
            this.report,
            this.Graphics});
			this.DataGridViewStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataGridViewStrategies.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.DataGridViewStrategies.Location = new System.Drawing.Point(0, 0);
			this.DataGridViewStrategies.MultiSelect = false;
			this.DataGridViewStrategies.Name = "DataGridViewStrategies";
			this.DataGridViewStrategies.RowHeadersWidth = 6;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.DataGridViewStrategies.RowsDefaultCellStyle = dataGridViewCellStyle7;
			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.DataGridViewStrategies.RowTemplate.Height = 27;
			this.DataGridViewStrategies.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.DataGridViewStrategies.Size = new System.Drawing.Size(926, 123);
			this.DataGridViewStrategies.TabIndex = 30;
			// 
			// StraName
			// 
			this.StraName.HeaderText = "编号";
			this.StraName.Name = "StraName";
			this.StraName.ReadOnly = true;
			this.StraName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// Type
			// 
			this.Type.HeaderText = "类型";
			this.Type.Name = "Type";
			this.Type.ReadOnly = true;
			this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// Param
			// 
			this.Param.HeaderText = "参数";
			this.Param.Name = "Param";
			this.Param.ReadOnly = true;
			this.Param.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// Instrument
			// 
			this.Instrument.HeaderText = "合约";
			this.Instrument.Name = "Instrument";
			this.Instrument.ReadOnly = true;
			this.Instrument.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// Interval
			// 
			this.Interval.HeaderText = "周期";
			this.Interval.Name = "Interval";
			this.Interval.ReadOnly = true;
			this.Interval.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.Interval.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// BeginDate
			// 
			dataGridViewCellStyle2.Format = "d";
			dataGridViewCellStyle2.NullValue = null;
			this.BeginDate.DefaultCellStyle = dataGridViewCellStyle2;
			this.BeginDate.HeaderText = "开始";
			this.BeginDate.Name = "BeginDate";
			this.BeginDate.ReadOnly = true;
			// 
			// EndDate
			// 
			dataGridViewCellStyle3.Format = "d";
			dataGridViewCellStyle3.NullValue = null;
			this.EndDate.DefaultCellStyle = dataGridViewCellStyle3;
			this.EndDate.HeaderText = "结束";
			this.EndDate.Name = "EndDate";
			this.EndDate.ReadOnly = true;
			// 
			// Loaded
			// 
			this.Loaded.HeaderText = "状态";
			this.Loaded.Name = "Loaded";
			this.Loaded.ReadOnly = true;
			// 
			// Order
			// 
			this.Order.HeaderText = "委托";
			this.Order.Name = "Order";
			this.Order.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// UpdateTime
			// 
			this.UpdateTime.HeaderText = "时间";
			this.UpdateTime.Name = "UpdateTime";
			this.UpdateTime.ReadOnly = true;
			// 
			// ExcStatus
			// 
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.NullValue = "Normal";
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.ExcStatus.DefaultCellStyle = dataGridViewCellStyle4;
			this.ExcStatus.HeaderText = "交易";
			this.ExcStatus.Name = "ExcStatus";
			this.ExcStatus.ReadOnly = true;
			// 
			// report
			// 
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.NullValue = "报告";
			this.report.DefaultCellStyle = dataGridViewCellStyle5;
			this.report.HeaderText = "测试报告";
			this.report.Name = "report";
			// 
			// Graphics
			// 
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.NullValue = "显示";
			this.Graphics.DefaultCellStyle = dataGridViewCellStyle6;
			this.Graphics.HeaderText = "图形显示";
			this.Graphics.Name = "Graphics";
			// 
			// DataGridViewOrders
			// 
			this.DataGridViewOrders.AllowUserToAddRows = false;
			this.DataGridViewOrders.AllowUserToDeleteRows = false;
			this.DataGridViewOrders.AllowUserToOrderColumns = true;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DataGridViewOrders.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
			this.DataGridViewOrders.ColumnHeadersHeight = 27;
			this.DataGridViewOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.DataGridViewOrders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataGridViewOrders.Location = new System.Drawing.Point(0, 0);
			this.DataGridViewOrders.Name = "DataGridViewOrders";
			this.DataGridViewOrders.ReadOnly = true;
			this.DataGridViewOrders.RowHeadersWidth = 6;
			this.DataGridViewOrders.RowTemplate.Height = 27;
			this.DataGridViewOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.DataGridViewOrders.Size = new System.Drawing.Size(926, 331);
			this.DataGridViewOrders.TabIndex = 29;
			// 
			// Plat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.panel1);
			this.Name = "Plat";
			this.Size = new System.Drawing.Size(1125, 553);
			this.panel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPageDB.ResumeLayout(false);
			this.tabPageDB.PerformLayout();
			this.tabPageTxt.ResumeLayout(false);
			this.tabPageTxt.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBoxLoad.ResumeLayout(false);
			this.groupBoxLoad.PerformLayout();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.DataGridViewStrategies)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DataGridViewOrders)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonStrategyFile;
		private System.Windows.Forms.ComboBox comboBoxStrategyFile;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboBoxInterval;
		private System.Windows.Forms.ComboBox comboBoxInst;
		private System.Windows.Forms.ComboBox ComboBoxType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonLoadStra;
		private System.Windows.Forms.GroupBox groupBoxLoad;
		private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
		private System.Windows.Forms.DateTimePicker dateTimePickerBegin;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButtonK;
		private System.Windows.Forms.RadioButton radioButtonT;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Button buttonAddStra;
		private System.Windows.Forms.Button buttonDel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridView DataGridViewStrategies;
		private System.Windows.Forms.DataGridView DataGridViewOrders;
		private System.Windows.Forms.DataGridViewTextBoxColumn StraName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Type;
		private System.Windows.Forms.DataGridViewTextBoxColumn Param;
		private System.Windows.Forms.DataGridViewTextBoxColumn Instrument;
		private System.Windows.Forms.DataGridViewTextBoxColumn Interval;
		private System.Windows.Forms.DataGridViewTextBoxColumn BeginDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn EndDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn Loaded;
		private System.Windows.Forms.DataGridViewCheckBoxColumn Order;
		private System.Windows.Forms.DataGridViewTextBoxColumn UpdateTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn ExcStatus;
		private System.Windows.Forms.DataGridViewButtonColumn report;
		private System.Windows.Forms.DataGridViewButtonColumn Graphics;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageTxt;
		private System.Windows.Forms.TextBox textBoxDataSourceReal;
		private System.Windows.Forms.TextBox textBoxDataSourceTick;
		private System.Windows.Forms.TextBox TextBoxDataSourceK;
		private System.Windows.Forms.Button buttonDataSourceReal;
		private System.Windows.Forms.Button buttonDataSourceTick;
		private System.Windows.Forms.Button ButtonDataSource;
		private System.Windows.Forms.TabPage tabPageDB;
		private System.Windows.Forms.TextBox textBoxPwd;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textBoxUser;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBoxServer;
		private System.Windows.Forms.Label label1;
	}
}
