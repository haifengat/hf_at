using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static HaiFeng.HFLog;

namespace HaiFeng
{
	public partial class Plat
	{
		DataTable _dtServer = null;
		//限制3秒内不允许重复点击登录按钮
		private DateTime _logTime = DateTime.MinValue;
		private TradeExt _t;
		private QuoteExt _q;

		//读取前置配置信息
		void ReadServerConfig()
		{
			//20170606:与Q7兼容
			_dtServer = new DataTable();
			_dtServer.Columns.Add("txt", typeof(string));
			_dtServer.Columns.Add("val", typeof(string));
			_dtServer.PrimaryKey = new[] { _dtServer.Columns[0] };
			if (!File.Exists("./simnow.xml"))
			{
				File.WriteAllText("./simnow.xml", Properties.Resources.simnow, Encoding.GetEncoding("GB2312"));
			}
			XmlDocument doc = new XmlDocument();
			foreach (var file in Directory.GetFiles("./", "*.xml", SearchOption.TopDirectoryOnly))
			{
				doc.Load(file);
				XmlElement broker = doc.DocumentElement["broker"];
				//过滤非配置文件
				if (broker == null) continue;

				var brokerid = broker.GetAttribute("BrokerID");
				var brokername = broker.GetAttribute("BrokerName");
				XmlNodeList nlist = broker["Servers"].ChildNodes;
				foreach (XmlNode server in nlist)
				{
					var linename = brokername + "-" + server["Name"].InnerText;
					var tradefront = "";
					foreach (XmlNode addr in server["Trading"].ChildNodes)
						tradefront += $"tcp://{addr.InnerText},";
					tradefront = tradefront.TrimEnd(',');
					var quotefront = "";
					foreach (XmlNode addr in server["MarketData"].ChildNodes)
						quotefront += $"tcp://{addr.InnerText},";
					quotefront = quotefront.TrimEnd(',');
					_dtServer.Rows.Add(linename, $"ctp|{brokerid}|{tradefront}|{quotefront}");
				}
			}
			if (_dtServer.Rows.Count == 0)
			{
				MessageBox.Show("未读取到服务器配置数据!");
				return;
			}
		}

		void LoginQuote(string[] front, string _Broker, string _Investor, string _Password)
		{
			if (_q != null)
			{
				if (_q.IsLogin)
					_q.ReqUserLogout();
				_q = null;
			}
			_q = new QuoteExt
			{
				Broker = _Broker,
				Investor = _Investor,
				Password = _Password,
			};
			_q.OnFrontConnected += quote_OnFrontConnected;
			_q.OnRspUserLogin += quote_OnRspUserLogin;
			_q.OnRtnTick += _q_OnRtnTick;
			_q.ReqConnect(front);
		}

		void quote_OnFrontConnected(object sender, EventArgs e)
		{
			var q = (QuoteExt)sender;
			q.ReqUserLogin(q.Investor, q.Password, q.Broker);
		}

		void LoginTrade(string[] front, string _Broker, string _Investor, string _Password)
		{
			if ((DateTime.Now - _logTime).TotalSeconds < 3)
				return;

			if (_t != null)
			{
				if (_t.IsLogin)
					_t.ReqUserLogout();
				_t = null;
			}

			_logTime = DateTime.Now;
			LogInfo("connecting ...");

			_t = new TradeExt
			{
				Broker = _Broker,
				Investor = _Investor,
				Password = _Password,
			};
			_t.OnFrontConnected += trade_OnFrontConnected;
			_t.OnRspUserLogin += trade_OnRspUserLogin;
			_t.OnRtnExchangeStatus += trade_OnRtnExchangeStatus;
			//接口相关信息
			_t.OnInfo += (msg) => LogInfo(msg);
			_t.OnRspUserLogout += _t_OnRspUserLogout;
			_t.OnRtnNotice += (snd, ea) => LogWarn($"重要提示: {ea.Value}");

			_t.ReqConnect(front);
		}

		void trade_OnFrontConnected(object sender, EventArgs e)
		{
			LogDebug("连接成功");
			var t = (TradeExt)sender;
			t.ReqUserLogin(t.Investor, t.Password, t.Broker);
			LogInfo("登录中 ...");
		}

	}
}
