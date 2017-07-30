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

		private string[] fs;

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

		private List<string> _tradingDate;
		private bool _offline = false; //是否为脱机模式登录


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

		private void QuoteLogged()
		{
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
			Instrument instInfo;
			Product procInfo;
			if (!_dataProcess.InstrumentInfo.TryGetValue(inst, out instInfo) || !_dataProcess.ProductInfo.TryGetValue(instInfo.ProductID, out procInfo))
			{
				LogError("无合约对应的品种信息");
				return;
			}
			data.InstrumentInfo = new InstrumentInfo
			{
				InstrumentID = inst,
				ProductID = instInfo.ProductID,
				PriceTick = procInfo.PriceTick,
				VolumeMultiple = procInfo.VolumeTuple,
			};

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
					TradingDay = int.Parse(n.TradingDay),
					O = n.Open,
					H = n.High,
					L = n.Low,
					C = n.Close,
					V = n.Volume,
					I = n.OpenInterest,
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
							O = n.Open,
							H = n.High,
							L = n.Low,
							C = n.Close,
							V = n.Volume,
							I = n.OpenInterest
						}).ToList());
				}
			}
			else //取日线数据
				bars = _dataProcess.QueryDay(inst, dtBegin.ToString("yyyyMMdd"), qryEndDate.ToString("yyyyMMdd")).Select(n => new Bar
				{
					D = DateTime.ParseExact(n._id, "yyyyMMdd", null),
					O = n.Open,
					H = n.High,
					L = n.Low,
					C = n.Close,
					V = n.Volume,
					I = n.OpenInterest
				}).ToList();


			//if (bars.Count == 0)
			//{
			//	LogInfo($"{stra.Name,8},未取到历史数据.");
			//	return;
			//}

			
			//=>初始化策略/回测
			stra.Init(data);
			//加载与tick加载同时处理
			//foreach (Bar bar in bars)
			//	data.Add(bar); //加入bar
			stra.LoadHistory(Tuple.Create(data, bars));

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
			//此处已完成接口启动
			if (_t != null)
				_t.StartFollow(_q, _cfg.FloConfig);

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
					_t.ClosePosi(pData.InstrumentOrder, pOrderItem.Dir == Direction.Buy ? DirectionType.Sell : DirectionType.Buy, pOrderItem.Price, pOrderItem.Lots, int.Parse(pStrategy.Name) * 100);
				}
				else
					_t.ReqOrderInsert(pData.InstrumentOrder, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Open, pOrderItem.Price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name) * 100);
			}
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




		//登录
		private void ButtonLogin_Click(object sender, EventArgs e)
		{
			fs = ((string)this.comboBoxServer.SelectedValue).Split('|');
			LoginTrade(fs[2].Split(','), fs[1], this.textBoxUser.Text, this.textBoxPwd.Text);
		}

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
	}
}
