#region
using System;

#endregion

namespace HaiFeng
{
	internal class Lowest : Indicator
	{
		private int lastBar;
		private decimal lastMin;
		private int runningBar;
		private decimal runningMin;
		private int thisBar;

		public Lowest(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				this.runningMin = Input[0];
				this.lastMin = Input[0];
				this.runningBar = 0;
				this.lastBar = 0;
				this.thisBar = 0;
				return;
			}
			if (CurrentBar - this.runningBar >= Period)
			{
				this.runningMin = decimal.MaxValue;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack > 0; barsBack--)
				{
					if (Input[barsBack] <= this.runningMin)
					{
						this.runningMin = Input[barsBack];
						this.runningBar = CurrentBar - barsBack;
					}
				}
			}
			if (this.thisBar != CurrentBar)
			{
				this.lastMin = this.runningMin;
				this.lastBar = this.runningBar;
				this.thisBar = CurrentBar;
			}
			if (Input[0] <= this.lastMin)
			{
				this.runningMin = Input[0];
				this.runningBar = CurrentBar;
			}
			else
			{
				this.runningMin = this.lastMin;
				this.runningBar = this.lastBar;
			}
			Value[0] = this.runningMin;
		}
	}
}
