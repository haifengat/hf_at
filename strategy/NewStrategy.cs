using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	class NewStrategy : Strategy
	{
		[Parameter("交易参数", "下单手数")]
		int Lots = 5;
		
		MACD macd;

		public override void Initialize()
		{
			macd = this.MACD(this.C, 13, 26, 9);
		}

		public override void OnBarUpdate()
		{
			var macd_0 = this.MACD(this.C, 13, 26, 9);
			if (macd_0.Diff[0].ApproxCompare(macd.Diff[0]) > 0)
				return;
		}
	}
}
