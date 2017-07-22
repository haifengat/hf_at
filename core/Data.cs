using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Numeric = System.Decimal;

namespace HaiFeng
{

	/// <summary>
	/// </summary>
	public class Data : Collection<Bar>
	{
		internal OrderItem lastOrder
		{
			get
			{
				return Operations.Count > 0 ? Operations.Last() : new OrderItem();
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="pData"> </param>
		public Data()
		{
			this.Tick = new Tick();
			Operations = new List<OrderItem>();
		}

		/// <summary>
		/// 自定义集合变化事件
		/// </summary>
		/// <param name="pType"></param>
		/// <param name="pNew"></param>
		/// <param name="pOld"></param>
		public delegate void CollectionChange(int pType, object pNew, object pOld);

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

		#region 数据序列

		/// <summary>
		/// 	时间(yyyyMMdd.HHmmss)
		/// </summary>
		public DataSeries D = new DataSeries();

		/// <summary>
		/// 	开盘价
		/// </summary>
		public DataSeries O = new DataSeries();

		/// <summary>
		/// 	最高价
		/// </summary>
		public DataSeries H = new DataSeries();

		/// <summary>
		/// 	最低价
		/// </summary>
		public DataSeries L = new DataSeries();

		/// <summary>
		/// 	收盘价
		/// </summary>
		public DataSeries C = new DataSeries();

		/// <summary>
		/// 	成交量
		/// </summary>
		public DataSeries V = new DataSeries();

		/// <summary>
		/// 	持仓量
		/// </summary>
		public DataSeries I = new DataSeries();

		/// <summary>
		/// 	均价
		/// </summary>
		public DataSeries A = new DataSeries();
		#endregion

		#region Properties
		/// <summary>
		/// 	实际行情(无数据时为Instrument== null)
		/// </summary>
		[Description("分笔数据"), Category("数据"), Browsable(false)]
		public Tick Tick { get; set; }

		/// <summary>
		/// 	合约信息
		/// </summary>
		[Description("合约信息"), Category("数据"), Browsable(false)]
		public InstrumentInfo InstrumentInfo { get; set; }

		/// <summary>
		/// 	合约
		/// </summary>
		[Description("合约"), Category("配置")]
		public string Instrument { get; set; } = string.Empty;

		/// <summary>
		/// 	委托合约
		/// </summary>
		[Description("合约"), Category("配置")]
		public string InstrumentOrder { get; set; } = string.Empty;

		/// <summary>
		/// 	周期类型
		/// </summary>
		[Description("周期类型"), Category("配置")]
		public EnumIntervalType IntervalType { get; set; } = EnumIntervalType.Min;

		/// <summary>
		/// 	周期数
		/// </summary>
		[Description("周期数"), Category("配置")]
		public int Interval { get; set; } = 5;

		/// <summary>
		/// 	当前K线索引(由左向右从0开始)
		/// </summary>
		[Description("当前K线索引"), Category("设计"), Browsable(false)]
		public int CurrentBar { get { return Count == 0 ? 0 : (Count - 1); } }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public string Name { get { return this.Instrument + "_" + this.Interval + "_" + this.IntervalType; } }

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
			//foreach (var data in this.Datas.Where(n => n.Instrument == f.InstrumentID))
			{
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
				if (Count == 0) //无数据
				{
					Bar bar = new Bar
					{
						D = dtBegin,
						PreVol = f.Volume,
						I = f.OpenInterest,
						A = f.AveragePrice,
						V = 0 // kOld.preVol == 0 ? 0 : _tick.Volume - kOld.preVol;
					};
					bar.H = bar.L = bar.O = bar.C = f.LastPrice;
					Add(bar);
				}
				else
				{
					Bar bar = this[CurrentBar];
					//if (bar == null)	//特殊处理,未找到为null的原因
					//{
					//	RemoveLast();
					//}
					//bar = this[CurrentBar];
					if (bar.D == dtBegin) //在当前K线范围内
					{
						bar.H = Math.Max(bar.H, f.LastPrice);
						bar.L = Math.Min(bar.L, f.LastPrice);
						bar.C = f.LastPrice;
						bar.V = bar.V + f.Volume - bar.PreVol; //此处可能有问题
						bar.PreVol = f.Volume;
						bar.I = f.OpenInterest;
						bar.A = f.AveragePrice;

						this[CurrentBar] = bar; //更新
					}
					else if (dtBegin > bar.D)
					{
						Bar di = new Bar
						{
							D = dtBegin,
							//V = Math.Abs(bar.PreVol - 0) < 1E-06 ? 0 : f.Volume - bar.PreVol,
							V = f.Volume - bar.PreVol,
							PreVol = f.Volume,
							I = f.OpenInterest,
							A = f.AveragePrice,
							O = f.LastPrice,
							H = f.LastPrice,
							L = f.LastPrice,
							C = f.LastPrice
						};
						Add(di);
					}
				}
				Tick = f; //更新最后的tick
			}
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Bar item)
		{
			D.Add(Numeric.Parse(item.D.ToString("yyyyMMdd.HHmmss")));
			O.Add(item.O);
			H.Add(item.H);
			L.Add(item.L);
			C.Add(item.C);
			V.Add(item.V);
			I.Add(item.I);
			A.Add(item.A);
			base.InsertItem(index, item);
			if (_onChange != null)
			{
				_onChange(1, item, item);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, Bar item)
		{
			Bar old = this[index];
			H[CurrentBar - index] = item.H;
			L[CurrentBar - index] = item.L;
			C[CurrentBar - index] = item.C;
			V[CurrentBar - index] = item.V;
			I[CurrentBar - index] = item.I;
			A[CurrentBar - index] = item.A;
			base.SetItem(index, item);
			if (_onChange != null)
			{
				_onChange(0, old, item);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			Bar old = this[index];
			if (D.Count == Count)
			{
				D.RemoveAt(index);
				O.RemoveAt(index);
				H.RemoveAt(index);
				L.RemoveAt(index);
				C.RemoveAt(index);
				V.RemoveAt(index);
				I.RemoveAt(index);
				A.RemoveAt(index);
			}
			base.RemoveItem(index);
			if (_onChange != null)
			{
				_onChange(-1, old, old);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected override void ClearItems()
		{
			D.Clear();
			O.Clear();
			H.Clear();
			L.Clear();
			C.Clear();
			V.Clear();
			I.Clear();
			A.Clear();
			base.ClearItems();
		}
		

		/// <summary>
		/// 	报单操作
		/// </summary>
		[Description("报单操作列表"), Category("交易")]
		public List<OrderItem> Operations { get; private set; }

		/// <summary>
		/// 	开多仓：买开
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void Buy(int pLots, Numeric pPrice, string pRemark = "")
		{
			this.Order(Direction.Buy, Offset.Open, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平空仓：卖平
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void Sell(int pLots, Numeric pPrice, string pRemark = "")
		{
			this.Order(Direction.Sell, Offset.Close, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	开空仓：卖开
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void SellShort(int pLots, Numeric pPrice, string pRemark = "")
		{
			this.Order(Direction.Sell, Offset.Open, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平多仓：买平
		/// </summary>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		public void BuyToCover(int pLots, Numeric pPrice, string pRemark = "")
		{
			this.Order(Direction.Buy, Offset.Close, pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	报单
		/// </summary>
		/// <param name="pDirector"> </param>
		/// <param name="pOffset"> </param>
		/// <param name="pLots"> </param>
		/// <param name="pPrice"> </param>
		/// <param name="pRemark">注释</param>
		private void Order(Direction pDirector, Offset pOffset, int pLots, Numeric pPrice, string pRemark)
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
					EntryDateLong = this.lastOrder.EntryDateLong,
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
						order.EntryDateLong = this.D[0];
						order.EntryPriceLong = pPrice;
					}
					order.IndexLastEntryLong = this.CurrentBar;
					order.LastEntryDateLong = this.D[0];
					order.LastEntryPriceLong = pPrice;
					break;
				case "SellOpen":
					order.PositionShort += pLots;

					order.AvgEntryPriceShort = (this.lastOrder.PositionShort * this.lastOrder.AvgEntryPriceShort + pLots * pPrice) / order.PositionShort;
					if (this.lastOrder.PositionShort == 0)
					{
						order.IndexEntryShort = this.CurrentBar;
						order.EntryDateShort = this.D[0];
						order.EntryPriceShort = pPrice;
					}
					order.IndexLastEntryShort = this.CurrentBar;
					order.LastEntryDateShort = this.D[0];
					order.LastEntryPriceShort = pPrice;
					break;
				case "BuyClose":
					if ((pLots = Math.Min(this.PositionShort, pLots)) <= 0)
						return;

					order.PositionShort -= pLots;

					order.IndexExitShort = this.CurrentBar;
					order.ExitDateShort = this.D[0];
					order.ExitPriceShort = pPrice;
					break;
				case "SellClose":
					if ((pLots = Math.Min(this.PositionLong, pLots)) <= 0)
						return;

					order.PositionLong -= pLots;

					order.IndexExitLong = this.CurrentBar;
					order.ExitDateLong = this.D[0];
					order.ExitPriceLong = pPrice;
					break;
			}
			//this.lastOrder = order;

			this.Operations.Add(order);

			_rtnOrder?.Invoke(order, this);
		}


		internal delegate void RtnOrder(OrderItem pOrderItem, Data pData);

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
		/// 	当前持仓手数:多
		/// </summary>
		[Description("当前持仓手数:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionLong { get { return this.lastOrder.PositionLong; } }

		/// <summary>
		/// 	当前持仓手数:空
		/// </summary>
		[Description("当前持仓手数:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionShort { get { return this.lastOrder.PositionShort; } }

		/// <summary>
		/// 	当前持仓手数:净
		/// </summary>
		[Description("当前持仓手数:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionNet { get { return this.lastOrder.PositionLong - this.lastOrder.PositionShort; } }

		/// <summary>
		/// 	当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryDateLong { get { return this.lastOrder.EntryDateLong; } }

		/// <summary>
		/// 	当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryDateShort { get { return this.lastOrder.EntryDateShort; } }

		/// <summary>
		/// 	当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryDateLong { get { return this.lastOrder.LastEntryDateLong; } }

		/// <summary>
		/// 	当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryDateShort { get { return this.lastOrder.LastEntryDateShort; } }

		/// <summary>
		/// 	当前持仓首个建仓价格:多
		/// </summary>
		[Description("当前持仓首个建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryPriceLong { get { return this.lastOrder.EntryPriceLong; } }

		/// <summary>
		/// 	当前持仓首个建仓价格:空
		/// </summary>
		[Description("当前持仓首个建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryPriceShort { get { return this.lastOrder.EntryPriceShort; } }

		/// <summary>
		/// 	当前持仓最后建仓价格:多
		/// </summary>
		[Description("当前持仓最后建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryPriceLong { get { return this.lastOrder.LastEntryPriceLong; } }

		/// <summary>
		/// 	当前持仓最后建仓价格:空
		/// </summary>
		[Description("当前持仓最后建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryPriceShort { get { return this.lastOrder.LastEntryPriceShort; } }

		/// <summary>
		/// 	当前持仓平均建仓价格:多
		/// </summary>
		[Description("当前持仓平均建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric AvgEntryPriceLong { get { return this.lastOrder.AvgEntryPriceLong; } }

		/// <summary>
		/// 	当前持仓平均建仓价格:空
		/// </summary>
		[Description("当前持仓平均建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric AvgEntryPriceShort { get { return this.lastOrder.AvgEntryPriceShort; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceEntryLong { get { return this.CurrentBar - this.lastOrder.IndexEntryLong; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceEntryShort { get { return this.CurrentBar - this.lastOrder.IndexEntryShort; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceLastEntryLong { get { return this.CurrentBar - this.lastOrder.IndexLastEntryLong; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceLastEntryShort { get { return this.CurrentBar - this.lastOrder.IndexLastEntryShort; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceExitLong { get { return this.CurrentBar - this.lastOrder.IndexExitLong; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceExitShort { get { return this.CurrentBar - this.lastOrder.IndexExitShort; } }

		/// <summary>
		/// 	最近平仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("平仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitDateLong { get { return this.lastOrder.ExitDateLong; } }

		///<summary>
		///	最近平仓时间:空(yyyyMMdd.HHmmss)
		///</summary>
		[Description("平仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitDateShort { get { return this.lastOrder.ExitDateShort; } }

		/// <summary>
		/// 	最近平仓价格:多
		/// </summary>
		[Description("平仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitPriceLong { get { return this.lastOrder.ExitPriceLong; } }

		/// <summary>
		/// 	最近平仓价格:空
		/// </summary>
		[Description("平仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitPriceShort { get { return this.lastOrder.ExitPriceShort; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:多
		/// </summary>
		[Description("浮动盈亏:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfitLong { get { return this.Count == 0 ? 0 : ((this.C[0] - this.lastOrder.AvgEntryPriceLong) * this.lastOrder.PositionLong); } }

		/// <summary>
		/// 	当前持仓浮动盈亏:空
		/// </summary>
		[Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfitShort { get { return this.Count == 0 ? 0 : ((this.lastOrder.AvgEntryPriceShort - this.C[0]) * this.lastOrder.PositionShort); } }

		/// <summary>
		/// 	当前持仓浮动盈亏:净
		/// </summary>
		[Description("浮动盈亏:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfit { get { return this.PositionProfitLong + this.PositionProfitShort; } }

		//Numeric _maxMarginLong = 0, _maxMarginShort = 0;
		//private int _currentBar;
		//public Numeric MaxMarginShort { get { return this._maxMarginShort * this.InstrumentField.VolumeMultiple; } }

		//public Numeric ContractProfit//: 获得当前持仓位置的每手浮动盈亏。
		//{
		//}
		//public Numeric CurrentEntries//: 获得当前持仓的建仓次数。
		//{
		//}
		//public Numeric MaxContracts//: 获得当前持仓的最大持仓合约数。
		//{
		//}
		//public Numeric MaxEntries//: 获得最大的建仓次数。
		//{
		//}
		//public Numeric MaxPositionLoss//: 获得当前持仓的最大浮动亏损数。
		//{
		//}
		//public Numeric MaxPositionProfit//: 获得当前持仓的最大浮动盈利数。
		//{
		//}
		#endregion
	}
}
