using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	class MACD_Cross : Strategy
	{
		private DataSeries diff;

		public override void Initialize()
		{
			diff = MACD(this.C, 12, 26, 9).Diff;
		}

		public override void OnBarUpdate()
		{
			if (diff[1] > 0 && diff[2] <= 0) //cross up
			{
				if (this.PositionShort > 0)
					BuyToCover(this.PositionShort, O[0]);
				Buy(1, O[0]);
			}
			else if (diff[1] < 0 && diff[2] >= 0) //cross dn
			{
				if (this.PositionLong > 0)
					Sell(PositionLong, O[0]);
				SellShort(1, O[0]);
			}
		}
	}
}
