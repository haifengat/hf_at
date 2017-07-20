using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DataCenter;
using static HaiFeng.HFLog;
using Numeric = System.Decimal;

namespace HaiFeng
{
	public partial class Plat
	{
		DataTable _dtServer = null;
		//限制3秒内不允许重复点击登录按钮
		private DateTime _logTime = DateTime.MinValue;
		private TradeExt _t;
		private QuoteExt _q;

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

		string[] fs;

		//登录
		private void ButtonLogin_Click(object sender, EventArgs e)
		{
			fs = ((string)this.comboBoxServer.SelectedValue).Split('|');
			LoginTrade(fs[2].Split(','), fs[1], this.textBoxUser.Text, this.textBoxPwd.Text);
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

		void quote_OnRspUserLogin(object sender, IntEventArgs e)
		{
			//隔夜处理,夜盘开始后需重新订阅.
			foreach (var v in _listOnTickStra)
			{
				SubscribeInstrument(v.InstrumentID);
				SubscribeInstrument(v.Datas[0].InstrumentOrder);
			}
			QuoteLogged();
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

		void trade_OnRtnExchangeStatus(object sender, StatusEventArgs e)
		{
			//ShowMsg(e.Exchange + "=>" + e.Status);
		}

		void trade_OnFrontConnected(object sender, EventArgs e)
		{
			LogDebug("连接成功");
			var t = (TradeExt)sender;
			t.ReqUserLogin(t.Investor, t.Password, t.Broker);
			LogInfo("登录中 ...");
		}

		void _q_OnRtnTick(object sender, TickEventArgs e)
		{
			Product instField;
			Instrument inst;
			if (!_dataProcess.InstrumentInfo.TryGetValue(e.Tick.InstrumentID, out inst) || !_dataProcess.ProductInfo.TryGetValue(inst.ProductID, out instField))
				return;

			Tick tick = new Tick
			{
				AskPrice = (decimal)e.Tick.AskPrice,
				AveragePrice = (decimal)e.Tick.AveragePrice,
				BidPrice = (decimal)e.Tick.BidPrice,
				LastPrice = (decimal)e.Tick.LastPrice,
				LowerLimitPrice = (decimal)e.Tick.LowerLimitPrice,
				OpenInterest = (decimal)e.Tick.OpenInterest,
				UpperLimitPrice = (decimal)e.Tick.UpperLimitPrice,
				AskVolume = e.Tick.AskVolume,
				BidVolume = e.Tick.BidVolume,
				InstrumentID = e.Tick.InstrumentID,
				UpdateMillisec = e.Tick.UpdateMillisec,
				UpdateTime = e.Tick.UpdateTime,
				Volume = e.Tick.Volume,
			};
			//20170720全处理,避免000的行情错误.if (_t.DicExcStatus.Count > 1) //非模拟才进行处理
			{
				if (!_dataProcess.FixTick(tick, _t.TradingDay, _t.DicInstrumentField[tick.InstrumentID].ProductID))    //修正tick时间格式:yyyMMdd HH:mm:ss
					return;
			}

			foreach (var v in _dicStrategies)
			{
				if (_listOnTickStra.IndexOf(v.Value) >= 0 && v.Value.InstrumentID == tick.InstrumentID)
				{
					v.Value.Datas[0].OnTick(tick);
					v.Value.Update();
				}
			}


			//处理000数据;20170719增加状态判断，非交易时段会收到脏数据！=>fixtick处理
			Tick f000;
			if (_dicTick000.TryGetValue(instField._id + "000", out f000)) //yyyyMMdd HH:mm:ss格式比较
			{
				if (_dicTick000.TryAdd(tick.InstrumentID, tick)) return;//首个tick只保存不处理

				if (tick.UpdateTime.CompareTo(f000.UpdateTime) <= 0) return;
				if (string.IsNullOrEmpty(f000.UpdateTime)) //第2个tick再处理;增加稳定性
				{
					f000.UpdateTime = tick.UpdateTime;
					return;
				}
				f000.UpdateTime = tick.UpdateTime;

				Numeric priceTick = (Numeric)instField.PriceTick;

				int sumV = 0;
				double sumI = 0;
				List<MarketData> ts = new List<MarketData>();

				foreach (var instInfo in _dataProcess.InstrumentInfo.Values.Where(n => n.ProductID == instField._id))
				{
					MarketData md;
					if (!_q.DicTick.TryGetValue(instInfo._id, out md)) continue;
					if (md.OpenInterest <= 0) continue;
					ts.Add(md);
				}
				//无有用数据:不处理
				if (ts.Count > 0)
				{
					foreach (var v in ts)
					{
						sumV += v.Volume;
						sumI += v.OpenInterest;
					}

					f000.Volume = sumV;
					f000.OpenInterest = (Numeric)sumI;
					f000.UpdateTime = tick.UpdateTime;

					//数据初始化
					f000.LastPrice = 0;
					f000.BidPrice = 0;
					f000.BidVolume = 0;
					f000.AskPrice = 0;
					f000.AskVolume = 0;
					f000.AveragePrice = 0;

					foreach (var v in ts)
					{
						double rate = v.OpenInterest / sumI;

						f000.LastPrice += (Numeric)(v.LastPrice * rate);
						f000.BidPrice += (Numeric)(v.BidPrice * rate);
						f000.BidVolume += v.BidVolume;
						f000.AskPrice += (Numeric)(v.AskPrice * rate);
						f000.AskVolume += v.AskVolume;
						f000.AveragePrice += (Numeric)(v.AveragePrice * rate);
					}
					//数据修正
					f000.LastPrice = Math.Round(f000.LastPrice / priceTick, 0) * priceTick;
					f000.BidPrice = Math.Round(f000.BidPrice / priceTick, 0) * priceTick;
					f000.AskPrice = Math.Round(f000.AskPrice / priceTick, 0) * priceTick;
					f000.AveragePrice = Math.Round(f000.AveragePrice / priceTick, 0) * priceTick;

					foreach (var v in _dicStrategies)
					{
						if (_listOnTickStra.IndexOf(v.Value) >= 0 && v.Value.InstrumentID == f000.InstrumentID)
						{
							v.Value.Datas[0].OnTick(new Tick
							{
								AskPrice = f000.AskPrice,
								AskVolume = f000.AskVolume,
								AveragePrice = f000.AveragePrice,
								BidPrice = f000.BidPrice,
								BidVolume = f000.BidVolume,
								InstrumentID = f000.InstrumentID,
								LastPrice = f000.LastPrice,
								LowerLimitPrice = f000.LowerLimitPrice,
								OpenInterest = f000.OpenInterest,
								UpdateMillisec = f000.UpdateMillisec,
								UpdateTime = f000.UpdateTime,
								UpperLimitPrice = f000.UpperLimitPrice,
								Volume = f000.Volume,
							});
							v.Value.Update();
						}
					}
				}
				//更新合约数据
				//_dicTick000[tick.InstrumentID] = tick; //注意f000的先后顺序
			}
		}
	}
}
