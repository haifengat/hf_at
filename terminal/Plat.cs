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
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;
using static HaiFeng.HFLog;
using Numeric = System.Decimal;
using DataCenter;

namespace HaiFeng
{
	public partial class Plat : UserControl
	{

		protected override void OnHandleDestroyed(EventArgs e)
		{
			SaveStrategy();
			SaveConfig();
			//解除事件绑定及现场清理
			_timer.Stop();
			if (_q != null)
				_q.OnRtnTick -= _q_OnRtnTick;
			base.OnHandleDestroyed(e);
		}

		private ConcurrentDictionary<string, Strategy> _dicStrategies = new ConcurrentDictionary<string, Strategy>();
		private readonly List<Strategy> _listOrderStra = new List<Strategy>();
		private readonly List<Strategy> _listOnTickStra = new List<Strategy>();

		//脱机
		private void Offline_Click(object sender, EventArgs e)
		{
			this.panelLogin.Enabled = false;
			_offline = true;
			this.toolTip1.SetToolTip(this.ComboBoxType, "策略文件(dll)放置在(./strategies)目录中.");
			this.toolTip1.Show("策略文件(dll)放置在(./strategies)目录中.", this.ComboBoxType, 6000);
			LogInfo("脱机模式启动平台");
			InitControls();
		}

		private void _t_OnRspUserLogout(object sender, IntEventArgs e)
		{
			LogDebug($"断开,{e.Value,4}");
			this.Invoke(new Action(() =>
			{
				this.pictureBox1.Image = Properties.Resources.Close;
				this.toolTip1.SetToolTip(this.pictureBox1, "停止");
				this.toolTip1.Show("停止.", this.pictureBox1, 6000);
			}));
		}

		void trade_OnRspUserLogin(object sender, IntEventArgs e)
		{
			if (e.Value == 0)
			{
				LogWarn("登录成功.");

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
					//foreach (var g in insts)
					//{
					//	MarketData f;
					//	if (_q.DicTick.TryGetValue(g, out f))
					//		_dicTick000[g] = new Tick
					//		{
					//			AskPrice = (Numeric)f.AskPrice,
					//			AskVolume = f.AskVolume,
					//			AveragePrice = (Numeric)f.AveragePrice,
					//			BidPrice = (Numeric)f.BidPrice,
					//			BidVolume = f.BidVolume,
					//			InstrumentID = f.InstrumentID,
					//			LastPrice = (Numeric)f.LastPrice,
					//			LowerLimitPrice = (Numeric)f.LowerLimitPrice,
					//			OpenInterest = (Numeric)f.OpenInterest,
					//			UpdateMillisec = f.UpdateMillisec,
					//			UpdateTime = f.UpdateTime,
					//			UpperLimitPrice = (Numeric)f.UpperLimitPrice,
					//			Volume = f.Volume,
					//		};
					//}
					return;
				}
			}
			_q.ReqSubscribeMarketData(inst);
		}

		private void QuoteLogged()
		{
			_t.StartFollow(_q);
			//未登录过(多次登录时不处理)
			if (this.panelLogin.Enabled)
			{
				//交易日历,需每年更新
				_dataProcess.UpdateInfo();
				_tradingDate = _dataProcess.TradeDates;
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

		// 初始化控件&绑定
		void InitControls()
		{
			//合约列表
			//var listInst = _t.DicInstrumentField.Keys.ToList();
			var listInst = _dataProcess.InstrumentInfo.Select(n => n.Key).ToList();
			listInst.Sort();
			this.comboBoxInst.Items.AddRange(listInst.ToArray());
			this.comboBoxInstOrder.Items.AddRange(listInst.ToArray());

			//时间周期
			this.comboBoxInterval.Items.AddRange(new[] { "Min 1", "Min 3", "Min 5", "Min 10", "Min 15", "Min 30", "Hour 1", "Day 1", "Week 1", "Year 1" });

			//设置表格
			_dtOrders.Columns.Add("ID", typeof(int));//增加编号列
			foreach (PropertyInfo v in typeof(OrderItem).GetProperties())
			{
				//有说明的属性才显示
				if (v.GetCustomAttributes(false).Any())
					_dtOrders.Columns.Add(v.Name, v.PropertyType);
			}
			//_dtOrders.PrimaryKey = new[] { _dtOrders.Columns[""] };

			LoadConfig();
			LoadStrategy();

			if (!_offline)
			{
				_timer.Tick += _timer_Tick;
				_timer.Start();
			}
			this.buttonAddStra.Click += ButtonAdd_Click;//添加策略
			this.buttonDel.Click += ButtonDel_Click;
			this.DataGridViewStrategies.SelectionChanged += DataGridViewStrategies_SelectionChanged;
			this.DataGridViewStrategies.CellContentClick += DataGridViewStrategies_CellContentClick;
			this.DataGridViewStrategies.CellFormatting += DataGridViewStrategies_CellFormatting;

			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.SelectionBackColor = this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.BackColor;
			this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.SelectionForeColor = this.DataGridViewStrategies.RowTemplate.DefaultCellStyle.ForeColor;

			//this.propertyGridFlo.SelectedObject = _cfg.FloConfig;	
			this.numericUpDownFirst.DataBindings.Add("Value", _cfg.FloConfig, "FirstAddTicks", false, DataSourceUpdateMode.OnPropertyChanged);
			this.numericUpDownWait.DataBindings.Add("Value", _cfg.FloConfig, "WaitSecondsAfterOrder", false, DataSourceUpdateMode.OnPropertyChanged);
			this.numericUpDownReorder.DataBindings.Add("Value", _cfg.FloConfig, "NotFirstAddticks", false, DataSourceUpdateMode.OnPropertyChanged);
			this.numericUpDownTimes.DataBindings.Add("Value", _cfg.FloConfig, "FollowTimes", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		//策略产生的买卖信号表
		private readonly DataTable _dtOrders = new DataTable();

		//数据处理功能模块
		private DataProcess _dataProcess = new DataProcess();

		//平台相关配置
		ConfigATP _cfg = null;

		private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer
		{
			Interval = 1000
		};

		private Strategy _curStrategy;
		private readonly ConcurrentDictionary<string, Tick> _dicTick000 = new ConcurrentDictionary<string, Tick>(); //用于处理000数据

		private List<string> _tradingDate;
		private bool _offline = false; //是否为脱机模式登录

		//添加策略
		void ButtonAdd_Click(object sender, EventArgs e)
		{
			if (this.ComboBoxType.SelectedIndex < 0) return;
			if (string.IsNullOrEmpty(this.comboBoxInst.Text)) return;
			if (this.comboBoxInterval.SelectedIndex < 0) return;

			Strategy stra = (Strategy)Activator.CreateInstance((Type)this.ComboBoxType.SelectedItem);

			//参数配置,20170307:弹出窗口
			using (FormParams fp = new FormParams())
			{
				//参数配置
				fp.propertyGrid1.SelectedObject = stra;
				fp.StartPosition = FormStartPosition.CenterParent;
				if (fp.ShowDialog(this) != DialogResult.OK) return;

				int rid = AddStra(stra, _dicStrategies.Count == 0 ? "1" : (_dicStrategies.Select(n => int.Parse(n.Key)).Max() + 1).ToString(), this.comboBoxInst.Text, this.comboBoxInstOrder.Text, this.comboBoxInterval.Text, this.dateTimePickerBegin.Value.Date, this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date : DateTime.MaxValue);

				//数据加载
				LoadStraData(rid, stra, this.comboBoxInterval.Text, this.comboBoxInst.Text, this.comboBoxInstOrder.Text, this.dateTimePickerBegin.Value.Date, this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date : DateTime.MaxValue);
			}
		}

		//删除按钮
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

		// 策略添加到表中,返回添加后的行号
		private int AddStra(Strategy stra, string pName, string pInstrument, string pInstrumentOrder, string pInterval, DateTime pBegin, DateTime? pEnd)
		{
			if (!_dicStrategies.TryAdd(pName, stra))
			{
				MessageBox.Show("名称不能重复", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return -1;
			}
			stra.Name = pName;
			int rid = this.DataGridViewStrategies.Rows.Add(stra.Name, stra.GetType(), stra.GetParams(), pInstrument, pInstrumentOrder, pInterval, pBegin, pEnd);

			this.DataGridViewStrategies.Rows[rid].Cells["Order"].Value = false;
			//if (pEnd.Date == DateTime.MaxValue.Date)
			this.DataGridViewStrategies.Rows[rid].Cells["EndDate"].Value = null;
			//this.DataGridViewStrategies.Rows[rid].Cells["BeginDate"].Value = DateTime.Today.AddMonths(-3);

			//20170307 this.propertyGridParams.SelectedObject = stra;   //属性显示

			if (!_offline)
				stra.OnRtnOrder += stra_OnRtnOrder;
			//鼠标提示信息:策略,参数...
			//表格右键开启停止提示???
			return rid;
		}

		//加载测试指定行的策略的
		private void LoadStraData(int rid, Strategy stra, string internalType, string inst, string order_inst, DateTime dtBegin, DateTime dtEnd)
		{
			this.DataGridViewStrategies.Rows[rid].Cells["Loaded"].Value = "加载中...";
			#region 加载数据进行测试
			Data data = new Data
			{
				Interval = int.Parse(internalType.Split(' ')[1]),
				IntervalType = (EnumIntervalType)Enum.Parse(typeof(EnumIntervalType), internalType.Split(' ')[0]),
				Instrument = inst,
				InstrumentOrder = order_inst,
			};

			//需处理成按合约取品种(期权规则与期货不同)
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

			//20170504从表行中取数据,而非界面上的数据.否则在"行"中点"加载"处理有误.
			//DateTime dtBegin = this.dateTimePickerBegin.Value;
			//DateTime dtEnd = this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date.AddDays(1) : DateTime.ParseExact(_t.TradingDay, "yyyyMMdd", null).AddDays(1);

			_dtOrders.Rows.Clear(); //清除委托列表

			this.DataGridViewStrategies.EndEdit();
			this.DataGridViewStrategies.Refresh();

			var qryEndDate = dtEnd == DateTime.MaxValue ? DateTime.Today.AddDays(7) : dtEnd.AddDays(1);
			List<Bar> bars = null;
			if (data.IntervalType == EnumIntervalType.Min || data.IntervalType == EnumIntervalType.Hour)
			{
				bars = _dataProcess.QueryMin(inst, dtBegin.ToString("yyyyMMdd"), qryEndDate.ToString("yyyyMMdd")).Select(n => new Bar
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
				if (dtEnd == DateTime.MaxValue)
				{
					var listReal = _dataProcess.QueryReal(inst);
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
			else //取日线数据
				bars = _dataProcess.QueryDay(inst, dtBegin.ToString("yyyyMMdd"), qryEndDate.ToString("yyyyMMdd")).Select(n => new Bar
				{
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
				LogInfo($"{stra.Name,8},未取到历史数据.");
				return;
			}

			if (data.Interval > 1 || data.IntervalType != EnumIntervalType.Min)
				bars = GetUpperPeriod(bars, data.Interval, data.IntervalType);

			//加载与tick加载同时处理
			foreach (Bar bar in bars)
				data.Add(bar); //加入bar
							   //=>初始化策略/回测
			stra.Init(data);

			DataGridViewStrategies_SelectionChanged(null, null); //刷新委托列表
																 //是否有结束日期:只测试
																 //if (dtEnd == DateTime.MaxValue)

			//未设置结束日期=>可订阅并接收行情
			if (dtEnd == DateTime.MaxValue && _q != null)
			{
				//订阅000合约
				SubscribeInstrument(stra.InstrumentID);
				SubscribeInstrument(stra.Datas[0].InstrumentOrder);
				_listOnTickStra.Add(stra); //可以接收实际行情
			}
			LogInfo($"{stra.Name,8},策略加载数据完成.");
			#endregion
			this.DataGridViewStrategies.Rows[rid].Cells["Loaded"].Value = "已加载";

			if (!_offline)
			{
				var colIdx = this.DataGridViewStrategies.Columns.IndexOf(this.DataGridViewStrategies.Columns["Order"]);
				var rect = this.DataGridViewStrategies.GetCellDisplayRectangle(colIdx, rid, false);
				this.toolTip1.Show("勾选'委托',对接口下单.", this.DataGridViewStrategies, rect.X + 30, rect.Y + 20, 3000);
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
				decimal val;
				DateTime dt;
				if (decimal.TryParse((string)e.Value, out val) && DateTime.TryParseExact(val.ToString("00000000.0000"), "yyyyMMdd.HHmm", null, DateTimeStyles.None, out dt))
					e.Value = dt.ToString("yyyy/MM/dd HH:mm");
			}
		}

		private void SaveConfig()
		{
			if (_cfg == null) return;
			File.WriteAllText("config.json", JsonConvert.SerializeObject(_cfg, Newtonsoft.Json.Formatting.Indented));
		}

		private void LoadConfig()
		{
			if (File.Exists("config.json"))
				_cfg = JsonConvert.DeserializeObject<ConfigATP>(File.ReadAllText("config.json"));
			else
				_cfg = new ConfigATP();

			//foreach (var file in _cfg.StrategyFiles)
			//20170307:读取指定目录策略文件
			Directory.CreateDirectory("./strategies");
			var files = new DirectoryInfo("./strategies").GetFiles("*.dll", SearchOption.AllDirectories).ToList();
			files.Add(new FileInfo("./at_strategy.dll")); //测试用
			foreach (var file in files)
			{
				//if (File.Exists(file))
				LoadStrategyFile(file.FullName);
			}

			if (_t != null)
				_t.FloConfig = _cfg.FloConfig;
		}

		void SaveStrategy()
		{
			List<StrategyConfig> list = new List<StrategyConfig>();
			//string str = "StraName,Type,Instrument,InstrumentOrder,Interval,BeginDate,EndDate,Param\r\n";
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
				//str += $"{stra.Name},{stra.GetType()},{row.Cells["Instrument"].Value},{row.Cells["InstrumentOrder"].Value},{row.Cells["Interval"].Value},{row.Cells["BeginDate"].Value},{row.Cells["EndDate"].Value},{row.Cells["Param"].Value}\r\n";
			}
			File.WriteAllText("strategies.cfg", JsonConvert.SerializeObject(list));
		}

		void LoadStrategy()
		{
			if (File.Exists("strategies.cfg"))
			{
				//string[] lines = File.ReadAllLines("strategies.txt");
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

		void _timer_Tick(object sender, EventArgs e)
		{
			//自动启停控制
			if (_t == null)
			{
				if (_tradingDate.IndexOf(DateTime.Today.ToString("yyyyMMdd")) < 0)
				{
					//LogError($"{DateTime.Today.ToString("yyyyMMdd")} 非交易日");
				}
				else
				{
					//如果时间在设定的开始时间的5分钟内则重启接口
					var now = DateTime.Now.TimeOfDay;
					if ((new[] { _cfg.OpenTime1.TimeOfDay, _cfg.OpenTime2.TimeOfDay }).Count(n => now > n && now < n.Add(TimeSpan.FromMinutes(5))) > 0)
					{
						LogInfo("接口隔夜启动");
						this.ButtonLogin_Click(null, null);
					}
				}
			}
			else if (_t.IsLogin)
			{//退出接口:1.全部结束  2.2:00:00后全部非交易状态
				if ((_t.DicExcStatus.Count(n => n.Value != ExchangeStatusType.Closed) == 0) || (DateTime.Now.TimeOfDay > TimeSpan.Parse("02:00:00") && DateTime.Now.TimeOfDay < TimeSpan.Parse("03:00:00") && _t.DicExcStatus.Count(n => n.Value == ExchangeStatusType.Trading) == 0))
				{
					Thread.Sleep(1000 * 5);
					if (_t != null)
					{
						_t.ReqUserLogout();
						_t = null;
					}
					if (_q != null) //行情未登出???
					{
						_q.ReqUserLogout();
						_q = null;
					}
					LogInfo("接口退出");
				}
				else
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
			}
		}

		//策略委托:
		void stra_OnRtnOrder(OrderItem pOrderItem, Data pData, Strategy pStrategy)
		{
			_dtOrders.Rows.Add(_dtOrders.Rows.Count + 1, pOrderItem.Date, pOrderItem.Dir, pOrderItem.Offset, pOrderItem.Price, pOrderItem.Lots, pOrderItem.Remark);
			//实际委托
			if (_listOrderStra.IndexOf(pStrategy) >= 0)
			{
				LogInfo($"{pStrategy.Name,-8}{pOrderItem.Date,20}{pOrderItem.Dir,4}{pOrderItem.Offset,6}{pOrderItem.Price,12:F2}{pOrderItem.Lots,4}{pOrderItem.Remark}");

				//处理上期所平今操作
				if (pOrderItem.Offset == Offset.Close)
				{
					int lots = pOrderItem.Lots;
					_t.ClosePosi(pData.InstrumentOrder, pOrderItem.Dir == Direction.Buy ? DirectionType.Sell : DirectionType.Buy, (double)pOrderItem.Price, pOrderItem.Lots, int.Parse(pStrategy.Name) * 100);
				}
				else
					_t.ReqOrderInsert(pData.InstrumentOrder, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Open, (double)pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name) * 100);
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
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}
	}
}
