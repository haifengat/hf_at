namespace HaiFeng
{
	partial class FormLogin
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
			this._statusStrip1 = new System.Windows.Forms.StatusStrip();
			this._toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
			this.KryptonButtonExit = new System.Windows.Forms.Button();
			this.KryptonButtonLogin = new System.Windows.Forms.Button();
			this.KryptonComboBoxServer = new System.Windows.Forms.ComboBox();
			this._kryptonLabel3 = new System.Windows.Forms.Label();
			this._kryptonLabel2 = new System.Windows.Forms.Label();
			this._kryptonLabel1 = new System.Windows.Forms.Label();
			this.KryptonTextBoxPassword = new System.Windows.Forms.TextBox();
			this.KryptonTextBoxInvestor = new System.Windows.Forms.TextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.button1 = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this._statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _statusStrip1
			// 
			this._statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripStatusLabelInfo});
			this._statusStrip1.Location = new System.Drawing.Point(0, 182);
			this._statusStrip1.Name = "_statusStrip1";
			this._statusStrip1.Size = new System.Drawing.Size(875, 22);
			this._statusStrip1.TabIndex = 14;
			this._statusStrip1.Text = "statusStrip1";
			// 
			// _toolStripStatusLabelInfo
			// 
			this._toolStripStatusLabelInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this._toolStripStatusLabelInfo.Name = "_toolStripStatusLabelInfo";
			this._toolStripStatusLabelInfo.Size = new System.Drawing.Size(41, 17);
			this._toolStripStatusLabelInfo.Text = "未登录";
			this._toolStripStatusLabelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// KryptonButtonExit
			// 
			this.KryptonButtonExit.AutoSize = true;
			this.KryptonButtonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.KryptonButtonExit.Location = new System.Drawing.Point(144, 119);
			this.KryptonButtonExit.Name = "KryptonButtonExit";
			this.KryptonButtonExit.Size = new System.Drawing.Size(90, 28);
			this.KryptonButtonExit.TabIndex = 13;
			this.KryptonButtonExit.Text = "退  出";
			// 
			// KryptonButtonLogin
			// 
			this.KryptonButtonLogin.AutoSize = true;
			this.KryptonButtonLogin.Location = new System.Drawing.Point(18, 119);
			this.KryptonButtonLogin.Name = "KryptonButtonLogin";
			this.KryptonButtonLogin.Size = new System.Drawing.Size(90, 28);
			this.KryptonButtonLogin.TabIndex = 3;
			this.KryptonButtonLogin.Text = "登  录";
			// 
			// KryptonComboBoxServer
			// 
			this.KryptonComboBoxServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.KryptonComboBoxServer.DropDownWidth = 124;
			this.KryptonComboBoxServer.Location = new System.Drawing.Point(59, 22);
			this.KryptonComboBoxServer.Name = "KryptonComboBoxServer";
			this.KryptonComboBoxServer.Size = new System.Drawing.Size(124, 20);
			this.KryptonComboBoxServer.TabIndex = 0;
			// 
			// _kryptonLabel3
			// 
			this._kryptonLabel3.AutoSize = true;
			this._kryptonLabel3.Location = new System.Drawing.Point(10, 80);
			this._kryptonLabel3.Margin = new System.Windows.Forms.Padding(3);
			this._kryptonLabel3.Name = "_kryptonLabel3";
			this._kryptonLabel3.Size = new System.Drawing.Size(41, 12);
			this._kryptonLabel3.TabIndex = 7;
			this._kryptonLabel3.Text = "密　码";
			this._kryptonLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _kryptonLabel2
			// 
			this._kryptonLabel2.AutoSize = true;
			this._kryptonLabel2.Location = new System.Drawing.Point(10, 54);
			this._kryptonLabel2.Margin = new System.Windows.Forms.Padding(3);
			this._kryptonLabel2.Name = "_kryptonLabel2";
			this._kryptonLabel2.Size = new System.Drawing.Size(41, 12);
			this._kryptonLabel2.TabIndex = 8;
			this._kryptonLabel2.Text = "帐　号";
			this._kryptonLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _kryptonLabel1
			// 
			this._kryptonLabel1.AutoSize = true;
			this._kryptonLabel1.Location = new System.Drawing.Point(10, 27);
			this._kryptonLabel1.Margin = new System.Windows.Forms.Padding(3);
			this._kryptonLabel1.Name = "_kryptonLabel1";
			this._kryptonLabel1.Size = new System.Drawing.Size(41, 12);
			this._kryptonLabel1.TabIndex = 9;
			this._kryptonLabel1.Text = "服务器";
			this._kryptonLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// KryptonTextBoxPassword
			// 
			this.KryptonTextBoxPassword.Location = new System.Drawing.Point(59, 75);
			this.KryptonTextBoxPassword.Name = "KryptonTextBoxPassword";
			this.KryptonTextBoxPassword.PasswordChar = '*';
			this.KryptonTextBoxPassword.Size = new System.Drawing.Size(100, 21);
			this.KryptonTextBoxPassword.TabIndex = 2;
			// 
			// KryptonTextBoxInvestor
			// 
			this.KryptonTextBoxInvestor.Location = new System.Drawing.Point(59, 49);
			this.KryptonTextBoxInvestor.Name = "KryptonTextBoxInvestor";
			this.KryptonTextBoxInvestor.Size = new System.Drawing.Size(100, 21);
			this.KryptonTextBoxInvestor.TabIndex = 1;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.button1);
			this.splitContainer1.Panel1.Controls.Add(this._kryptonLabel1);
			this.splitContainer1.Panel1.Controls.Add(this.KryptonTextBoxInvestor);
			this.splitContainer1.Panel1.Controls.Add(this.KryptonButtonExit);
			this.splitContainer1.Panel1.Controls.Add(this.KryptonTextBoxPassword);
			this.splitContainer1.Panel1.Controls.Add(this.KryptonButtonLogin);
			this.splitContainer1.Panel1.Controls.Add(this._kryptonLabel2);
			this.splitContainer1.Panel1.Controls.Add(this.KryptonComboBoxServer);
			this.splitContainer1.Panel1.Controls.Add(this._kryptonLabel3);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
			this.splitContainer1.Size = new System.Drawing.Size(875, 182);
			this.splitContainer1.SplitterDistance = 245;
			this.splitContainer1.TabIndex = 15;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(190, 18);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(44, 23);
			this.button1.TabIndex = 14;
			this.button1.Text = "保存";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(626, 182);
			this.richTextBox1.TabIndex = 99;
			this.richTextBox1.Text = "";
			// 
			// FormLogin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(875, 204);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this._statusStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLogin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "登录";
			this.TopMost = true;
			this._statusStrip1.ResumeLayout(false);
			this._statusStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip _statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel _toolStripStatusLabelInfo;
		public System.Windows.Forms.Button KryptonButtonExit;
		public System.Windows.Forms.Button KryptonButtonLogin;
		public System.Windows.Forms.ComboBox KryptonComboBoxServer;
		private System.Windows.Forms.Label _kryptonLabel3;
		private System.Windows.Forms.Label _kryptonLabel2;
		private System.Windows.Forms.Label _kryptonLabel1;
		public System.Windows.Forms.TextBox KryptonTextBoxPassword;
		public System.Windows.Forms.TextBox KryptonTextBoxInvestor;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RichTextBox richTextBox1;
	}
}