using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class Data : Collection<Bar>
	{
		private Bar _oneMinBar = new Bar();

		internal OrderItem lastOrder
		{
			get
			{
				return Operations.Count > 0 ? Operations.Last() : new OrderItem();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Data()
		{
			this.Tick = new Tick();
			Operations = new List<OrderItem>();
		}

		private CollectionChange _onChange;
		/// <summary>
		/// 策略变化:加1;减-1;更新0
		/// </summary>
		public event CollectionChange OnChanged
		{
			add
			{
				_onChange += value;
			}
			remove
			{
				_onChange -= value;
			}
		}

		DataSeries _date = new DataSeries(), _time = new DataSeries(), _high = new DataSeries(), _low = new DataSeries(), _open = new DataSeries(), _close = new DataSeries(), _volume = new DataSeries(), _openinterest = new DataSeries();

		#region 数据序列
		/// <summary>
		/// 日期(yyyyMMdd)
		/// </summary>
		public DataSeries D { get => _date; }

		/// <summary>
		/// 时间(0.HHmmss)
		/// </summary>
		public DataSeries T { get => _time; }

		/// <summary>
		/// 开盘价
		/// </summary>
		public DataSeries O { get => _open; }

		/// <summary>
		/// 最高价
		/// </summary>
		public DataSeries H { get => _high; }

		/// <summary>
		/// 最低价
		/// </summary>
		public DataSeries L { get => _low; }

		/// <summary>
		/// 收盘价
		/// </summary>
		public DataSeries C { get => _close; }

		/// <summary>
		/// 成交量
		/// </summary>
		public DataSeries V { get => _volume; }

		/// <summary>
		/// 持仓量
		/// </summary>
		public DataSeries I { get => _openinterest; }

		/// <summary>
		/// 日期(yyyyMMdd)
		/// </summary>
		public DataSeries Date { get => D; }

		/// <summary>
		/// 时间(0.HHmmss)
		/// </summary>
		public DataSeries Time { get => T; }

		/// <summary>
		/// 开盘价
		/// </summary>
		public DataSeries Open { get => O; }

		/// <summary>
		/// 最高价
		/// </summary>
		public DataSeries High { get => H; }

		/// <summary>
		/// 最低价
		/// </summary>
		public DataSeries Low { get => L; }

		/// <summary>
		/// 收盘价
		/// </summary>
		public DataSeries Close { get => C; }

		/// <summary>
		/// 成交量
		/// </summary>
		public DataSeries Volume { get => V; }

		/// <summary>
		/// 持仓量
		/// </summary>
		public DataSeries OpenInterest { get => I; }
		#endregion

		#region Properties
		/// <summary>
		/// 实际行情(无数据时为Instrument== null)
		/// </summary>
		[Description("分笔数据"), Category("数据"), Browsable(false)]
		public Tick Tick { get; set; }

		/// <summary>
		/// 合约信息
		/// </summary>
		[Description("合约信息"), Category("数据"), Browsable(false)]
		public InstrumentInfo InstrumentInfo { get; set; }

		/// <summary>
		/// 最小变动
		/// </summary>
		[Description("最小变动"), Category("数据"), Browsable(false)]
		public double PriceTick { get { return InstrumentInfo.PriceTick; } }

		/// <summary>
		/// 合约
		/// </summary>
		[Description("合约"), Category("配置")]
		public string Instrument { get; set; } = string.Empty;

		/// <summary>
		/// 委托合约
		/// </summary>
		[Description("合约"), Category("配置")]
		public string InstrumentOrder { get; set; } = string.Empty;

		/// <summary>
		/// 周期类型
		/// </summary>
		[Description("周期类型"), Category("配置")]
		public EnumIntervalType IntervalType { get; set; } = EnumIntervalType.Min;

		/// <summary>
		/// 周期数
		/// </summary>
		[Description("周期数"), Category("配置")]
		public int Interval { get; set; } = 5;

		/// <summary>
		/// 当前K线索引(由左向右从0开始)
		/// </summary>
		[Description("当前K线索引"), Category("设计"), Browsable(false)]
		public int CurrentBar { get => Count == 0 ? -1 : (Count - 1); }

		/// <summary>
		/// 当前的1分钟K线
		/// </summary>
		public Bar CurrentMinBar { get => _oneMinBar; }
		#endregion

		/// <summary>
		/// 被tick行情调用
		/// </summary>
		/// <param name="f"></param>
		/// <exception cref="Exception"></exception>
		public void OnTick(Tick f)
		{
			if (this.Instrument != f.InstrumentID)
				return;

			#region 生成or更新K线
			DateTime dt = DateTime.ParseExact(f.UpdateTime, "yyyyMMdd HH:mm:ss", null);
			DateTime dtBegin = dt.Date;
			switch (IntervalType)
			{
				case EnumIntervalType.Sec:
					dtBegin = dtBegin.Date.AddHours(dt.Hour).AddMinutes(dt.Minute).AddSeconds(dt.Second / Interval * Interval);
					break;
				case EnumIntervalType.Min:
					dtBegin = dtBegin.Date.AddHours(dt.Hour).AddMinutes(dt.Minute / Interval * Interval);
					break;
				case EnumIntervalType.Hour:
					dtBegin = dtBegin.Date.AddHours(dt.Hour / Interval * Interval);
					break;
				case EnumIntervalType.Day:
					dtBegin = dtBegin.Date;
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
			if (_oneMinBar == null)
			{
				_oneMinBar = new Bar
				{
					D = DateTime.ParseExact(f.UpdateTime.Substring(0, f.UpdateTime.Length - 3), "yyyyMMdd HH:mm", null),
					TradingDay = f.TradingDay,
					PreVol = f.Volume,
					I = f.OpenInterest,
					V = 0
				};
				_oneMinBar.H = _oneMinBar.L = _oneMinBar.O = _oneMinBar.C = f.LastPrice;
			}
			else
			{
				if (_oneMinBar.D - dt < TimeSpan.FromMinutes(1))
				{
					_oneMinBar.H = Math.Max(_oneMinBar.H, f.LastPrice);
					_oneMinBar.L = Math.Min(_oneMinBar.L, f.LastPrice);
					_oneMinBar.C = f.LastPrice;
					_oneMinBar.V = _oneMinBar.V + f.Volume - _oneMinBar.PreVol;
					_oneMinBar.PreVol = f.Volume;       //逐个tick累加
					_oneMinBar.I = f.OpenInterest;
				}
				else
				{
					_oneMinBar.D = DateTime.ParseExact(f.UpdateTime.Substring(0, f.UpdateTime.Length - 3), "yyyyMMdd HH:mm", null);
					_oneMinBar.TradingDay = f.TradingDay;
					_oneMinBar.I = f.OpenInterest;
					_oneMinBar.V = f.Volume - _oneMinBar.PreVol;
					_oneMinBar.PreVol = f.Volume;
					_oneMinBar.H = _oneMinBar.L = _oneMinBar.O = _oneMinBar.C = f.LastPrice;
				}
			}
			if (Count == 0) //无数据
			{
				Bar bar = new Bar
				{
					D = dtBegin,
					TradingDay = f.TradingDay,
					PreVol = f.Volume,
					I = f.OpenInterest,
					V = 0 // kOld.preVol == 0 ? 0 : _tick.Volume - kOld.preVol;
				};
				bar.H = bar.L = bar.O = bar.C = f.LastPrice;
				Add(bar);
			}
			else
			{
				Bar bar = this[CurrentBar];
				if (bar.D == dtBegin) //在当前K线范围内
				{
					bar.H = Math.Max(bar.H, f.LastPrice);
					bar.L = Math.Min(bar.L, f.LastPrice);
					bar.C = f.LastPrice;
					bar.V = bar.V + f.Volume - bar.PreVol;
					bar.PreVol = f.Volume;      //逐个tick累加
					bar.I = f.OpenInterest;

					this[CurrentBar] = bar; //更新会与 _onChange?.Invoke(0, old, item); 连动
				}
				else if (dtBegin > bar.D)
				{
					Bar di = new Bar
					{
						D = dtBegin,
						TradingDay = f.TradingDay,
						//V = Math.Abs(bar.PreVol - 0) < 1E-06 ? 0 : f.Volume - bar.PreVol,
						V = f.Volume - bar.PreVol,
						PreVol = f.Volume,
						I = f.OpenInterest,
						O = f.LastPrice,
						H = f.LastPrice,
						L = f.LastPrice,
						C = f.LastPrice
					};
					Add(di);
				}
			}
			Tick = f; //更新最后的tick
			#endregion
		}

		/// <summary>
		/// 接收分钟测试数据
		/// </summary>
		/// <param name="min"></param>
		internal void OnUpdatePerMin(Bar min)
		{
			_oneMinBar.D = min.D;
			_oneMinBar.C = min.C;
			_oneMinBar.H = min.H;
			_oneMinBar.I = min.I;
			_oneMinBar.L = min.L;
			_oneMinBar.O = min.O;
			_oneMinBar.TradingDay = min.TradingDay;
			_oneMinBar.V = min.V;


			DateTime dtBegin = DateTime.MaxValue;
			switch (IntervalType)
			{
				case EnumIntervalType.Sec:
					dtBegin = min.D.Date.AddHours(min.D.Hour).AddMinutes(min.D.Minute).AddSeconds(min.D.Second / Interval * Interval);
					break;
				case EnumIntervalType.Min:
					dtBegin = min.D.Date.AddHours(min.D.Hour).AddMinutes(min.D.Minute / Interval * Interval);
					break;
				case EnumIntervalType.Hour:
					dtBegin = min.D.Date.AddHours(min.D.Hour / Interval * Interval);
					break;
				case EnumIntervalType.Day:
					dtBegin = DateTime.ParseExact(min.TradingDay.ToString(), "yyyyMMdd", null);
					break;
				case EnumIntervalType.Week:
					dtBegin = DateTime.ParseExact(min.TradingDay.ToString(), "yyyyMMdd", null);
					dtBegin = dtBegin.Date.AddDays(1 - (byte)dtBegin.DayOfWeek);
					break;
				case EnumIntervalType.Month:
					dtBegin = DateTime.ParseExact(min.TradingDay.ToString(), "yyyyMMdd", null);
					dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, 1);
					break;
				case EnumIntervalType.Year:
					dtBegin = DateTime.ParseExact(min.TradingDay.ToString(), "yyyyMMdd", null);
					dtBegin = new DateTime(dtBegin.Year, 1, 1);
					break;
				default:
					throw new Exception("参数错误");
			}
			if (Count == 0) //无数据
			{
				Bar bar = new Bar
				{
					D = dtBegin,
					PreVol = min.V,
					I = min.I,
					V = min.V, // kOld.preVol == 0 ? 0 : _tick.Volume - kOld.preVol;
				};
				bar.H = min.H;
				bar.L = min.L;
				bar.O = min.O;
				bar.C = min.C;
				Add(bar);
			}
			else
			{
				Bar bar = this[CurrentBar];
				if (bar.D == dtBegin) //在当前K线范围内
				{
					bar.H = Math.Max(bar.H, min.H);
					bar.L = Math.Min(bar.L, min.L);
					bar.V = bar.V + min.V;
					bar.I = min.I;
					bar.C = min.C;

					this[CurrentBar] = bar; //更新会与 _onChange?.Invoke(0, old, item); 连动
				}
				else if (dtBegin > bar.D)
				{
					Bar di = new Bar
					{
						D = dtBegin,
						V = min.V,
						I = min.I,
						O = min.O,
						H = min.H,
						L = min.L,
						C = min.C,
					};
					Add(di);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Bar item)
		{
			base.InsertItem(index, item);
			_date.Add(double.Parse(item.D.ToString("yyyyMMdd")));
			_time.Add(double.Parse(item.D.ToString("0.HHmmss")));

			_open.Add(item.O);
			_high.Add(item.H);
			_low.Add(item.L);
			_volume.Add(item.V);
			_openinterest.Add(item.I);
			_close.Add(item.C); //最后一项更新:用于触发指标相关执行

			_onChange?.Invoke(1, item, item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, Bar item)
		{
			base.SetItem(index, item);
			Bar old = this[index];
			_high[CurrentBar - index] = item.H;
			_low[CurrentBar - index] = item.L;
			_volume[CurrentBar - index] = item.V;
			_openinterest[CurrentBar - index] = item.I;
			_close[CurrentBar - index] = item.C; //最后一项更新:用于触发指标相关执行

			_onChange?.Invoke(0, old, item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			Bar old = this[index];
			if (_date.Count == Count)
			{
				_date.RemoveAt(index);
				_time.RemoveAt(index);
				_open.RemoveAt(index);
				_high.RemoveAt(index);
				_low.RemoveAt(index);
				_close.RemoveAt(index);
				_volume.RemoveAt(index);
				_openinterest.RemoveAt(index);
			}
			base.RemoveItem(index);
			_onChange?.Invoke(-1, old, old);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void ClearItems()
		{
			_date.Clear();
			_time.Clear();
			_open.Clear();
			_high.Clear();
			_low.Clear();
			_close.Clear();
			_volume.Clear();
			_openinterest.Clear();
			base.ClearItems();
		}


		/// <summary>
		/// 报单操作
		/// </summary>
		[Description("报单操作列表"), Category("交易")]
		public List<OrderItem> Operations { get; private set; }

		/// <summary>
		/// 开多仓：买开
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void Buy(int pLots, double pPrice, string pRemark = "")
		{
			this.Order(Direction.Buy, Offset.Open, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 平空仓：卖平
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void Sell(int pLots, double pPrice, string pRemark = "")
		{
			this.Order(Direction.Sell, Offset.Close, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 开空仓：卖开
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void SellShort(int pLots, double pPrice, string pRemark = "")
		{
			this.Order(Direction.Sell, Offset.Open, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 平多仓：买平
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void BuyToCover(int pLots, double pPrice, string pRemark = "")
		{
			this.Order(Direction.Buy, Offset.Close, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 报单
		/// </summary>
		/// <param name="pDirector"> </param>
		/// <param name="pOffset"> </param>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		private void Order(Direction pDirector, Offset pOffset, int pLots, double pPrice, string pRemark)
		{
			OrderItem order;

			if (this.Operations.Count == 0)
			{
				order = new OrderItem
				{
					Date = this[CurrentBar].D,
					Dir = pDirector,
					Offset = pOffset,
					Price = pPrice,
					Lots = pLots,
					Remark = pRemark
				};
			}
			else
			{
				order = new OrderItem
				{
					Date = this[this.CurrentBar].D,
					Dir = pDirector,
					Offset = pOffset,
					Price = pPrice,
					Lots = pLots,
					Remark = pRemark,

					IndexEntryLong = this.lastOrder.IndexEntryLong,
					IndexExitLong = this.lastOrder.IndexExitLong,
					IndexEntryShort = this.lastOrder.IndexEntryShort,
					IndexExitShort = this.lastOrder.IndexExitShort,
					IndexLastEntryLong = this.lastOrder.IndexLastEntryLong,
					IndexLastEntryShort = this.lastOrder.IndexLastEntryShort,

					AvgEntryPriceLong = this.lastOrder.AvgEntryPriceLong,
					AvgEntryPriceShort = this.lastOrder.AvgEntryPriceShort,
					PositionLong = this.lastOrder.PositionLong,
					PositionShort = this.lastOrder.PositionShort,
					EntryTimeLong = this.lastOrder.EntryTimeLong,
					EntryDateShort = this.lastOrder.EntryDateShort,
					EntryPriceLong = this.lastOrder.EntryPriceLong,
					EntryPriceShort = this.lastOrder.EntryPriceShort,
					ExitDateLong = this.lastOrder.ExitDateLong,
					ExitDateShort = this.lastOrder.ExitDateShort,
					ExitPriceLong = this.lastOrder.ExitPriceLong,
					ExitPriceShort = this.lastOrder.ExitPriceShort,
					LastEntryDateLong = this.lastOrder.LastEntryDateLong,
					LastEntryDateShort = this.lastOrder.LastEntryDateShort,
					LastEntryPriceLong = this.lastOrder.LastEntryPriceLong,
					LastEntryPriceShort = this.lastOrder.LastEntryPriceShort
				};
			}

			switch (string.Format("{0}{1}", pDirector, pOffset))
			{
				case "BuyOpen":
					order.PositionLong += pLots;

					order.AvgEntryPriceLong = (this.lastOrder.PositionLong * this.lastOrder.AvgEntryPriceLong + pLots * pPrice) / order.PositionLong;
					if (this.lastOrder.PositionLong == 0)
					{
						order.IndexEntryLong = this.CurrentBar;
						order.EntryTimeLong = this._date[0];
						order.EntryTimeLong = this._time[0];
						order.EntryPriceLong = pPrice;
					}
					order.IndexLastEntryLong = this.CurrentBar;
					order.LastEntryDateLong = this._date[0];
					order.LastEntryTimeLong = this._time[0];
					order.LastEntryPriceLong = pPrice;
					break;
				case "SellOpen":
					order.PositionShort += pLots;

					order.AvgEntryPriceShort = (this.lastOrder.PositionShort * this.lastOrder.AvgEntryPriceShort + pLots * pPrice) / order.PositionShort;
					if (this.lastOrder.PositionShort == 0)
					{
						order.IndexEntryShort = this.CurrentBar;
						order.EntryDateShort = this._date[0];
						order.EntryTimeShort = this._time[0];
						order.EntryPriceShort = pPrice;
					}
					order.IndexLastEntryShort = this.CurrentBar;
					order.LastEntryDateShort = this._date[0];
					order.LastEntryTimeShort = this._time[0];
					order.LastEntryPriceShort = pPrice;
					break;
				case "BuyClose":
					if ((pLots = Math.Min(this.PositionShort, pLots)) <= 0)
						return;

					order.PositionShort -= pLots;

					order.IndexExitShort = this.CurrentBar;
					order.ExitDateShort = this._date[0];
					order.ExitTimeShort = this._time[0];
					order.ExitPriceShort = pPrice;
					break;
				case "SellClose":
					if ((pLots = Math.Min(this.PositionLong, pLots)) <= 0)
						return;

					order.PositionLong -= pLots;

					order.IndexExitLong = this.CurrentBar;
					order.ExitDateLong = this._date[0];
					order.ExitTimeLong = this._time[0];
					order.ExitPriceLong = pPrice;
					break;
			}
			//this.lastOrder = order;

			this.Operations.Add(order);

			_rtnOrder?.Invoke(order, this);
		}


		private RtnOrder _rtnOrder;

		/// <summary>
		/// 
		/// </summary>
		internal event RtnOrder OnRtnOrder
		{
			add
			{
				_rtnOrder += value;
			}
			remove
			{
				_rtnOrder -= value;
			}
		}

		#region 策略状态信息

		/// <summary>
		/// 当前持仓手数:多
		/// </summary>
		[Description("当前持仓手数:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionLong { get => this.lastOrder.PositionLong; }

		/// <summary>
		/// 当前持仓手数:空
		/// </summary>
		[Description("当前持仓手数:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionShort { get => this.lastOrder.PositionShort; }

		/// <summary>
		/// 当前持仓手数:净
		/// </summary>
		[Description("当前持仓手数:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionNet { get => this.lastOrder.PositionLong - this.lastOrder.PositionShort; }

		/// <summary>
		/// 当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryDateLong { get => this.lastOrder.EntryTimeLong; }

		/// <summary>
		/// 当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryDateShort { get => this.lastOrder.EntryDateShort; }

		/// <summary>
		/// 当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryDateLong { get => this.lastOrder.LastEntryDateLong; }

		/// <summary>
		/// 当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryDateShort { get => this.lastOrder.LastEntryDateShort; }

		/// <summary>
		/// 当前持仓首个建仓价格:多
		/// </summary>
		[Description("当前持仓首个建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryPriceLong { get => this.lastOrder.EntryPriceLong; }

		/// <summary>
		/// 当前持仓首个建仓价格:空
		/// </summary>
		[Description("当前持仓首个建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryPriceShort { get => this.lastOrder.EntryPriceShort; }

		/// <summary>
		/// 当前持仓最后建仓价格:多
		/// </summary>
		[Description("当前持仓最后建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryPriceLong { get => this.lastOrder.LastEntryPriceLong; }

		/// <summary>
		/// 当前持仓最后建仓价格:空
		/// </summary>
		[Description("当前持仓最后建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryPriceShort { get => this.lastOrder.LastEntryPriceShort; }

		/// <summary>
		/// 当前持仓平均建仓价格:多
		/// </summary>
		[Description("当前持仓平均建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double AvgEntryPriceLong { get => this.lastOrder.AvgEntryPriceLong; }

		/// <summary>
		/// 当前持仓平均建仓价格:空
		/// </summary>
		[Description("当前持仓平均建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double AvgEntryPriceShort { get => this.lastOrder.AvgEntryPriceShort; }

		/// <summary>
		/// 当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceEntryLong { get => this.CurrentBar - this.lastOrder.IndexEntryLong; }

		/// <summary>
		/// 当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceEntryShort { get => this.CurrentBar - this.lastOrder.IndexEntryShort; }

		/// <summary>
		/// 当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceLastEntryLong { get => this.CurrentBar - this.lastOrder.IndexLastEntryLong; }

		/// <summary>
		/// 当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceLastEntryShort { get => this.CurrentBar - this.lastOrder.IndexLastEntryShort; }

		/// <summary>
		/// 最近平仓位置到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceExitLong { get => this.CurrentBar - this.lastOrder.IndexExitLong; }

		/// <summary>
		/// 最近平仓位置到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceExitShort { get => this.CurrentBar - this.lastOrder.IndexExitShort; }

		/// <summary>
		/// 最近平仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("平仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitDateLong { get => this.lastOrder.ExitDateLong; }

		///<summary>
		///	最近平仓时间:空(yyyyMMdd.HHmmss)
		///</summary>
		[Description("平仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitDateShort { get => this.lastOrder.ExitDateShort; }

		/// <summary>
		/// 最近平仓价格:多
		/// </summary>
		[Description("平仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitPriceLong { get => this.lastOrder.ExitPriceLong; }

		/// <summary>
		/// 最近平仓价格:空
		/// </summary>
		[Description("平仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitPriceShort { get => this.lastOrder.ExitPriceShort; }

		/// <summary>
		/// 当前持仓浮动盈亏:多
		/// </summary>
		[Description("浮动盈亏:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double PositionProfitLong { get => this.Count == 0 ? 0 : ((this._close[0] - this.lastOrder.AvgEntryPriceLong) * this.lastOrder.PositionLong); }

		/// <summary>
		/// 当前持仓浮动盈亏:空
		/// </summary>
		[Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double PositionProfitShort { get => this.Count == 0 ? 0 : ((this.lastOrder.AvgEntryPriceShort - this._close[0]) * this.lastOrder.PositionShort); }

		/// <summary>
		/// 当前持仓浮动盈亏:净
		/// </summary>
		[Description("浮动盈亏:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double PositionProfit { get => this.PositionProfitLong + this.PositionProfitShort; }

		//double _maxMarginLong = 0, _maxMarginShort = 0;
		//private int _currentBar;
		//public double MaxMarginShort { get =>this._maxMarginShort * this.InstrumentField.VolumeMultiple;}

		//public double ContractProfit//: 获得当前持仓位置的每手浮动盈亏。
		//{
		//}
		//public double CurrentEntries//: 获得当前持仓的建仓次数。
		//{
		//}
		//public double MaxContracts//: 获得当前持仓的最大持仓合约数。
		//{
		//}
		//public double MaxEntries//: 获得最大的建仓次数。
		//{
		//}
		//public double MaxPositionLoss//: 获得当前持仓的最大浮动亏损数。
		//{
		//}
		//public double MaxPositionProfit//: 获得当前持仓的最大浮动盈利数。
		//{
		//}
		#endregion
	}
}
