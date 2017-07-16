using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace HaiFeng
{
	class RB_01 : Strategy
	{
		[Parameter("压力线参数", "RB")]
		public int UpLine = 24;
		[Parameter("支持线参数", "RB")]
		public int DnLine = 24;

		[Parameter("手数", "交易参数")]
		public int Lots = 1;


		Highest ht;
		Lowest lt;

		public override void Initialize()
		{
			ht = new Highest(H, UpLine);
			lt = new Lowest(L, DnLine);
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar < Math.Max(UpLine, DnLine)) return;

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
		}
	}
}
