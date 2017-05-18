using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	class RB_02 : Strategy
	{
		public override void Initialize()
		{
		}

		[Parameter("压力线参数", "RB")]
		public int UpLine = 24;
		[Parameter("支持线参数", "RB")]
		public int DnLine = 24;

		[Parameter("手数", "交易参数")]
		public int Lots = 1;
		[Parameter("开始止赢(%)", "交易参数")]
		public decimal StopProfitStart = 5;
		[Parameter("回落止赢(%)", "交易参数")]
		public decimal StopProfit = 20;

		[Parameter("止损-多(%)", "交易参数")]
		public decimal StopLossLong = 3;
		[Parameter("止损-空(%)", "交易参数")]
		public decimal StopLossShort = 3;
		/*//------------------------------------------------------------------------
// 简称: RB_02
// 名称: RangeBreak
// 类别: 公式应用
// 类型: 用户应用
// 输出:
//------------------------------------------------------------------------

Params
    Numeric UpLine(24);
    Numeric DnLine(24);
	
	Numeric StopProfitStart(5);
	Numeric StopProfit(20);
	
	Numeric StopLossLong(6);
	Numeric StopLossShort(3);
Vars
	Bool UpBreak;
	Bool DnBreak;
	Numeric UpValue;
	Numeric DnValue;
	
	Numeric stopLine;
	
	
	If(BarsSinceEntry > 0)
	{
		if(MarketPosition == 1)
		{
			//止盈
			If(Highest(H, BarsSinceEntry) >= EntryPrice * (1+StopProfitStart/100))
			{
				stopLine = EntryPrice + (Highest(H, BarsSinceEntry) - EntryPrice) * (1-StopProfit/100); //回落止盈
				Commentary("回落止盈");
			}
			If(L[0] <= EntryPrice * (1-StopLossLong/100))
			{
				stopLine = EntryPrice * (1-StopLossLong/100);
				Commentary("止损");
			}
			//止损
			If(L[0] <= stopLine)
				Sell(0, Min(O[0], stopLine));
		}
		else if(MarketPosition == -1)
		{
			//止盈
			If(Lowest(L, BarsSinceEntry) <= EntryPrice * (1-StopProfitStart/100))
			{
				stopLine = EntryPrice - (EntryPrice - Lowest(L, BarsSinceEntry)) * (1-StopProfit/100); //回落止盈
				Commentary("回落止盈");
			}
			If(H[0] >= EntryPrice * (1+StopLossShort/100))
			{
				stopLine = EntryPrice * (1+StopLossShort/100);
				Commentary("止损");
			}
			//止盈
			If(L[0] <= stopLine)
				Sell(0, Min(O[0], stopLine));
		}
	}
End
*/
		public override void OnBarUpdate()
		{
			if (CurrentBar < Max(UpLine, DnLine)) return;

			var UpValue = Highest(H, UpLine, 1);
			var DnValue = Lowest(L, DnLine, 1);

			var UpBreak = H[0] >= UpValue;
			var DnBreak = L[0] <= DnValue;

			if (PositionLong == 0 && UpBreak)
			{
				if (PositionShort > 0)
					BuyToCover(Lots, Max(O[0], UpValue));
				Buy(Lots, Max(O[0], UpValue));
			}
			else if (PositionShort == 0 && DnBreak)
			{
				if (PositionLong > 0)
					Sell(Lots, Min(O[0], DnValue));
				SellShort(Lots, Min(O[0], DnValue));
			}

			//止盈止损
			if (BarsSinceEntry == 0) return;

			decimal stopLine = 0;
			if (PositionLong > 0)
			{
				//止盈
				var h1 = Highest(H, BarsSinceEntryLong, 1);
				var remark = string.Empty;
				if (h1 >= EntryPriceLong * (1 + StopProfitStart / 100))
				{
					stopLine = EntryPriceLong + (h1 - EntryPriceLong) * (1 - StopProfit / 100);
					remark = "回落止赢";
				}
				if (L[0] <= EntryPriceLong * (1 - StopLossLong / 100))
				{
					stopLine = EntryPriceLong * (1 - StopLossLong / 100);
					remark = "止损";
				}
				if (stopLine != 0 && L[0] <= stopLine)
					Sell(0, Min(O[0], stopLine), remark);
			}
			if (PositionShort > 0)
			{
				//止盈
				var L1 = Lowest(L, BarsSinceEntryShort, 1);
				var remark = string.Empty;
				if (L1 <= EntryPriceShort * (1 - StopProfitStart / 100))
				{
					stopLine = EntryPriceShort - (EntryPriceShort - L1) * (1 - StopProfit / 100);
					remark = "回落止赢";
				}
				if (H[0] >= EntryPriceShort * (1 + StopLossShort / 100))
				{
					stopLine = EntryPriceShort * (1 + StopLossShort / 100);
					remark = "止损";
				}
				if (stopLine != 0 && H[0] >= stopLine)
					BuyToCover(0, Max(O[0], stopLine), remark);
			}
		}
	}
}
