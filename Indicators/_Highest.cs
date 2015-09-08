
#region

using System;

#endregion

/*
 * 
 * 作者:吴俊海		时间:2012/3/17 15:49:56
 * 
 * 文件名:  Lowest
 * 
 * CLR版本: 4.0.30319.239
 * 
 */

namespace HaiFeng
{
	internal class Highest : Indicator
	{
		private int lastBar;
		private decimal lastMax;
		private int runningBar;
		private decimal runningMax;
		private int thisBar;

		public Highest(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				this.runningMax = Input[0];
				this.lastMax = Input[0];
				this.runningBar = 0;
				this.lastBar = 0;
				this.thisBar = 0;
				return;
			}
			if (CurrentBar - this.runningBar >= Period)
			{
				this.runningMax = decimal.MinValue;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack > 0; barsBack--)
				{
					if (Input[barsBack] >= this.runningMax)
					{
						this.runningMax = Input[barsBack];
						this.runningBar = CurrentBar - barsBack;
					}
				}
			}
			if (this.thisBar != CurrentBar)
			{
				this.lastMax = this.runningMax;
				this.lastBar = this.runningBar;
				this.thisBar = CurrentBar;
			}
			if (Input[0] >= this.lastMax)
			{
				this.runningMax = Input[0];
				this.runningBar = CurrentBar;
			}
			else
			{
				this.runningMax = this.lastMax;
				this.runningBar = this.lastBar;
			}
			Value[0] = (this.runningMax);
		}
	}
}
