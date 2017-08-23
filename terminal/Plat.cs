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
				_q.OnRtnTick -= quote_OnRtnTick;
			base.OnHandleDestroyed(e);
		}

		private ConcurrentDictionary<string, Strategy> _dicStrategies = new ConcurrentDictionary<string, Strategy>();

		private string[] fs;

		//数据处理功能模块
		private DataProcess _dataProcess = new DataProcess();

		//平台相关配置
		ConfigATP _cfg = null;

		private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer
		{
			Interval = 1000
		};

		private List<string> _tradingDate;
		private bool _offline = false; //是否为脱机模式登录
		private string _tradingDay;
		private List<string> _inst888 = null;

		// 初始化控件&绑定
		void InitControls()
		{
			//合约列表
			//var listInst = _t.DicInstrumentField.Keys.ToList();
			var listInst = _dataProcess.InstrumentInfo.Select(n => n.Key).ToList();
			listInst.Sort();
			this.comboBoxInst.Items.AddRange(listInst.ToArray());

			this.comboBoxInst.SelectedIndexChanged += ComboBoxInst_SelectedIndexChanged;
			for (int i = 0; i < listInst.Count; i++)
			{
				if (listInst[i].EndsWith("000"))
				{
					listInst.RemoveAt(i);
					i--;
				}
			}
			this.comboBoxInstOrder.Items.AddRange(listInst.ToArray());//添加没有000结尾的合约

			foreach (DataGridViewColumn col in this.DataGridViewStrategies.Columns)
				if (col.Name != "Order")
					col.ReadOnly = true;

			//时间周期
			this.comboBoxInterval.Items.AddRange(new[] { "Min 1", "Min 3", "Min 5", "Min 10", "Min 15", "Min 30", "Hour 1", "Day 1", "Week 1", "Year 1" });


			LoadConfig();
			LoadStrategy();

			//非主力合约作为委托合约,提醒
			_inst888 = _dataProcess.Instrument888.Values.ToList();
			string instpass = "";
			foreach (DataGridViewRow row in this.DataGridViewStrategies.Rows)
			{
				var instord = (string)row.Cells["InstrumentOrder"].Value;
				if (_inst888.IndexOf(instord) < 0)
					instpass += instord + ",";
			}
			if (!string.IsNullOrEmpty(instpass))
				MessageBox.Show($"{instpass.TrimEnd(',')} 非主力合约,请确认是否进行合约迁移.");

			if (!_offline)
			{
				_timer.Tick += _timer_Tick;
				_timer.Start();
			}
			this.buttonAddStra.Click += ButtonAdd_Click;//添加策略
			this.buttonTick.Click += ButtonTick_Click;
			this.buttonDel.Click += ButtonDel_Click;
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

		private void ComboBoxInst_SelectedIndexChanged(object sender, EventArgs e)
		{
			var inst = ((ComboBox)sender).Text;
			if (inst.EndsWith("000"))
			{
				string inst888;
				if (_dataProcess.Instrument888.TryGetValue(inst.TrimEnd('0'), out inst888))
					this.comboBoxInstOrder.Text = inst888;
			}
			else
				this.comboBoxInstOrder.Text = inst;
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
			this.buttonTick.Visible = true;
			this.DataGridViewStrategies.Columns["TickLoad"].Visible = true;
			this.DataGridViewStrategies.Columns["Order"].Visible = false;
			_offline = true;
			this.toolTip1.SetToolTip(this.ComboBoxType, "策略文件(dll)放置在(./strategies)目录中.");
			this.toolTip1.Show("策略文件(dll)放置在(./strategies)目录中.", this.ComboBoxType, 6000);
			LogWarn("脱机模式启动平台");
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
			if (stra.GetParams().Trim('(', ')').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length >= 1)
				using (FormParams fp = new FormParams())
				{
					//参数配置
					fp.propertyGrid1.SelectedObject = stra;
					fp.StartPosition = FormStartPosition.CenterParent;
					if (fp.ShowDialog(this) != DialogResult.OK) return;
				}
			int rid = AddStra(stra, _dicStrategies.Count == 0 ? "1" : (_dicStrategies.Select(n => int.Parse(n.Key)).Max() + 1).ToString(), this.comboBoxInst.Text, this.comboBoxInstOrder.Text, this.comboBoxInterval.Text, this.dateTimePickerBegin.Value.Date, this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date : DateTime.MaxValue);

			//数据加载
			LoadDataBar(rid);
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
				stra.EnableOrder = false;
				stra.EnableTick = false;
			}
			this.DataGridViewStrategies.Rows.Remove(row);
		}

		//tick测试按钮
		private void ButtonTick_Click(object sender, EventArgs e)
		{
			if (this.ComboBoxType.SelectedIndex < 0) return;
			if (string.IsNullOrEmpty(this.comboBoxInst.Text)) return;
			if (this.comboBoxInterval.SelectedIndex < 0) return;

			Strategy stra = (Strategy)Activator.CreateInstance((Type)this.ComboBoxType.SelectedItem);

			//参数配置,20170307:弹出窗口
			if (stra.GetParams().Split(',').Length > 1)
				using (FormParams fp = new FormParams())
				{
					//参数配置
					fp.propertyGrid1.SelectedObject = stra;
					fp.StartPosition = FormStartPosition.CenterParent;
					if (fp.ShowDialog(this) != DialogResult.OK) return;
				}
			int rid = AddStra(stra, _dicStrategies.Count == 0 ? "1" : (_dicStrategies.Select(n => int.Parse(n.Key)).Max() + 1).ToString(), this.comboBoxInst.Text, this.comboBoxInstOrder.Text, this.comboBoxInterval.Text, this.dateTimePickerBegin.Value.Date, this.dateTimePickerEnd.Checked ? this.dateTimePickerEnd.Value.Date : DateTime.MaxValue);
			LoadDataTick(rid);
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
					TypeFullName = stra.GetType().FullName,
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
						if (t.FullName == sc.TypeFullName)
						{
							straType = t;
							break;
						}
					}
					if (straType == null)
						continue;
					Strategy stra = (Strategy)Activator.CreateInstance(straType);
					//参数赋值
					if (!string.IsNullOrEmpty(sc.Params.Trim('(', ')')))
						foreach (var v in sc.Params.Trim('(', ')').Split(','))
						{
							stra.SetParameterValue(v.Split(':')[0], v.Split(':')[1]);
						}

					int rid = AddStra(stra, sc.Name, sc.Instrument, sc.InstrumentOrder, sc.Interval, sc.BeginDate, sc.EndDate);

					LogInfo($"{stra.Name,8},读取策略 {(rid == -1 ? "出错" : "完成")}");
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
						stra.EnableOrder = true; //可以发送委托
					else
						stra.EnableOrder = false;
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
					if (stra.Datas.Count == 0)
						MessageBox.Show("策略无数据");
					else
						new FormWorkSpace(stra).Show();
				}
				else if (head == "Loaded") //加载
				{
					if (this.DataGridViewStrategies[e.ColumnIndex, e.RowIndex].Value == null || !this.DataGridViewStrategies[e.ColumnIndex, e.RowIndex].Value.Equals("已加载"))
						LoadDataBar(e.RowIndex);
				}
				else if (head == "TickLoad")
				{
					if (this.DataGridViewStrategies[e.ColumnIndex, e.RowIndex].Value == null || !this.DataGridViewStrategies[e.ColumnIndex, e.RowIndex].Value.Equals("已加载"))
						this.LoadDataTick(e.RowIndex);
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
			this.DataGridViewStrategies.Rows[rid].Cells["EndDate"].Value = null;

			if (!_offline)
				stra.OnRtnOrder += stra_OnRtnOrder;
			return rid;
		}

		//tick加载
		private void LoadDataTick(int rid)
		{
			DataGridViewRow row = this.DataGridViewStrategies.Rows[rid];

			var inst = row.Cells["Instrument"].Value.ToString();
			var instOrder = row.Cells["InstrumentOrder"].Value.ToString();
			var interval = row.Cells["Interval"].Value.ToString();
			var stra = _dicStrategies[row.Cells["StraName"].Value.ToString()];
			var begin = ((DateTime)row.Cells["BeginDate"].Value).ToString("yyyyMMdd");
			var end = row.Cells["EndDate"].Value == null ? DateTime.Today.ToString("yyyyMMdd") : ((DateTime)row.Cells["EndDate"].Value).ToString("yyyyMMdd");

			Data data = new Data
			{
				Interval = int.Parse(interval.Split(' ')[1]),
				IntervalType = (EnumIntervalType)Enum.Parse(typeof(EnumIntervalType), interval.Split(' ')[0]),
				Instrument = inst,
				InstrumentOrder = instOrder,
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
			this.DataGridViewStrategies.EndEdit();
			this.DataGridViewStrategies.Refresh();

			new Thread(() =>
			{
				row.Cells["TickLoad"].Value = "加载中...";

				//=>初始化策略/回测
				stra.Init(data);
				stra.EnableTick = true; //允许接收tick数据
				List<string> insts = new List<string>();    //需要处理的合约
				if (data.Instrument.EndsWith("000"))
				{
					//_dicTick000[data.Instrument] = new Tick { InstrumentID = data.Instrument };//对于xx000需要先有数据保存
					insts.AddRange(_dataProcess.InstrumentInfo.Where(n => n.Value.ProductID == _dataProcess.InstrumentInfo[data.Instrument].ProductID).Select(n => n.Key).ToArray());
				}
				else
					insts.Add(data.Instrument);

				foreach (var day in new DirectoryInfo("Y:\\_tick").GetFiles("*.csv").Select(n => n.Name.Replace(n.Extension, "")).OrderBy(n => n))
				{
					if (day.CompareTo(begin) < 0 || day.CompareTo(end) > 0) continue;

					_tradingDay = day;
					//var reader = new StreamReader($"y:\\_tick\\{day}.csv");
					//while (!string.IsNullOrEmpty(line = reader.ReadLine()))
					var list = new List<MarketData>();
					foreach (var line in File.ReadAllLines($"y:\\_tick\\{day}.csv"))
					{//ag1209,5929,2,5915,9,5929,8,19542,88935,20120814,08:59:00,500
						string[] fs = line.Split(',');
						if (insts.IndexOf(fs[0]) < 0) continue;

						list.Add(new MarketData
						{
							InstrumentID = fs[0],
							AskPrice = double.Parse(fs[1]),
							AskVolume = int.Parse(fs[2]),
							BidPrice = double.Parse(fs[3]),
							BidVolume = int.Parse(fs[4]),
							LastPrice = double.Parse(fs[5]),
							Volume = int.Parse(fs[6]),
							OpenInterest = double.Parse(fs[7]),
							AveragePrice = double.Parse(fs[8]),
							UpdateTime = $"{fs[9]} {fs[10]}",//模仿原始tick不加日期 $"{fs[9]} {fs[10]}",
							UpdateMillisec = int.Parse(fs[11]),
						});
					}
					list = list.OrderBy(n => n.UpdateTime).ToList();
					int i = 0;
					foreach (var tick in list)
					{
						tick.UpdateTime = tick.UpdateTime.Split(' ')[1];
						quote_OnRtnTick(null, new TickEventArgs { Tick = tick });
						row.Cells["TickLoad"].Value = $"{++i}/{list.Count}";
						Thread.Sleep(1);   //行情太快时由于多线程处理,造成bug
					}
				}
				LogInfo($"{stra.Name,8},策略加载数据完成.");
				this.DataGridViewStrategies.Rows[rid].Cells["TickLoad"].Value = "已加载";
				this.DataGridViewStrategies.Rows[rid].Cells["Loaded"].Value = "已加载";
				this.DataGridViewStrategies.Rows[rid].ReadOnly = true;
			}).Start();
		}

		//加载测试指定行的策略的
		private void LoadDataBar(int rid)
		{
			DataGridViewRow row = this.DataGridViewStrategies.Rows[rid];

			var inst = row.Cells["Instrument"].Value.ToString();
			var instOrder = row.Cells["InstrumentOrder"].Value.ToString();
			var interval = row.Cells["Interval"].Value.ToString();
			var stra = _dicStrategies[row.Cells["StraName"].Value.ToString()];
			var begin = ((DateTime)row.Cells["BeginDate"].Value).ToString("yyyyMMdd");
			var end = row.Cells["EndDate"].Value == null ? DateTime.Today.AddDays(7).ToString("yyyyMMdd") : ((DateTime)row.Cells["EndDate"].Value).AddDays(1).ToString("yyyyMMdd");

			this.DataGridViewStrategies.Rows[rid].Cells["Loaded"].Value = "加载中...";

			Data data = new Data
			{
				Interval = int.Parse(interval.Split(' ')[1]),
				IntervalType = (EnumIntervalType)Enum.Parse(typeof(EnumIntervalType), interval.Split(' ')[0]),
				Instrument = inst,
				InstrumentOrder = instOrder,
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

			this.DataGridViewStrategies.EndEdit();
			this.DataGridViewStrategies.Refresh();

			List<Bar> bars = null;
			if (data.IntervalType == EnumIntervalType.Min || data.IntervalType == EnumIntervalType.Hour)
			{
				bars = _dataProcess.QueryMin(inst, begin, end).Select(n => new Bar
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
				if (row.Cells["EndDate"].Value == null)
				{
					var listReal = _dataProcess.QueryReal(inst);
					bars = (bars ?? new List<Bar>());
					if (listReal != null)
						bars.AddRange(listReal.Select(n => new Bar
						{
							D = DateTime.ParseExact(n._id, "yyyyMMdd HH:mm:ss", null),
							TradingDay = int.Parse(n.TradingDay),
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
				bars = _dataProcess.QueryDay(inst, begin, end).Select(n => new Bar
				{
					D = DateTime.ParseExact(n._id, "yyyyMMdd", null),
					O = n.Open,
					H = n.High,
					L = n.Low,
					C = n.Close,
					V = n.Volume,
					I = n.OpenInterest
				}).ToList();

			//=>初始化策略/回测
			stra.Init(data);
			stra.LoadHistory(Tuple.Create(data, bars));

			//未设置结束日期=>可订阅并接收行情
			if (row.Cells["EndDate"].Value == null && _q != null)
			{
				stra.EnableTick = true;
				foreach (var v in stra.Datas)
				{
					LogInfo($"行情订阅:{v.Instrument}");
					SubscribeInstrument(v.Instrument);
					SubscribeInstrument(v.InstrumentOrder);
				}
			}
			LogInfo($"{stra.Name,8},策略加载数据完成.");

			this.DataGridViewStrategies.Rows[rid].Cells["Loaded"].Value = "已加载";
			this.DataGridViewStrategies.Rows[rid].Cells["TickLoad"].Value = "已加载";

			if (!_offline)
			{
				var colIdx = this.DataGridViewStrategies.Columns.IndexOf(this.DataGridViewStrategies.Columns["Order"]);
				if (!(bool)this.DataGridViewStrategies[colIdx, rid].Value) //未被选中
				{
					var rect = this.DataGridViewStrategies.GetCellDisplayRectangle(colIdx, rid, false);
					this.toolTip1.Show("勾选'委托',对接口下单.", this.DataGridViewStrategies, rect.X + 30, rect.Y + 20, 3000);
				}
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
			//本执行文件中的策略
			LoadStrategyFile(Application.ExecutablePath);
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
			//实际委托
			if (pStrategy.EnableOrder)
			{
				LogInfo($"{pStrategy.Name,-8}{pOrderItem.Date,20}{pData.InstrumentOrder,8}{pOrderItem.Dir,4}{pOrderItem.Offset,6}{pOrderItem.Price,12:F2}{pOrderItem.Lots,4}{pOrderItem.Remark}");
				var price = pOrderItem.Price;

				//处理映射合约的价格
				if (pData.Instrument != pData.InstrumentOrder)
				{
					price = pOrderItem.Dir == Direction.Buy ? _q.DicTick[pData.InstrumentOrder].AskPrice : _q.DicTick[pData.InstrumentOrder].AskPrice;
				}

				//处理上期所平今操作
				if (pOrderItem.Offset == Offset.Close)
				{
					int lots = pOrderItem.Lots;
					_t.ClosePosi(pData.InstrumentOrder, pOrderItem.Dir == Direction.Buy ? DirectionType.Sell : DirectionType.Buy, price, pOrderItem.Lots, int.Parse(pStrategy.Name) * 100);
				}
				else
					_t.ReqOrderInsert(pData.InstrumentOrder, pOrderItem.Dir == Direction.Buy ? DirectionType.Buy : DirectionType.Sell, OffsetType.Open, price, pOrderItem.Lots, pCustom: int.Parse(pStrategy.Name) * 100);
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
	}
}
