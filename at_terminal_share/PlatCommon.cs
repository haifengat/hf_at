using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
			_q.OnRspUserLogout += _q_OnRspUserLogout;
			_q.OnRtnTick += _q_OnRtnTick;
			_q.ReqConnect(front);
		}

		private void _q_OnRspUserLogout(object sender, IntEventArgs e)
		{
			_dicTick000.Clear();
			LogInfo("行情退出");
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
			ExchangeStatusType excStatus;
			if (!_dataProcess.InstrumentInfo.TryGetValue(e.Tick.InstrumentID, out inst) || !_dataProcess.ProductInfo.TryGetValue(inst.ProductID, out instField) || !_t.DicExcStatus.TryGetValue(instField._id, out excStatus))
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
				UpdateMillisec = e.Tick.UpdateMillisec,
				UpdateTime = e.Tick.UpdateTime,
				Volume = e.Tick.Volume,
			};
			//20170720全处理,避免000的行情错误.if (_t.DicExcStatus.Count > 1) //非模拟才进行处理

			if (!_dataProcess.FixTick(tick, _t.TradingDay, _t.DicInstrumentField[tick.InstrumentID].ProductID))    //修正tick时间格式:yyyMMdd HH:mm:ss
				return;


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
				if (excStatus != ExchangeStatusType.Trading) return;	//只在交易时段处理数据
				if (tick.UpdateTime.CompareTo(f000.UpdateTime) <= 0 || string.IsNullOrEmpty(f000.UpdateTime)) //第2个tick再处理;增加稳定性
				{
					_dicTick000[tick.InstrumentID] = tick; //注意f000的先后顺序
					f000.UpdateTime = tick.UpdateTime;
					return;
				}
				f000.UpdateTime = tick.UpdateTime;

				double priceTick = instField.PriceTick;

				int sumV = 0;
				double sumI = 0;
				List<Tick> ts = new List<Tick>();

				foreach (var instInfo in _dataProcess.InstrumentInfo.Values.Where(n => n.ProductID == instField._id))
				{
					if (instInfo._id == f000.InstrumentID) continue;
					Tick md;
					if (!_dicTick000.TryGetValue(instInfo._id, out md)) continue;
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
					f000.OpenInterest = sumI;
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

						f000.LastPrice += (v.LastPrice * rate);
						f000.BidPrice += (v.BidPrice * rate);
						f000.BidVolume += v.BidVolume;
						f000.AskPrice += (v.AskPrice * rate);
						f000.AskVolume += v.AskVolume;
						f000.AveragePrice += (v.AveragePrice * rate);
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
							v.Value.Datas[0].OnTick(f000);
							v.Value.Update();
						}
					}
				}
				//更新合约数据
				_dicTick000[tick.InstrumentID] = tick; //注意f000的先后顺序
			}
		}


		//订阅合约
		void SubscribeInstrument(string inst)
		{
			//000
			if (inst.EndsWith("000"))
			{
				var insts = _dataProcess.InstrumentInfo.Where(n => n.Value.ProductID == inst.TrimEnd('0')).Select(n => n.Value._id).ToArray();
				if (insts.Count() > 0)
				{
					_dicTick000.TryAdd(inst, new Tick
					{
						InstrumentID = inst,
						UpdateTime = string.Empty,
						UpdateMillisec = 0,
					});
					//_q.ReqSubscribeMarketData(insts);//不能订阅多个??
					foreach (var v in insts)
						_q.ReqSubscribeMarketData(v);
					return;
				}
			}
			_q.ReqSubscribeMarketData(inst);
		}

		//保存策略
		void SaveStrategy()
		{
			List<StrategyConfig> list = new List<StrategyConfig>();
			foreach (DataGridViewRow row in this.DataGridViewStrategies.Rows)
			{//参数置于最后,避免参数中的','影响加载时分隔
				Strategy stra;
				if (!_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out stra))
					continue;
				list.Add(new StrategyConfig
				{
					Name = stra.Name,
					Type = stra.GetType(),
					Instrument = (string)row.Cells["Instrument"].Value,
					InstrumentOrder = (string)row.Cells["InstrumentOrder"].Value,
					Interval = (string)row.Cells["Interval"].Value,
					BeginDate = (DateTime)row.Cells["BeginDate"].Value,
					EndDate = (DateTime?)row.Cells["EndDate"].Value,
					Params = (string)row.Cells["Param"].Value,
				});
			}
			File.WriteAllText("strategies.cfg", JsonConvert.SerializeObject(list));
		}

		//加载保存的策略到列表里
		void LoadStrategy()
		{
			if (File.Exists("strategies.cfg"))
			{
				var list = JsonConvert.DeserializeObject<List<StrategyConfig>>(File.ReadAllText("strategies.cfg"));
				foreach (var sc in list)
				{
					Type straType = null;
					//类型是否存在
					foreach (Type t in this.ComboBoxType.Items)
					{
						if (t == sc.Type)
						{
							straType = t;
							break;
						}
					}
					if (straType == null)
						continue;
					Strategy stra = (Strategy)Activator.CreateInstance(straType);
					//参数赋值
					foreach (var v in sc.Params.Trim('(', ')').Split(','))
					{
						stra.SetParameterValue(v.Split(':')[0], v.Split(':')[1]);
					}

					int rid = AddStra(stra, sc.Name, sc.Instrument, sc.InstrumentOrder, sc.Interval, sc.BeginDate, sc.EndDate);

					LogInfo($"{stra.Name,8},读取策略 {(rid == -1 ? "出错" : "完成")}");
				}
			}
		}

		//选择不同策略:显示策略成交记录
		private void DataGridViewStrategies_SelectionChanged(object sender, EventArgs e)
		{
			_dtOrders.Rows.Clear();
			if (this.DataGridViewStrategies.SelectedRows.Count == 0)
				return;

			DataGridViewRow row = this.DataGridViewStrategies.SelectedRows[0];

			if (_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out _curStrategy))
			{
				if (_curStrategy.Datas.Count > 0)
				{
					foreach (var v in _curStrategy.Operations)
					{
						_dtOrders.Rows.Add(_dtOrders.Rows.Count + 1, v.Date, v.Dir, v.Offset, v.Price, v.Lots, v.Remark);
					}
				}
			}
		}

		//加载/委托/报告/图显
		private void DataGridViewStrategies_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex < 0 || e.RowIndex < 0)
			{
				return;
			}

			DataGridViewRow row = this.DataGridViewStrategies.Rows[e.RowIndex];
			string name = (string)row.Cells["StraName"].Value;
			Strategy stra;
			if (_dicStrategies.TryGetValue(name, out stra))
			{
				var head = row.Cells[e.ColumnIndex].OwningColumn.Name;
				this.DataGridViewStrategies.EndEdit();
				if (head == "Order")
				{
					//勾选委托
					if ((bool)this.DataGridViewStrategies[e.ColumnIndex, e.RowIndex].Value)
						_listOrderStra.Add(stra); //可以发送委托
					else
						_listOrderStra.Remove(stra);
				}
				else if (head == "report") //报告
				{
					if (stra.Operations == null || stra.Operations.Count == 0)
						MessageBox.Show("策略无交易");
					else
						new FormTest(stra).Show();
				}
				else if (head == "Graphics") //报告
				{
					new FormWorkSpace(_curStrategy).Show();
				}
				else if (head == "Loaded") //加载
				{
					LoadStraData(e.RowIndex, stra, (string)row.Cells["Interval"].Value, (string)row.Cells["Instrument"].Value, (string)row.Cells["InstrumentOrder"].Value, (DateTime)row.Cells["BeginDate"].Value, row.Cells["EndDate"].Value == null ? DateTime.MaxValue : (DateTime)row.Cells["EndDate"].Value);
				}
			}
		}

		//格式化时间字段
		private void DataGridViewStrategies_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.Value == null) return;

			if (this.DataGridViewStrategies.Columns[e.ColumnIndex].HeaderText == "时间")
			{
				double val;
				DateTime dt;
				if (double.TryParse((string)e.Value, out val) && DateTime.TryParseExact(val.ToString("00000000.0000"), "yyyyMMdd.HHmm", null, DateTimeStyles.None, out dt))
					e.Value = dt.ToString("yyyy/MM/dd HH:mm");
			}
		}
	}
}
