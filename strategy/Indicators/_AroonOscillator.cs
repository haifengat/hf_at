
#region

using System;

#endregion

/*
 * 
 * 作者:吴俊海		时间:2012/3/17 13:12:34
 * 
 * 文件名:  _AroonOscillator
 * 
 * CLR版本: 4.0.30319.239
 * 
 */

namespace HaiFeng
{
	internal class AroonOscillator : Indicator
	{
		public AroonOscillator(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value[0] = (0);
			}
			else
			{
				int back = Math.Min(Period, CurrentBar);
				int idxMax = -1;
				int idxMin = -1;
				decimal max = decimal.MinValue;
				decimal min = decimal.MaxValue;
				for (int idx = back; idx >= 0; idx--)
				{
					if (High[back - idx] >= max)
					{
						max = High[back - idx];
						idxMax = CurrentBar - back + idx;
					}
					if (Low[back - idx]  <= min)
					{
						min = Low[back - idx];
						idxMin = CurrentBar - back + idx;
					}
				}
				Value[0] = (100*((decimal) (back - (CurrentBar - idxMax))/back) - 100*((decimal) (back - (CurrentBar - idxMin))/back));
			}
		}
	}
}
