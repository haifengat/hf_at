using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HaiFeng
{
	public partial class FormLogin : Form
	{
		public FormLogin()
		{
			InitializeComponent();
			Init();
		}

		readonly DataTable _dt = new DataTable("ServerConfig");
		public TradeExt Trade;
		public Quote Quote;
		public string Server;
		private DateTime _logTime = DateTime.MinValue;

		void Init()
		{
			this.Load += FormLogin_Load;
			this.FormClosed += FormLogin_FormClosed;
			this.KryptonButtonLogin.Click += kryptonButtonLogin_Click;
			this.KryptonButtonExit.Click += kryptonButtonExit_Click;
			this.button1.Click += Button1_Click;
			this.KeyPress += NextTab;
			//this.splitContainer1.Panel2Collapsed = true;
			//this.Width = this.splitContainer1.Panel1.Width;
		}
		//保存
		private void Button1_Click(object sender, EventArgs e)
		{
			File.WriteAllText("server.txt", this.richTextBox1.Text, Encoding.GetEncoding("GB2312"));
			ReadServer();
		}
		void ReadServer()
		{
			_dt.Rows.Clear();
			this.KryptonComboBoxServer.Items.Clear();
			if (!File.Exists("server.txt"))
				File.WriteAllText("server.txt", @"模拟,ctp|9999|tcp://180.168.146.187:10000|tcp://180.168.146.187:10010
股指仿真,ctp|1010|tcp://simctp1010.yhqh.com:41205|tcp://simctp1010.yhqh.com:41213", Encoding.GetEncoding("GB2312"));
			var lines = File.ReadAllLines("server.txt", Encoding.GetEncoding("GB2312"));
			this.richTextBox1.Lines = lines;
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line))
					continue;
				_dt.Rows.Add(line.Split(','));
			}

			foreach (DataRow dr in _dt.Rows)
				this.KryptonComboBoxServer.Items.Add(dr["txt"]);

			//保存的登录信息
			if (File.Exists("login.ini"))
			{
				string[] fs = File.ReadAllLines("login.ini");
				this.KryptonComboBoxServer.Text = fs[0].Split('@')[1];
				this.KryptonTextBoxInvestor.Text = fs[0].Split('@')[0];
				this.ActiveControl = this.KryptonTextBoxPassword;
			}
		}

		private void FormLogin_Load(object sender, EventArgs e)
		{
			//前置列表
			_dt.Columns.Add("txt");
			_dt.Columns.Add("val");
			_dt.PrimaryKey = new[] { _dt.Columns[0] };
			ReadServer();
		}

		private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void kryptonButtonLogin_Click(object sender, EventArgs e)
		{
			if ((DateTime.Now - _logTime).TotalSeconds < 3)
				return;
			_logTime = DateTime.Now;
			ShowMsg("connecting ...");
			string front = (string)_dt.Rows.Find(this.KryptonComboBoxServer.Text)[1];
			string[] fs = front.Split('|');

			if (!string.IsNullOrEmpty(fs[3]))
			{
				Quote = new Quote
				{
					Broker = fs[1],
					Server = fs[3],
					Investor = this.KryptonTextBoxInvestor.Text,
					Password = this.KryptonTextBoxPassword.Text,
				};
				Quote.OnFrontConnected += quote_OnFrontConnected;
				Quote.OnRspUserLogin += quote_OnRspUserLogin;
			}

			Trade = new TradeExt
			{
				Server = fs[2],
				Investor = this.KryptonTextBoxInvestor.Text,
				Password = this.KryptonTextBoxPassword.Text,
				Broker = fs[1],
			};
			Trade.OnFrontConnected += trade_OnFrontConnected;
			Trade.OnRspUserLogin += trade_OnRspUserLogin;
			Trade.OnRtnExchangeStatus += trade_OnRtnExchangeStatus;
			Trade.ReqConnect();
		}

		private void kryptonButtonExit_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void NextTab(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				if (this.KryptonTextBoxPassword.Focused || this.KryptonButtonLogin.Focused)
				{
					this.KryptonButtonLogin.PerformClick();
				}
				else if (this.KryptonTextBoxInvestor.Focused)
					this.KryptonTextBoxPassword.Focus();
			}
		}


		void trade_OnRtnExchangeStatus(object sender, StatusEventArgs e)
		{
			ShowMsg(e.Exchange + "=>" + e.Status);
		}

		void trade_OnRspUserLogin(object sender, IntEventArgs e)
		{
			if (e.Value == 0)
			{
				ShowMsg("login successed.");
				//Thread.Sleep(1500);
				//交易登录成功后,登录行情
				if (Quote == null)
					LoginSuccess();
				else
					Quote.ReqConnect();
			}
			else
			{
				ShowMsg("login error:" + e.Value);
				Trade.ReqUserLogout();
				Trade = null;
				Quote = null;
			}
		}

		void trade_OnFrontConnected(object sender, EventArgs e)
		{
			ShowMsg("connected.");
			((Trade)sender).ReqUserLogin();
			ShowMsg("loging ...");
		}

		void quote_OnRspUserLogin(object sender, IntEventArgs e)
		{
			LoginSuccess();
		}

		void quote_OnFrontConnected(object sender, EventArgs e)
		{
			((Quote)sender).ReqUserLogin();
		}

		//登录成功
		void LoginSuccess()
		{
			Trade.OnFrontConnected -= trade_OnFrontConnected;
			Trade.OnRspUserLogin -= trade_OnRspUserLogin;
			Trade.OnRtnExchangeStatus -= trade_OnRtnExchangeStatus;
			if (Quote != null)
			{
				Quote.OnFrontConnected -= quote_OnFrontConnected;
				Quote.OnRspUserLogin -= quote_OnRspUserLogin;
			}

			this.Invoke(new Action(() =>
			{
				Server = this.KryptonComboBoxServer.Text;
				File.WriteAllText("login.ini", Trade.Investor + "@" + Server);
				this.DialogResult = DialogResult.OK;
			}));
		}

		/// <summary>
		/// 消息传递,接口登录过程中有消息返回时被调用.
		/// </summary>
		public Action<string> Msg;

		void ShowMsg(string pMsg)
		{
			try
			{
				this.Invoke(new Action(() =>
				{
					//var parentForm = (FormShadom)this.Owner.ActiveMdiChild;
					//if (parentForm != null) parentForm.ShowMsg(pMsg);
					if (Msg != null)
					{
						Msg(pMsg);
					}
					this._toolStripStatusLabelInfo.Text = DateTime.Now.ToString("HH:mm:ss") + "|" + pMsg;
				}));
			}
			catch
			{
				// ignored
			}
		}

	}
}
