
#region

using System;

#endregion

/*
 * 
 * 作者:吴俊海		时间:2012/3/17 12:55:19
 * 
 * 文件名:  _Aroon
 * 
 * CLR版本: 4.0.30319.239
 * 
 */

namespace HaiFeng
{
	internal class Aroon : Indicator
	{
		public DataSeries Down;

		public DataSeries Up;

		private decimal runningMax;
		private int runningMaxBar;
		private decimal runningMin;
		private int runningMinBar;

		public Aroon(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				this.Down[0] = (0);
				this.Up[0] = (0);
				this.runningMax = High[0];
				this.runningMin = Low[0];
				this.runningMaxBar = 0;
				this.runningMinBar = 0;
				return;
			}
			int back = Math.Min(Period, CurrentBar);
			if (CurrentBar - this.runningMaxBar >= Period)
			{
				this.runningMax = decimal.MinValue;
				for (int barsBack = back; barsBack > 0; barsBack--)
				{
					if (High[barsBack] >= this.runningMax)
					{
						this.runningMax = High[barsBack];
						this.runningMaxBar = CurrentBar - barsBack;
					}
				}
			}
			if (CurrentBar - this.runningMinBar >= Period)
			{
				this.runningMin = decimal.MaxValue;
				for (int barsBack = back; barsBack > 0; barsBack--)
				{
					if (Low[barsBack] <= this.runningMin)
					{
						this.runningMin = Low[barsBack];
						this.runningMinBar = CurrentBar - barsBack;
					}
				}
			}
			if (High[0] >= this.runningMax)
			{
				this.runningMax = High[0];
				this.runningMaxBar = CurrentBar;
			}
			if (Low[0] <= this.runningMin)
			{
				this.runningMin = Low[0];
				this.runningMinBar = CurrentBar;
			}
			this.Up[0] = (100*((decimal) (back - (CurrentBar - this.runningMaxBar))/back));
			this.Down[0] = (100*((decimal) (back - (CurrentBar - this.runningMinBar))/back));
		}
	}
}
