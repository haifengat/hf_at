
#region

using System;

#endregion

/*
 * 
 * 作者:吴俊海		时间:2012/3/17 13:18:29
 * 
 * 文件名:  _StdDev
 * 
 * CLR版本: 4.0.30319.239
 * 
 */

namespace HaiFeng
{
	internal class StdDev : Indicator
	{
		private DataSeries sumSeries;

		public StdDev(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar < 1)
			{
				Value[0] = (0);
				this.sumSeries[0] = (Input[0]);
			}
			else
			{
				this.sumSeries[0] = (Input[0] + this.sumSeries[1] - (CurrentBar >= Period ? Input[Period] : 0));
				decimal avg = this.sumSeries[0]/Math.Min(CurrentBar + 1, Period);
				decimal sum = 0;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack >= 0; barsBack--)
				{
					sum += (Input[barsBack] - avg)*(Input[barsBack] - avg);
				}
				Value[0] = (decimal)(Math.Sqrt((double)sum/Math.Min(CurrentBar + 1, Period)));
			}
		}
	}
}
