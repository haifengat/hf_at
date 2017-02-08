using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
using HaiFeng;
using System.IO;
using System.Xml;
using System.Reflection;
using Numeric = System.Decimal;
using System.Globalization;
using System.Net;
using static HaiFeng.HFLog;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace HaiFeng
{
	public partial class Plat : UserControl
	{

		public Plat(TradeExt pTrade, Quote pQuote)
		{
			_t = pTrade;
			_q = pQuote;
			_t.StartFollow(_q);

			_t.OnInfo += (msg) => LogInfo(msg);

			InitializeComponent();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			SaveStrategy();
			SaveConfig();
			//解除事件绑定及现场清理
			_timer.Stop();
			_q.OnRtnTick -= _q_OnRtnTick;
			base.OnHandleDestroyed(e);
		}

		private ConcurrentDictionary<string, Strategy> _dicStrategies = new ConcurrentDictionary<string, Strategy>();
		private readonly List<Strategy> _listOrderStra = new List<Strategy>();
		private readonly List<Strategy> _listOnTickStra = new List<Strategy>();

		private readonly TradeExt _t;
		private readonly Quote _q;
		private readonly DataTable _dtOrders = new DataTable();
		private DataProcess _dataProcess = new DataProcess();

		private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer
		{
			Interval = 1000
		};

		private Strategy _curStrategy;
		private readonly ConcurrentDictionary<string, MarketData> _dicTick000 = new ConcurrentDictionary<string, MarketData>(); //用于处理000数据


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			//合约列表
			var listInst = _t.DicInstrumentField.Keys.ToList();
			listInst.Sort();
			this.comboBoxInst.Items.AddRange(listInst.ToArray());

			//时间周期
			this.comboBoxInterval.Items.AddRange(new[] { "Sec 1", "Min 1", "Min 3", "Min 5", "Min 10", "Min 15", "Min 30", "Hour 1", "Day 1", "Week 1", "Year 1" });

			//设置表格
			_dtOrders.Columns.Add("ID", typeof(int));//增加编号列
			foreach (PropertyInfo v in typeof(OrderItem).GetProperties())
			{
				//有说明的属性才显示
				if (v.GetCustomAttributes(false).Any())
					_dtOrders.Columns.Add(v.Name, v.PropertyType);
			}
			//_dtOrders.PrimaryKey = new[] { _dtOrders.Columns[""] };
			this.DataGridViewOrders.DataSource = _dtOrders;

			_q.OnRtnTick += _q_OnRtnTick;

			LoadConfig();
			LoadStrategy();

			_timer.Tick += _timer_Tick;
			_timer.Start();
			this.buttonAddStra.Click += ButtonAdd_Click;//添加策略
			this.buttonDel.Click += ButtonDel_Click;
			this.buttonLoadStra.Click += ButtonLoadStra_Click; //加载策略
			this.DataGridViewStrategies.SelectionChanged += DataGridViewStrategies_SelectionChanged;
			this.DataGridViewStrategies.CellContentClick += DataGridViewStrategies_CellContentClick;
			this.DataGridViewStrategies.CellFormatting += DataGridViewStrategies_CellFormatting;

			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.SelectionBackColor = this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.BackColor;
			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.SelectionForeColor = this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.ForeColor;

			this.propertyGridParams.PropertyValueChanged += propertyGridParams_PropertyValueChanged;

			this.propertyGridFlo.SelectedObject = _t.FloConfig; //跟单配置
		}

		//添加策略
		void ButtonAdd_Click(object sender, EventArgs e)
		{
			if (this.ComboBoxType.SelectedIndex < 0) return;
			if (string.IsNullOrEmpty(this.comboBoxInst.Text)) return;
			if (this.comboBoxInterval.SelectedIndex < 0) return;

			Strategy stra = (Strategy)Activator.CreateInstance((Type)this.ComboBoxType.SelectedItem);
			AddStra(stra, _dicStrategies.Count == 0 ? "1" : (_dicStrategies.Select(n => int.Parse(n.Key)).Max() + 1).ToString(), this.comboBoxInst.Text, this.comboBoxInterval.Text, this.dateTimePickerBegin.Value.Date, this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date : DateTime.MaxValue);
		}

		private void ButtonDel_Click(object sender, EventArgs e)
		{
			if (this.DataGridViewStrategies.SelectedRows.Count == 0) return;

			DataGridViewRow row = this.DataGridViewStrategies.SelectedRows[0];
			string name = (string)row.Cells["StraName"].Value;
			Strategy stra;
			_dicStrategies.TryGetValue(name, out stra);
			if (stra != null)
			{
				_listOnTickStra.Remove(stra);
				//if ((bool)row.Cells["Order"].Value)
				_listOrderStra.Remove(stra);
			}
			this.DataGridViewStrategies.Rows.Remove(row);
		}

		private int AddStra(Strategy stra, string pName, string pInstrument, string pInterval, DateTime pBegin, DateTime pEnd)
		{
			if (!_dicStrategies.TryAdd(pName, stra))
			{
				MessageBox.Show("名称不能重复", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return -1;
			}
			stra.Name = pName;
			int rid = this.DataGridViewStrategies.Rows.Add(stra.Name, stra.GetType(), stra.GetParams(), pInstrument, pInterval, pBegin, pEnd);
			this.DataGridViewStrategies.Rows[rid].Cells["Order"].Value = false;
			if (pEnd.Date == DateTime.MaxValue.Date)
				this.DataGridViewStrategies.Rows[rid].Cells["EndDate"].Value = null;
			//this.DataGridViewStrategies.Rows[rid].Cells["BeginDate"].Value = DateTime.Today.AddMonths(-3);

			this.propertyGridParams.SelectedObject = stra;   //属性显示

			stra.OnRtnOrder += stra_OnRtnOrder;
			return rid;
		}

		//加载策略
		private void ButtonLoadStra_Click(object sender, EventArgs e)
		{
			if (this.DataGridViewStrategies.SelectedRows.Count == 0) return;

			//if (_dataProcess.ProductInfo.Count == 0)
			//{
			//	HFLog.LogInfo("读取配置信息(交易日历,交易时间段)...");
			//	_dataProcess.UpdateInfo();  //更新信息
			//}
			DataGridViewRow row = this.DataGridViewStrategies.SelectedRows[0];
			string name = (string)row.Cells["StraName"].Value;
			Strategy stra;
			if (!_dicStrategies.TryGetValue(name, out stra))
			{
				LogError($"无对应的策略:{name}");
				return;
			}

			//参数是否可修改
			this.propertyGridParams.Enabled = !this.propertyGridParams.Enabled;

			if (row.Cells["Loaded"].Value != null && row.Cells["Loaded"].Value.Equals("已加载")) return;

			string internalType = (string)row.Cells["Interval"].Value;
			var inst = (string)row.Cells["Instrument"].Value;

			Data data = new Data
			{
				Interval = int.Parse(internalType.Split(' ')[1]),
				IntervalType = (EnumIntervalType)Enum.Parse(typeof(EnumIntervalType), internalType.Split(' ')[0]),
				Instrument = inst,
			};

			string proc = new string(inst.TakeWhile(char.IsLetter).ToArray());
			data.InstrumentInfo = new InstrumentInfo
			{
				InstrumentID = inst,
				ProductID = proc,
				PriceTick = 1,
				VolumeMultiple = 10,    //默认值
			};

			Product pi;
			if (_dataProcess.ProductInfo.TryGetValue(proc, out pi))
			{
				data.InstrumentInfo.PriceTick = (decimal)pi.PriceTick;
				data.InstrumentInfo.VolumeMultiple = (int)pi.VolumeTuple;
			}

			DateTime dtBegin = (DateTime)row.Cells["BeginDate"].Value;
			DateTime dtEnd;// = this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date.AddDays(1) : DateTime.MaxValue;
			if (row.Cells["EndDate"].Value == null)
				dtEnd = DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null).AddDays(1);
			else
				dtEnd = (DateTime)row.Cells["EndDate"].Value;

			_dtOrders.Rows.Clear(); //清除委托列表

			row.Cells["Loaded"].Value = "加载中...";
			this.DataGridViewStrategies.EndEdit();
			this.DataGridViewStrategies.Refresh();

			if (data.IntervalType == EnumIntervalType.Sec || this.radioButtonT.Checked) //秒周期或tick加载
			{ }
			else
			{
				List<Bar> bars = null;
				if (data.IntervalType == EnumIntervalType.Min || data.IntervalType == EnumIntervalType.Hour)
				{
					bars = _dataProcess.GetData.QueryMin(inst, dtBegin.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd")).Select(n => new Bar
					{
						D = DateTime.ParseExact(n._id, "yyyyMMdd HH:mm:ss", null),
						O = (decimal)n.Open,
						H = (decimal)n.High,
						L = (decimal)n.Low,
						C = (decimal)n.Close,
						V = n.Volume,
						I = (decimal)n.OpenInterest
					}).ToList();
					// 取当日数据
					if (dtEnd > DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null))
					{
						var listReal = _dataProcess.GetData.QueryReal(inst);
						bars = (bars ?? new List<Bar>());
						if (listReal != null)
							bars.AddRange(listReal.Select(n => new Bar
							{
								D = DateTime.ParseExact(n._id, "yyyyMMdd HH:mm:ss", null),
								O = (decimal)n.Open,
								H = (decimal)n.High,
								L = (decimal)n.Low,
								C = (decimal)n.Close,
								V = n.Volume,
								I = (decimal)n.OpenInterest
							}).ToList());
					}
				}
				else
					bars = _dataProcess.GetData.QueryDay(inst, dtBegin.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd")).Select(n => new Bar
					{
						/*{ 
							"_id" : "20140519", 
							"Open" : 16050.0, 
							"High" : 16050.0, 
							"Low" : 15890.0, 
							"Close" : 16015.0, 
							"Volume" : NumberInt(3604), 
							"OpenInterest" : 2984.0, 
							"UpperLimit" : 17280.0, 
							"LowerLimit" : 14720.0, 
							"Settlement" : 15965.0
						}*/
						D = DateTime.ParseExact(n._id, "yyyyMMdd", null),
						O = (decimal)n.Open,
						H = (decimal)n.High,
						L = (decimal)n.Low,
						C = (decimal)n.Close,
						V = n.Volume,
						I = (decimal)n.OpenInterest
					}).ToList();


				if (bars == null)
				{
					return;
				}

				if (data.Interval > 1 || data.IntervalType != EnumIntervalType.Min)
					bars = GetUpperPeriod(bars, data.Interval, data.IntervalType);

				//参数是否可修改
				this.propertyGridParams.Enabled = false;
				//加载与tick加载同时处理
				row.Cells["Loaded"].Value = "已加载";
				foreach (Bar bar in bars)
					data.Add(bar); //加入bar
								   //=>初始化策略/回测
				stra.Init(data);
			}
			DataGridViewStrategies_SelectionChanged(null, null); //刷新委托列表
																 //是否有结束日期:只测试
																 //if (dtEnd == DateTime.MaxValue)
			{
				//订阅000合约
				if (stra.InstrumentID.EndsWith("000"))
				{
					foreach (var g in _t.DicInstrumentField.Where(n => n.Value.ProductID == stra.InstrumentID.TrimEnd('0')))
					{
						_q.ReqSubscribeMarketData(g.Key);
						_dicTick000.TryAdd(g.Key, new MarketData
						{
							InstrumentID = g.Key,
						});
					}
					_dicTick000.TryAdd(stra.InstrumentID, new MarketData
					{
						InstrumentID = stra.InstrumentID,
						UpdateMillisec = 0,
					});
				}
				else
					_q.ReqSubscribeMarketData(inst);
				_listOnTickStra.Add(stra); //可以接收实际行情
			}
		}

		//选择不同策略:显示策略成交记录
		private void DataGridViewStrategies_SelectionChanged(object sender, EventArgs e)
		{
			_dtOrders.Rows.Clear();
			if (this.DataGridViewStrategies.SelectedRows.Count == 0)
				return;

			DataGridViewRow row = this.DataGridViewStrategies.SelectedRows[0];
			this.propertyGridParams.Enabled = row.Cells["Loaded"].Value == null || !row.Cells["Loaded"].Value.Equals("已加载");

			if (_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out _curStrategy))
			{
				this.propertyGridParams.SelectedObject = _curStrategy;
				if (_curStrategy.Datas.Count > 0)
				{
					foreach (var v in _curStrategy.Operations)
					{
						_dtOrders.Rows.Add(_dtOrders.Rows.Count + 1, v.Date, v.Dir, v.Offset, v.Price, v.Lots, v.Remark);
					}
				}
			}
		}

		//委托/报告/图显
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
			}
		}

		//策略参数被修改:提交
		private void propertyGridParams_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			foreach (DataGridViewRow row in this.DataGridViewStrategies.Rows)
			{
				Strategy stra;
				if (_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out stra) && stra.Equals(this.propertyGridParams.SelectedObject))
				{
					row.Cells["Param"].Value = stra.GetParams();
					return;
				}
			}
		}

		//格式化时间字段
		private void DataGridViewStrategies_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.Value == null) return;

			if (this.DataGridViewStrategies.Columns[e.ColumnIndex].HeaderText == "时间")
			{
				decimal val;
				DateTime dt;
				if (decimal.TryParse((string)e.Value, out val) && DateTime.TryParseExact(val.ToString("00000000.0000"), "yyyyMMdd.HHmm", null, DateTimeStyles.None, out dt))
					e.Value = dt.ToString("yyyy/MM/dd HH:mm");
			}
		}

		private void SaveConfig()
		{
			ConfigFile cfg = new ConfigFile
			{
				StrategyFiles = new string[this.comboBoxStrategyFile.Items.Count],
				FloConfig = _t.FloConfig,
			};
			for (int i = 0; i < this.comboBoxStrategyFile.Items.Count; i++)
			{
				cfg.StrategyFiles[i] = (string)this.comboBoxStrategyFile.Items[i];
			}
			File.WriteAllText("config.json", JsonConvert.SerializeObject(cfg));
		}

		private void LoadConfig()
		{
			ConfigFile cfg = new ConfigFile();
			if (File.Exists("config.json"))
			{
				cfg = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText("config.json"));
				foreach (var file in cfg.StrategyFiles)
				{
					LoadStrategyFile(file);
				}
				_t.FloConfig = cfg.FloConfig;
			}
		}

		void SaveStrategy()
		{
			string str = "StraName,Type,Instrument,Interval,BeginDate,EndDate,Param\r\n";
			foreach (DataGridViewRow row in this.DataGridViewStrategies.Rows)
			{//参数置于最后,避免参数中的','影响加载时分隔
				Strategy stra;
				if (!_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out stra))
					continue;
				str += string.Format("{0},{1},{2},{3},{4},{5} ,{6}\r\n", stra.Name, stra.GetType(),
					row.Cells["Instrument"].Value, row.Cells["Interval"].Value,
					row.Cells["BeginDate"].Value, row.Cells["EndDate"].Value, row.Cells["Param"].Value);
			}
			File.WriteAllText("strategies.txt", str);
		}

		void LoadStrategy()
		{
			if (File.Exists("strategies.txt"))
			{
				string[] lines = File.ReadAllLines("strategies.txt");
				for (int i = 1; i < lines.Length; ++i)
				{
					string[] fields = lines[i].Split(new[] { ',' }, 7);
					Type straType = null;
					//类型是否存在
					foreach (Type t in this.ComboBoxType.Items)
					{
						if (t.ToString() == fields[1])
						{
							straType = t;
							break;
						}
					}
					if (straType == null)
						continue;
					Strategy stra = (Strategy)Activator.CreateInstance(straType);
					//参数赋值
					foreach (var v in fields[6].Trim('(', ')').Split(','))
					{
						stra.SetParameterValue(v.Split(':')[0], v.Split(':')[1]);
					}

					int rid = AddStra(stra, fields[0], fields[2], fields[3], DateTime.Parse(fields[4]), string.IsNullOrWhiteSpace(fields[5]) ? DateTime.MaxValue : DateTime.Parse(fields[5]));
					//if (rid == -1)//加载不成功
					//{
					//	continue;
					//}
					////按读取数据修正
					//this.DataGridViewStrategies.Rows[rid].Cells["Instrument"].Value = fields[2];
					//this.DataGridViewStrategies.Rows[rid].Cells["Interval"].Value = fields[3];
					//this.DataGridViewStrategies.Rows[rid].Cells["BeginDate"].Value = DateTime.Parse(fields[4]);
					//if (!string.IsNullOrWhiteSpace(fields[5]))
					//{
					//	//((DataGridViewTextBoxCell)this.DataGridViewStrategies.Rows[rid].Cells["EndDate"]).Checked = true;
					//	this.DataGridViewStrategies.Rows[rid].Cells["EndDate"].Value = DateTime.Parse(fields[5]);
					//}
				}
			}
		}

		void _timer_Tick(object sender, EventArgs e)
		{

			//更新时间与交易所状态
			foreach (DataGridViewRow row in this.DataGridViewStrategies.Rows)
			{
				Strategy stra;
				if (_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out stra))
				{
					if (stra.Datas.Count > 0)
					{
						row.Cells["UpdateTime"].Value = string.IsNullOrEmpty(stra.Tick.UpdateTime) ? stra.D[0].ToString() : stra.Tick.UpdateTime.Split(' ')[1];
						ExchangeStatusType status;
						if (_t.DicExcStatus.Count == 1)
							row.Cells["ExcStatus"].Value = _t.DicExcStatus.ElementAt(0).Value;
						else if (_t.DicExcStatus.TryGetValue(stra.InstrumentInfo.ProductID, out status))
							row.Cells["ExcStatus"].Value = status;
					}
				}
			}
		}

		void _q_OnRtnTick(object sender, TickEventArgs e)
		{
			InstrumentField instField;
			if (!_t.DicInstrumentField.TryGetValue(e.Tick.InstrumentID, out instField))
				return;

			MarketData tick = e.Tick;
			if (_t.DicExcStatus.Count > 1) //非模拟才进行处理
			{
				if (!_dataProcess.FixTick(tick, _t.TradingDay))    //修正tick时间格式:yyyMMdd HH:mm:ss
					return;
			}

			foreach (var v in _dicStrategies)
			{
				if (_listOnTickStra.IndexOf(v.Value) >= 0 && v.Value.InstrumentID == tick.InstrumentID)
				{
					v.Value.Datas[0].OnTick(new Tick
					{
						AskPrice = (Numeric)tick.AskPrice,
						AskVolume = tick.AskVolume,
						AveragePrice = (Numeric)tick.AveragePrice,
						BidPrice = (Numeric)tick.BidPrice,
						BidVolume = tick.BidVolume,
						InstrumentID = tick.InstrumentID,
						LastPrice = (Numeric)tick.LastPrice,
						LowerLimitPrice = (Numeric)tick.LowerLimitPrice,
						OpenInterest = (Numeric)tick.OpenInterest,
						UpdateMillisec = tick.UpdateMillisec,
						UpdateTime = tick.UpdateTime,
						UpperLimitPrice = (Numeric)tick.UpperLimitPrice,
						Volume = tick.Volume,
					});
					v.Value.Update();
				}
			}


			//处理000数据
			MarketData f000;
			if (_dicTick000.TryGetValue(instField.ProductID + "000", out f000) && tick.UpdateTime != f000.UpdateTime)
			{
				if (string.IsNullOrEmpty(f000.UpdateTime))
					f000.UpdateTime = e.Tick.UpdateTime;
				else
				{
					double priceTick = instField.PriceTick;

					int sumV = 0;
					double sumI = 0;
					List<MarketData> ts = new List<MarketData>();

					foreach (var inst in _t.DicInstrumentField.Values.Where(n => n.ProductID == instField.ProductID))
					{
						MarketData md;
						if (!_dicTick000.TryGetValue(inst.InstrumentID, out md)) continue;
						if (md.Volume <= 0 || md.OpenInterest <= 0) continue;
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

						foreach (var v in ts)
						{
							double rate = v.Volume / sumV * 0.1 + v.OpenInterest / sumI * 0.9;

							f000.LastPrice += v.LastPrice * rate;
							f000.BidPrice += v.BidPrice * rate;
							f000.BidVolume += v.BidVolume;
							f000.AskPrice += v.AskPrice * rate;
							f000.AskVolume += v.AskVolume;
							f000.AveragePrice += v.AveragePrice * rate;
						}
						//数据修正
						f000.LastPrice = Math.Round(f000.LastPrice / priceTick, 0) * priceTick;
						f000.BidPrice = Math.Round(f000.BidPrice / priceTick, 0) * priceTick;
						f000.AskPrice = Math.Round(f000.AskPrice / priceTick, 0) * priceTick;
						f000.AveragePrice = Math.Round(f000.AveragePrice / priceTick, 0) * priceTick;

						tick = f000; //指向000数据
						foreach (var v in _dicStrategies)
						{
							if (_listOnTickStra.IndexOf(v.Value) >= 0 && v.Value.InstrumentID == tick.InstrumentID)
							{
								v.Value.Datas[0].OnTick(new Tick
								{
									AskPrice = (Numeric)tick.AskPrice,
									AskVolume = tick.AskVolume,
									AveragePrice = (Numeric)tick.AveragePrice,
									BidPrice = (Numeric)tick.BidPrice,
									BidVolume = tick.BidVolume,
									InstrumentID = tick.InstrumentID,
									LastPrice = (Numeric)tick.LastPrice,
									LowerLimitPrice = (Numeric)tick.LowerLimitPrice,
									OpenInterest = (Numeric)tick.OpenInterest,
									UpdateMillisec = tick.UpdateMillisec,
									UpdateTime = tick.UpdateTime,
									UpperLimitPrice = (Numeric)tick.UpperLimitPrice,
									Volume = tick.Volume,
								});
								v.Value.Update();
							}
						}
					}
					//更新合约数据
					_dicTick000[e.Tick.InstrumentID] = e.Tick; //注意f000的先后顺序
				}
			}
		}

		//策略委托:
		void stra_OnRtnOrder(OrderItem pOrderItem, Data pData, Strategy pStrategy)
		{
			_dtOrders.Rows.Add(_dtOrders.Rows.Count + 1, pOrderItem.Date, pOrderItem.Dir, pOrderItem.Offset, pOrderItem.Price, pOrderItem.Lots, pOrderItem.Remark);
			//实际委托
			if (_listOrderStra.IndexOf(pStrategy) >= 0)
			{
				LogInfo($"{pOrderItem.Date},{pOrderItem.Dir},{pOrderItem.Offset},{pOrderItem.Price},{pOrderItem.Lots},{pOrderItem.Remark}");

				//处理上期所平今操作
				if (pOrderItem.Offset == Offset.Close)
				{
					int lots = pOrderItem.Lots;
					_t.ClosePosi(pData.Instrument, pOrderItem.Dir == Direction.Buy ? DirectionType.Sell : DirectionType.Buy, (double)pOrderItem.Price, pOrderItem.Lots, int.Parse(pStrategy.Name) * 100);
				}
				else
					_t.ReqOrderInsert(pData.Instrument, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Open, (double)pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name) * 100);
			}
		}

		private List<Bar> GetUpperPeriod(IList<Bar> datasOfMinute, int pInterval, EnumIntervalType pIntervalType)
		{
			DateTime dtBegin = datasOfMinute[0].D;
			//取首K时间
			switch (pIntervalType)
			{
				case EnumIntervalType.Min:
					dtBegin = dtBegin.Date.AddHours(dtBegin.Hour).AddMinutes(dtBegin.Minute / pInterval * pInterval);
					break;
				case EnumIntervalType.Hour:
					dtBegin = dtBegin.Date.AddHours(dtBegin.Hour / pInterval * pInterval);
					break;
				case EnumIntervalType.Day:
					dtBegin = dtBegin.Date.Date;
					break;
				case EnumIntervalType.Week:
					dtBegin = dtBegin.Date.AddDays(1 - (byte)dtBegin.DayOfWeek);
					break;
				case EnumIntervalType.Month:
					dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, 1);
					break;
				case EnumIntervalType.Year:
					dtBegin = new DateTime(dtBegin.Year, 1, 1);
					break;
				default:
					throw new Exception("参数错误");
			}
			List<Bar> tmp = new List<Bar>();
			Bar item = new Bar
			{
				D = dtBegin,
				O = datasOfMinute[0].O,
				H = datasOfMinute[0].H,
				L = datasOfMinute[0].L,
				C = datasOfMinute[0].C
			};
			//item.Volume = datas[0].Volume;
			//item.Value = datas[0].Value;
			//item.Avg = datas[0].Avg;
			tmp.Add(item);
			DateTime dtNext = DateTime.MinValue;
			switch (pIntervalType)
			{
				case EnumIntervalType.Min:
					dtNext = dtBegin.AddMinutes(pInterval);
					break;
				case EnumIntervalType.Hour:
					dtNext = dtBegin.AddHours(pInterval);
					break;
				case EnumIntervalType.Day:
					dtNext = dtBegin.AddDays(pInterval);
					break;
				case EnumIntervalType.Week:
					dtNext = dtBegin.AddDays(7 * pInterval);
					break;
				case EnumIntervalType.Month:
					dtNext = dtBegin.AddMonths(pInterval);
					break;
				case EnumIntervalType.Year:
					dtNext = dtBegin.AddYears(pInterval);
					break;
			}
			foreach (Bar v in datasOfMinute)
			{
				//在当前K线范围内:更新
				if (v.D >= dtBegin && v.D < dtNext)
				{
					item.H = Math.Max(item.H, v.H);
					item.L = Math.Min(item.L, v.L);
					item.C = v.C;
					item.V += v.V;
					item.I = v.I;
					item.A = v.A;
				}
				else
				{
					//超出当前K线范围
					while (v.D >= dtNext)
					{
						dtBegin = dtNext;
						switch (pIntervalType)
						{
							case EnumIntervalType.Min:
								dtNext = dtBegin.AddMinutes(pInterval);
								break;
							case EnumIntervalType.Hour:
								dtNext = dtBegin.AddHours(pInterval);
								break;
							case EnumIntervalType.Day:
								dtNext = dtBegin.AddDays(pInterval);
								break;
							case EnumIntervalType.Week:
								dtNext = dtBegin.AddDays(7 * pInterval);
								break;
							case EnumIntervalType.Month:
								dtNext = dtBegin.AddMonths(pInterval);
								break;
							case EnumIntervalType.Year:
								dtNext = dtBegin.AddYears(pInterval);
								break;
						}
					}
					item = new Bar
					{
						D = dtBegin,
						O = v.O,
						H = v.H,
						L = v.L,
						C = v.C,
						V = v.V,
						I = v.I,
						A = v.A
					};
					tmp.Add(item);
				}
			}
			return tmp;
		}

		private void LoadStrategyFile(string file)
		{
			try
			{
				Assembly ass = Assembly.LoadFile(file);
				//加载hf_plat报错:增加对hf_plat_core的引用
				foreach (var t in ass.GetTypes())
				{
					if (t.BaseType == typeof(Strategy))
						this.ComboBoxType.Items.Add(t);
				}
				this.comboBoxStrategyFile.Items.Insert(0, file);
				this.comboBoxStrategyFile.SelectedIndex = 0;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}

		//加载策略
		private void buttonLoadStrategy_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fbd = new OpenFileDialog())
			{
				if (fbd.ShowDialog() == DialogResult.OK)
				{
					LoadStrategyFile(fbd.FileName);

				}
			}
		}

		private void buttonClearFiles_Click(object sender, EventArgs e)
		{
			this.comboBoxStrategyFile.Items.Clear();
			this.comboBoxStrategyFile.Text = string.Empty;
			this.ComboBoxType.Items.Clear();
		}
	}
	public class ConfigFile
	{
		public string[] StrategyFiles { get; set; }
		public FollowConfig FloConfig { get; set; } = new FollowConfig();
	}

}
