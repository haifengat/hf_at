using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static HaiFeng.Functions;

namespace HaiFeng
{
	class RB_02 : Strategy
	{
		[Parameter("压力线参数", "RB")]
		public int UpLine = 24;
		[Parameter("支持线参数", "RB")]
		public int DnLine = 24;

		[Parameter("手数", "交易参数")]
		public int Lots = 5;
		[Parameter("开始止赢(%)", "交易参数")]
		public decimal StopProfitStart = 5;
		[Parameter("回落止赢(%)", "交易参数")]
		public decimal StopProfit = 20;

		[Parameter("止损-多(%)", "交易参数")]
		public decimal StopLossLong = 3;
		[Parameter("止损-空(%)", "交易参数")]
		public decimal StopLossShort = 3;


		Highest ht;
		Lowest lt;
		public override void Initialize()
		{
			ht = new Highest(H, UpLine);
			lt = new Lowest(L, DnLine);
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar < Max(UpLine, DnLine)) return;

			var UpValue = ht[1];
			var DnValue = lt[1];

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
			if (BarsSinceEntryLong == 0 && BarsSinceEntryShort == 0) return;

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
