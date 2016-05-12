using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Numeric = System.Decimal;

namespace HaiFeng
{

	/// <summary>
	/// </summary>
	public class StrategyData
	{
		/// <summary>
		/// 	行情序列，指针指向WorkSpace中的bars
		/// </summary>
		public Data Data { get; set; }

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
		public StrategyData(Data pData)
		{
			this.Data = pData;
			Operations = new List<OrderItem>();
		}

		#region 引用Data相关属性

		/// <summary>
		/// 	合约
		/// </summary>
		[Description("合约"), Category("设计")]
		public string InstrumentID { get { return this.Data.Instrument; } }

		/// <summary>
		/// 	周期数
		/// </summary>
		[Description("周期数"), Category("设计")]
		public int Interval { get { return this.Data.Interval; } }

		/// <summary>
		/// 	周期类型:0-tick(保留);1-秒(保留);2-分;3-时;4-天;5-周;6-月;7-年
		/// </summary>
		[Description("周期类型"), Category("设计")]
		public EnumIntervalType IntervalType { get { return this.Data.IntervalType; } }

		/// <summary>
		/// 	当前K线索引(由左向右从0开始)
		/// </summary>
		[Description("当前K线索引"), Category("数据")]
		public int CurrentBar { get { return this.Data.CurrentBar; } }

		/// <summary>
		/// 	实际行情(无数据时为UpdateTime == null)
		/// </summary>
		[Description("分笔数据"), Category("数据")]
		public Tick Tick { get { return this.Data.Tick; } }

		/// <summary>
		/// 	合约信息
		/// </summary>
		public InstrumentInfo InstrumentInfo { get { return this.Data.InstrumentInfo; } }

		/// <summary>
		/// 	时间(yyyyMMdd.HHmmss)
		/// </summary>
		public DataSeries D { get { return this.Data.D; } }

		/// <summary>
		/// 	最高价
		/// </summary>
		public DataSeries H { get { return this.Data.H; } }

		/// <summary>
		/// 	最低价
		/// </summary>
		public DataSeries L { get { return this.Data.L; } }

		/// <summary>
		/// 	开盘价
		/// </summary>
		public DataSeries O { get { return this.Data.O; } }

		/// <summary>
		/// 	收盘价
		/// </summary>
		public DataSeries C { get { return this.Data.C; } }

		/// <summary>
		/// 	成交量
		/// </summary>
		public DataSeries V { get { return this.Data.V; } }

		/// <summary>
		/// 	持仓量
		/// </summary>
		public DataSeries I { get { return this.Data.I; } }

		/// <summary>
		/// 	均价
		/// </summary>
		public DataSeries A { get { return this.Data.A; } }

		#endregion

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
					Date = this.Data[this.Data.CurrentBar].D,
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
					Date = this.Data[this.Data.CurrentBar].D,
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

			if ( _rtnOrder != null)
			{
				_rtnOrder(order, this.Data);
			}
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
		public Numeric BarsSinceEntryLong { get { return this.Data.CurrentBar - this.lastOrder.IndexEntryLong; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceEntryShort { get { return this.Data.CurrentBar - this.lastOrder.IndexEntryShort; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceLastEntryLong { get { return this.Data.CurrentBar - this.lastOrder.IndexLastEntryLong; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceLastEntryShort { get { return this.Data.CurrentBar - this.lastOrder.IndexLastEntryShort; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceExitLong { get { return this.Data.CurrentBar - this.lastOrder.IndexExitLong; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceExitShort { get { return this.Data.CurrentBar - this.lastOrder.IndexExitShort; } }

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
		public Numeric PositionProfitLong { get { return this.Data.Count == 0 ? 0 : ((this.C[0] - this.lastOrder.AvgEntryPriceLong) * this.lastOrder.PositionLong); } }

		/// <summary>
		/// 	当前持仓浮动盈亏:空
		/// </summary>
		[Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfitShort { get { return this.Data.Count == 0 ? 0 : ((this.lastOrder.AvgEntryPriceShort - this.C[0]) * this.lastOrder.PositionShort); } }

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
