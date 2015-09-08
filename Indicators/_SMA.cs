#region
using System;

#endregion

namespace HaiFeng
{
	public class SMA : Indicator
	{
		public SMA(int period)
			: base(null, period)
		{
		}

		public SMA(DataSeries input, int period)
			: base(input, period)
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
				decimal last = Value[1] * Math.Min(CurrentBar, Period);
				if (CurrentBar >= Period)
				{
					Value[0] = (last + Input[0] - Input[Period]) / Math.Min(CurrentBar, Period);
				}
				else
				{
					Value[0] = (last + Input[0]) / (Math.Min(CurrentBar, Period) + 1);
				}
			}
		}
	}
}
