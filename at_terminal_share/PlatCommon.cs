using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DataCenter;
using Newtonsoft.Json;
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
		private readonly ConcurrentDictionary<string, Tick> _dicTick000 = new ConcurrentDictionary<string, Tick>(); //用于处理000数据

		public Plat()
		{
			InitializeComponent();
			this.buttonLogin.Click += ButtonLogin_Click;
			this.buttonOffline.Click += Offline_Click;
			ReadServerConfig();

			this.comboBoxServer.DataSource = _dtServer;
			this.comboBoxServer.DisplayMember = "txt";
			this.comboBoxServer.ValueMember = "val";

			//读取登录信息
			if (File.Exists("login.ini"))
			{
				string[] fs = File.ReadAllLines("login.ini");
				this.comboBoxServer.Text = fs[0].Split('@')[1];
				this.textBoxUser.Text = fs[0].Split('@')[0];
				this.ActiveControl = this.textBoxPwd;
			}
		}

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
			LogWarn("connecting ...");

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
			_t.OnInfo += trade_OnInfo;
			_t.OnRspUserLogout += trade_OnRspUserLogout;
			_t.OnRtnNotice += trade_OnRtnNotice;

			_t.ReqConnect(front);
		}


		private void trade_OnRspUserLogout(object sender, IntEventArgs e)
		{
			LogError($"交易接口断开,{e.Value,4}");
			this.Invoke(new Action(() =>
			{
				this.pictureBox1.Image = Properties.Resources.Close;
				this.toolTip1.SetToolTip(this.pictureBox1, "停止");
				this.toolTip1.Show("停止.", this.pictureBox1, 6000);
			}));
		}

		private void trade_OnRspUserLogin(object sender, IntEventArgs e)
		{
			if (e.Value == 0)
			{
				LogWarn("登录成功.");
				_tradingDay = ((Trade)sender).TradingDay;

				if (!string.IsNullOrEmpty(fs[3]))
					LoginQuote(fs[3].Split(','), fs[1], this.textBoxUser.Text, this.textBoxPwd.Text);

				this.Invoke(new Action(() =>
				{
					this.pictureBox1.Image = Properties.Resources.Open;
					this.toolTip1.SetToolTip(this.pictureBox1, "已连接");
					//this.toolTip1.Show("已连接.", this.pictureBox1, 6000);

					this.toolTip1.SetToolTip(this.ComboBoxType, "策略文件(dll)放置在(./strategies)目录中.");
					this.toolTip1.Show("策略文件(dll)放置在(./strategies)目录中.", this.ComboBoxType, 6000);
				}));
			}
			else
			{
				LogError("login error:" + e.Value);
				_t.ReqUserLogout();
				_t = null;
				_q = null;
			}
		}

		private void trade_OnRtnNotice(object sender, StringEventArgs e)
		{
			LogWarn($"重要提示信息:\n{e.Value}");
		}

		private void trade_OnInfo(string msg) { LogWarn(msg); }


		private void trade_OnRtnExchangeStatus(object sender, StatusEventArgs e)
		{
			//ShowMsg(e.Exchange + "=>" + e.Status);
		}

		private void trade_OnFrontConnected(object sender, EventArgs e)
		{
			var t = (TradeExt)sender;
			t.ReqUserLogin(t.Investor, t.Password, t.Broker);
			LogWarn("登录中 ...");
		}

		private void LoginQuote(string[] front, string _Broker, string _Investor, string _Password)
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
			_q.OnRspUserLogout += quote_OnRspUserLogout;
			_q.OnRtnTick += quote_OnRtnTick;
			_q.ReqConnect(front);
		}

		private void quote_OnRspUserLogout(object sender, IntEventArgs e)
		{
			_dicTick000.Clear();
			LogError($"行情断开,{e.Value,4}");
		}

		private void quote_OnFrontConnected(object sender, EventArgs e)
		{
			LogWarn("行情连接");
			var q = (QuoteExt)sender;
			q.ReqUserLogin(q.Investor, q.Password, q.Broker);
		}

		private void quote_OnRspUserLogin(object sender, IntEventArgs e)
		{
			//交易日历,需每年更新
			_dataProcess.UpdateInfo();
			_tradingDate = _dataProcess.TradeDates;
			//隔夜处理,夜盘开始后需重新订阅.
			foreach (var stra in _dicStrategies.Values)
			{
				if (!stra.EnableTick) continue;
				foreach (var data in stra.Datas)
				{
					LogInfo($"订阅行情{data.Instrument}");
					SubscribeInstrument(data.Instrument);
					SubscribeInstrument(data.InstrumentOrder);
				}
			}
			LogWarn("行情登录成功");
			QuoteLogged();
		}


		private void QuoteLogged()
		{
			//未登录过(多次登录时不处理)
			if (this.panelLogin.Enabled)
			{
				this.Invoke(new Action(() =>
				{
					this.panelLogin.Enabled = false;
					Console.Title += $" ({textBoxUser.Text}@{comboBoxServer.Text})";
					this.ParentForm.Text = Console.Title;
					File.WriteAllText("login.ini", _t.Investor + "@" + this.comboBoxServer.Text);
					InitControls();
				}));
			}
		}

		private void quote_OnRtnTick(object sender, TickEventArgs e)
		{
			Product instField;
			Instrument inst;
			ExchangeStatusType excStatus = ExchangeStatusType.Trading;
			if (!_dataProcess.InstrumentInfo.TryGetValue(e.Tick.InstrumentID, out inst) || !_dataProcess.ProductInfo.TryGetValue(inst.ProductID, out instField) || (_t != null && !_t.DicExcStatus.TryGetValue(instField._id, out excStatus)))
				return;

			Tick tick = new Tick
			{
				AskPrice = e.Tick.AskPrice,
				AveragePrice = e.Tick.AveragePrice,
				BidPrice = e.Tick.BidPrice,
				LastPrice = e.Tick.LastPrice,
				LowerLimitPrice = e.Tick.LowerLimitPrice,
				OpenInterest = e.Tick.OpenInterest,
				UpperLimitPrice = e.Tick.UpperLimitPrice,
				AskVolume = e.Tick.AskVolume,
				BidVolume = e.Tick.BidVolume,
				InstrumentID = e.Tick.InstrumentID,
				TradingDay = int.Parse(_tradingDay),
				UpdateMillisec = e.Tick.UpdateMillisec,
				UpdateTime = e.Tick.UpdateTime,
				Volume = e.Tick.Volume,
			};
			//20170720全处理,避免000的行情错误.
			//if (_t.DicExcStatus.Count > 1) //非模拟才进行处理
			if (!_dataProcess.FixTick(tick, _tradingDay, _dataProcess.InstrumentInfo[tick.InstrumentID].ProductID))    //修正tick时间格式:yyyMMdd HH:mm:ss
				return;

			foreach (var stra in _dicStrategies.Values)
				if (stra.EnableTick)
					foreach (var data in stra.Datas)
						if (data.Instrument == tick.InstrumentID)
							if (sender == null)//tick回测
								data.OnTick(tick);
							else
								ThreadPool.QueueUserWorkItem((state) => data.OnTick(tick));

			//处理000数据;20170719增加状态判断，非交易时段会收到脏数据！=>fixtick处理
			if (_dicTick000.TryAdd(tick.InstrumentID, tick)) return;//首个tick只保存不处理
			if (excStatus != ExchangeStatusType.Trading) return;    //只在交易时段处理数据

			//if (_qry.Make000(md, _dicTick, out tick000))
			if (_inst888.IndexOf(tick.InstrumentID) >= 0)
				if (Make000Double(tick, out Tick tick000))
				{
					foreach (var stra in _dicStrategies.Values)
						if (stra.EnableTick)
							foreach (var data in stra.Datas)
								if (data.Instrument == tick000.InstrumentID)
								{
									if (sender == null)//tick回测
										data.OnTick(tick000);
									else
										ThreadPool.QueueUserWorkItem((state) => data.OnTick(tick000));
								}
					tick000.UpdateTime = tick.UpdateTime;
					tick000.UpdateMillisec = tick.UpdateMillisec;
				}
			//更新合约数据
			_dicTick000[tick.InstrumentID] = tick; //注意f000的先后顺序
		}


		bool Make000Double(Tick tick, out Tick tick000)
		{
			var proc = _dataProcess.InstrumentInfo[tick.InstrumentID].ProductID;
			tick000 = _dicTick000.GetOrAdd(_dataProcess.InstrumentInfo[tick.InstrumentID].ProductID + "000", new Tick
			{
				InstrumentID = proc + "000",
				UpdateTime = tick.UpdateTime,
				UpdateMillisec = tick.UpdateMillisec,
			});

			//小节开盘=>旧数据不处理
			if ((DateTime.ParseExact(tick.UpdateTime, "yyyyMMdd HH:mm:ss", null) - DateTime.ParseExact(tick000.UpdateTime, "yyyyMMdd HH:mm:ss", null)).TotalMinutes > 10)
				_dicTick000.Clear();

			//只处理rate中的合约
			var ticks = _dicTick000.Where(n => _dataProcess.InstrumentInfo[n.Key].ProductID == proc && _dataProcess.Rate000.Keys.FirstOrDefault(m => m == n.Key) != null);

			tick000.LastPrice = 0;
			tick000.Volume = 0;
			tick000.OpenInterest = 0;

			//至少收到所有合约的数据
			if (ticks.Count() == _dataProcess.Rate000.Count(n => _dataProcess.InstrumentInfo[n.Key].ProductID == proc))
				foreach (var t in ticks)
				{
					var rate = _dataProcess.Rate000[t.Value.InstrumentID];
					tick000.LastPrice += t.Value.LastPrice * rate;
					tick000.Volume += t.Value.Volume;
					tick000.OpenInterest += t.Value.OpenInterest;
				}
			else if (ticks.Count() == 2) //只收到2个
			{
				var cnt = ticks.Sum(n => n.Value.OpenInterest);
				foreach (var t in ticks)
				{
					var rate = t.Value.OpenInterest / cnt;
					tick000.LastPrice += t.Value.LastPrice * rate;
					tick000.Volume += t.Value.Volume;
					tick000.OpenInterest += t.Value.OpenInterest;
				}
			}
			else
				return false;
			tick000.LastPrice = (int)(tick000.LastPrice / _dataProcess.ProductInfo[proc].PriceTick) * _dataProcess.ProductInfo[proc].PriceTick;//小数修正
			return true;
		}

		//订阅合约
		void SubscribeInstrument(string inst)
		{
			if (inst.EndsWith("000"))
			{
				var insts = _dataProcess.Rate000.Where(n => _dataProcess.InstrumentInfo[n.Key].ProductID == inst.TrimEnd('0')).Select(n => n.Key).ToArray();
				if (insts.Count() > 0)
				{
					_q.ReqSubscribeMarketData(insts);
					return;
				}
			}
			_q.ReqSubscribeMarketData(inst);
		}

	}
}
