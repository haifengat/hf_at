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
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace HaiFeng
{
	public partial class Plat : UserControl
	{

		public Plat(Trade pTrade, Quote pQuote)
		{
			_t = pTrade;
			_q = pQuote;
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

		private readonly Trade _t;
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
			this.ButtonDataSource.Click += ButtonDataSource_Click;
			this.buttonDataSourceTick.Click += ButtonDataSource_Click;
			this.buttonDataSourceReal.Click += ButtonDataSource_Click;
			this.DataGridViewStrategies.SelectionChanged += DataGridViewStrategies_SelectionChanged;
			this.DataGridViewStrategies.CellContentClick += DataGridViewStrategies_CellContentClick;
			this.DataGridViewStrategies.CellFormatting += DataGridViewStrategies_CellFormatting;

			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.SelectionBackColor = this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.BackColor;
			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.SelectionForeColor = this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.ForeColor;

			this.propertyGrid1.PropertyValueChanged += propertyGrid1_PropertyValueChanged;
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

			this.propertyGrid1.SelectedObject = stra;   //属性显示

			stra.OnRtnOrder += stra_OnRtnOrder;
			return rid;
		}

		//加载策略
		private void ButtonLoadStra_Click(object sender, EventArgs e)
		{
			if (this.DataGridViewStrategies.SelectedRows.Count == 0) return;

			if (_dataProcess.ProductInfo.Count == 0)
			{
				HFLog.LogInfo("读取配置信息(交易日历,交易时间段)...");
				_dataProcess.UpdateInfo(this.textBoxServer.Text, this.textBoxUser.Text, this.textBoxPwd.Text);  //更新信息
			}
			DataGridViewRow row = this.DataGridViewStrategies.SelectedRows[0];
			string name = (string)row.Cells["StraName"].Value;
			Strategy stra;
			if (!_dicStrategies.TryGetValue(name, out stra))
			{
				LogError($"无对应的策略:{name}");
				return;
			}

			//参数是否可修改
			this.propertyGrid1.Enabled = !this.propertyGrid1.Enabled;

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

			ProductInfo pi;
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
			{
				stra.Init(data);

				var ticks = ReadTick(inst, dtBegin, dtEnd);
				if (ticks == null)
				{
					//数据读取有误:恢复加载前状态

					return;
				}
				//参数是否可修改
				this.propertyGrid1.Enabled = false;
				//加载与tick加载同时处理
				row.Cells["Loaded"].Value = "已加载";
				foreach (MarketData tick in ticks)
				{
					data.OnTick(new Tick
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
					stra.Update();
				}
				//取当日tick加载:未加载当日数据
				if (stra.D[0] < int.Parse(_t.TradingDay))
				{
					//var ticks = _useCompress ? ReadTickEncrypt(inst, DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null), DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null)) : ReadTick(inst, DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null), DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null));
					var ticksReal = ReadTick(inst, dtEnd, dtEnd, true); //取当日tick数据
					if (ticksReal != null)
						foreach (MarketData tick in ticksReal)
						{
							data.OnTick(new Tick
							{
								AskPrice = (Numeric)tick.AskPrice,
								AskVolume = tick.AskVolume,
								AveragePrice = (Numeric)tick.AveragePrice,
								BidPrice = (Numeric)tick.BidPrice,
								BidVolume = tick.BidVolume,
								InstrumentID = tick.InstrumentID,
								LastPrice = (Numeric)tick.LastPrice,
								LowerLimitPrice = double.IsNaN(tick.LowerLimitPrice) ? Numeric.MinValue : (Numeric)tick.LowerLimitPrice,
								OpenInterest = (Numeric)tick.OpenInterest,
								UpdateMillisec = tick.UpdateMillisec,
								UpdateTime = tick.UpdateTime,
								UpperLimitPrice = double.IsNaN(tick.UpperLimitPrice) ? Numeric.MaxValue : (Numeric)tick.UpperLimitPrice,
								Volume = tick.Volume,
							});
							stra.Update();
						}
				}
			}
			else
			{
				List<Bar> bars = ReadDataMongo(inst, dtBegin, dtEnd);
				//if (this.tabControl1.SelectedTab == this.tabPageDB) //数据库
				//	bars = ReadDataMongo(inst, dtBegin, dtEnd);
				//else
				//	bars = _useCompress ? ReadDataEncrypt(inst, dtBegin, dtEnd) : ReadData(inst, data.Interval, data.IntervalType, dtBegin, dtEnd);
				// 取当日数据
				if (dtEnd > DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null))
				{
					var listReal = ReadMongoReal(inst);
					bars = (bars ?? new List<Bar>());
					bars.AddRange(listReal);
				}
				if (bars == null)
				{
					return;
				}
				if (data.Interval > 1)
					bars = GetUpperPeriod(bars, data.Interval, EnumIntervalType.Min);

				//参数是否可修改
				this.propertyGrid1.Enabled = false;
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

		//改变数据源
		private void ButtonDataSource_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog fbd = new FolderBrowserDialog())
			{
				if (fbd.ShowDialog() == DialogResult.OK)
				{
					if (sender == this.ButtonDataSource)
						this.TextBoxDataSourceK.Text = fbd.SelectedPath;
					else if (sender == this.buttonDataSourceTick)
						this.textBoxDataSourceTick.Text = fbd.SelectedPath;
					else
						this.textBoxDataSourceReal.Text = fbd.SelectedPath;
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
			this.propertyGrid1.Enabled = row.Cells["Loaded"].Value == null || !row.Cells["Loaded"].Value.Equals("已加载");

			if (_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out _curStrategy))
			{
				this.propertyGrid1.SelectedObject = _curStrategy;
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
		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			foreach (DataGridViewRow row in this.DataGridViewStrategies.Rows)
			{
				Strategy stra;
				if (_dicStrategies.TryGetValue((string)row.Cells["StraName"].Value, out stra) && stra.Equals(this.propertyGrid1.SelectedObject))
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
				DataPathK = this.TextBoxDataSourceK.Text,
				DataPathT = this.textBoxDataSourceTick.Text,
				DataPathR = this.textBoxDataSourceReal.Text,
				MongoServer = this.textBoxServer.Text,
				StrategyFiles = new string[this.comboBoxStrategyFile.Items.Count],
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
				this.TextBoxDataSourceK.Text = cfg.DataPathK;
				this.textBoxDataSourceTick.Text = cfg.DataPathT;
				this.textBoxDataSourceReal.Text = cfg.DataPathR;
				this.textBoxUser.Text = cfg.MongoUser;
				this.textBoxPwd.Text = cfg.MongoPwd;
				foreach (var file in cfg.StrategyFiles)
				{
					LoadStrategyFile(file);
				}
			}
			this.textBoxServer.Text = cfg.MongoServer;
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
			//实际委托
			if (_listOrderStra.IndexOf(pStrategy) >= 0)
			{
				//处理上期所平今操作
				InstrumentField instField;
				if (pOrderItem.Offset == Offset.Close)
				{
					int lots = pOrderItem.Lots;
					if (_t.DicInstrumentField.TryGetValue(pData.Instrument, out instField) && instField.ExchangeID == Exchange.SHFE)
					{
						PositionField posiField;
						if (_t.DicPositionField.TryGetValue(pData.Instrument + "_" + (pOrderItem.Dir == Direction.Buy ? "Sell" : "Buy"), out posiField))
						{
							if (posiField.TdPosition > 0)
							{
								int lot = Math.Min(posiField.TdPosition, lots);
								_t.ReqOrderInsert(pData.Instrument, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.CloseToday, (double)pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name));
								lots -= lot;
							}
							if (lots > 0)
							{
								_t.ReqOrderInsert(pData.Instrument, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Close, (double)pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name));
							}
						}
					}
					else
						_t.ReqOrderInsert(pData.Instrument, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Close, (double)pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name));
				}
				else
					_t.ReqOrderInsert(pData.Instrument, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Open, (double)pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name));
			}
		}

		private MongoDatabase GetDatabase(string dbname)
		{
			//var strconn = "mongodb://reader:reader2015@192.168.105.203:27017/?authenticationDatabase=admin";
			var serverAddress = this.textBoxServer.Text.IndexOf(':') >= 0 ? new MongoServerAddress(this.textBoxServer.Text.Split(':')[0], short.Parse(this.textBoxServer.Text.Split(':')[1])) : new MongoServerAddress(this.textBoxServer.Text);

			var sett = new MongoServerSettings
			{
				Server = serverAddress,
			};
			if (!string.IsNullOrEmpty(this.textBoxUser.Text))
				sett.Credentials = new[] { MongoCredential.CreateCredential("admin", this.textBoxUser.Text, this.textBoxPwd.Text) };
			var server = new MongoServer(sett);
			return server.GetDatabase(dbname);
		}

		public List<Bar> ReadDataMongo(string pInstrument, DateTime pBegin, DateTime pEnd)
		{
			//数据库连接字符串
			HFLog.LogInfo("read mongo data start ...");

			var db = GetDatabase("future_min");

			var col = db.GetCollection<BsonDocument>(pInstrument);
			//取从前1天21:00:00开始;结束日15:30:00结束
			var filter = Query.And(Query.GTE("_id", new BsonDateTime(pBegin.Date.AddDays(-1).Add(new TimeSpan(21, 0, 0)))), Query.LT("_id", new BsonDateTime(pEnd.Date.Add(new TimeSpan(15, 30, 0)))));
			var list = col.Find(filter).ToList();

			var bars = list.Select(x => new Bar
			{
				D = x["_id"].ToUniversalTime().ToLocalTime(),
				O = (decimal)x["Open"].AsDouble,
				H = (decimal)x["High"].AsDouble,
				L = (decimal)x["Low"].AsDouble,
				C = (decimal)x["Close"].AsDouble,
				V = x["Volume"].AsInt32,
				I = (decimal)x["OpenInterest"].AsDouble,
			}).ToList();
			HFLog.LogInfo($"read mongo data finish ({bars.Count()}).");
			return bars;
		}

		private List<Bar> ReadMongoReal(string pInstrument)
		{
			//数据库连接字符串
			HFLog.LogInfo("read mongo data start ...");

			var db = GetDatabase("future_real");
			var col = db.GetCollection<BsonDocument>(pInstrument);
			var list = col.FindAll();
			//无数据:返回
			//if (list.Fields == null) return new List<Bar>();

			var bars = list.Select(x => new Bar
			{
				D = x["Date"].ToUniversalTime().ToLocalTime(),
				O = (decimal)x["Open"].AsDouble,
				H = (decimal)x["High"].AsDouble,
				L = (decimal)x["Low"].AsDouble,
				C = (decimal)x["Close"].AsDouble,
				V = x["Volume"].AsInt32,
				I = (decimal)x["OpenInterest"].AsDouble,
			}).ToList();
			HFLog.LogInfo($"read mongo data finish ({bars.Count()}).");
			return bars;
		}

		#region 文本方式读取
		//读取Tick数据
		public List<MarketData> ReadTick(string pInstrument, DateTime pBegin, DateTime pEnd, bool pReal = false)
		{
			var bars = new List<MarketData>();
			var dataPath = new DirectoryInfo(this.textBoxDataSourceTick.Text + "\\" + pInstrument);
			if (pReal) //实时tick
				dataPath = new DirectoryInfo(this.textBoxDataSourceReal.Text + "\\" + pInstrument);

			if (!dataPath.Exists)
			{
				LogError("无Tick数据源!");
				return null;
			}
			foreach (FileInfo fi in dataPath.GetFiles())// new DirectoryInfo(dataPath.FullName).GetFiles(pInstrument + ".csv", SearchOption.AllDirectories).OrderBy(n => n.DirectoryName))
			{
				if (DateTime.ParseExact(fi.Name.Split('_', '.')[1], "yyyyMMdd", null) < pBegin)
					continue;
				if (DateTime.ParseExact(fi.Name.Split('_', '.')[1], "yyyyMMdd", null) > pEnd)
					break;
				foreach (string line in File.ReadAllLines(fi.FullName))
				{
					if (!char.IsDigit(line[0]))
						continue;
					//#InstrumentID,TradingDay,UpperLimitPrice,LowerLimitPrice,OpenPrice,ClosePrice,SettlementPrice,PreClosePrice,PreOpenInterest,PreSettlementPrice
					//#a1507,20140801,4636,4280,4469,4490,4483,4458,32,4458
					//#LastPrice,BidPrice1,BidVolume1,AskPrice1,AskVolume1,AveragePrice,HighestPrice,LowestPrice,OpenInterest,Turnover,Volume,UpdateTime,UpdateMillisec

					//LastPrice,BidPrice1,BidVolume1,AskPrice1,AskVolume1,AveragePrice,HighestPrice,LowestPrice,OpenInterest, Turnover,Volume,UpdateTime,UpdateMillisec,ActionDay,TradingDay

					string[] fields = line.Split(',');
					var tick = new MarketData
					{
						LastPrice = double.Parse(fields[0]),
						BidPrice = double.Parse(fields[1]),
						BidVolume = int.Parse(fields[2]),
						AskPrice = double.Parse(fields[3]),
						AskVolume = int.Parse(fields[4]),
						AveragePrice = double.Parse(fields[5]),
						OpenInterest = double.Parse(fields[8]),
						Volume = int.Parse(fields[10]),
						//UpdateTime = fields[13] + " " + fields[11],
						UpdateTime = fields[11].IndexOf(" ") > 0 ? fields[11].Split(' ')[1] : fields[11],
						UpdateMillisec = int.Parse(fields[12]),
						InstrumentID = pInstrument,
						LowerLimitPrice = double.NaN, //.Parse(fields[3]),
						UpperLimitPrice = double.NaN, //(fields[3]),
					};
					if (_dataProcess.FixTick(tick, fields[14]))
					{
						bars.Add(tick);
					}
				}
			}
			return bars;
		}

		//读取分钟/日数据
		public List<Bar> ReadData(string pInstrument, int pInterval, EnumIntervalType pIntervalType, DateTime pBegin, DateTime pEnd)
		{
			List<Bar> bars = new List<Bar>();

			if (pInstrument.EndsWith("000"))
			{
				FileInfo fi = new FileInfo(this.TextBoxDataSourceK.Text + "\\Future_" + (pIntervalType == EnumIntervalType.Day ? "Day" : "Min") + "\\000\\" + pInstrument + ".csv");
				if (!fi.Exists)
				{
					MessageBox.Show("数据源不正确!");
					return null;
				}

				foreach (string line in File.ReadAllLines(fi.FullName))
				{
					if (!char.IsDigit(line[0]))
						continue;

					// 时间(精确到分),开,高,低,收,量,仓,均价
					// 2010 /04/16 09:15,3497.2,3517.4,3497.2,3507.9,823,650
					string[] fields = line.Split(',');
					DateTime dt = DateTime.ParseExact(fields[0], "yyyy/MM/dd HH:mm", null);
					if (dt < pBegin)
						continue;
					if (dt >= pEnd)
						break;

					bars.Add(new Bar
					{
						D = dt,
						O = decimal.Parse(fields[1]),
						H = decimal.Parse(fields[2]),
						L = decimal.Parse(fields[3]),
						C = decimal.Parse(fields[4]),
						V = decimal.Parse(fields[5]),
						I = decimal.Parse(fields[6]),
						A = 0,
					});
				}
				if (pInterval > 1)
					bars = GetUpperPeriod(bars, pInterval, EnumIntervalType.Min);

				return bars;
			}

			var dataPath = new DirectoryInfo(this.TextBoxDataSourceK.Text + "\\Future_" + (pIntervalType == EnumIntervalType.Day ? "Day" : "Min") + "\\" + pInstrument);
			if (!dataPath.Exists)
			{
				MessageBox.Show("数据源不正确!");
				return null;
			}

			foreach (FileInfo fi in dataPath.GetFiles())// new DirectoryInfo(dataPath.FullName).GetDirectories().GetFiles(pInstrument + "_*.csv", SearchOption.AllDirectories).OrderBy(n => n.DirectoryName))
			{
				if (DateTime.ParseExact(fi.Name.Split('_', '.')[1], "yyyyMMdd", null) < pBegin)
					continue;
				if (DateTime.ParseExact(fi.Name.Split('_', '.')[1], "yyyyMMdd", null) >= pEnd)
					break;
				foreach (string line in File.ReadAllLines(fi.FullName))
				{
					if (!char.IsDigit(line[0]))
						continue;
					//Open,High,Low,Close,Vol,OI,Avg,TickTime,TradingDay
					string[] fields = line.Split(',');
					bars.Add(new Bar
					{
						D = DateTime.Parse(fields[7]),
						O = decimal.Parse(fields[0]),
						H = decimal.Parse(fields[1]),
						L = decimal.Parse(fields[2]),
						C = decimal.Parse(fields[3]),
						V = decimal.Parse(fields[4]),
						I = decimal.Parse(fields[5]),
						A = decimal.Parse(fields[6]),
					});
				}
				if (pInterval > 1)
					bars = GetUpperPeriod(bars, pInterval, EnumIntervalType.Min);

			}
			return bars;
		}
		#endregion

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
				if (v.D >= dtBegin
					&& v.D < dtNext)
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
	}
	public class ConfigFile
	{
		public string[] StrategyFiles { get; set; }
		public string DataPathK { get; set; }
		public string DataPathT { get; set; }
		public string DataPathR { get; set; } //实时数据
		public string MongoServer { get; set; } = "192.168.105.202";
		public string MongoUser { get; set; } = "";
		public string MongoPwd { get; set; } = "";
	}

}
