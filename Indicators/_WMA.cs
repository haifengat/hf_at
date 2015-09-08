#region
using System;

#endregion

namespace HaiFeng
{
	internal class WMA : Indicator
	{
		public WMA(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value[0] = Input[0];
			}
			else
			{
				int back = Math.Min(Period - 1, CurrentBar);
				decimal val = 0;
				int weight = 0;
				for (int idx = back; idx >= 0; idx--)
				{
					val += (idx + 1)*Input[back - idx];
					weight += (idx + 1);
				}
				Value[0] = val/weight;
			}
		}
	}
}
