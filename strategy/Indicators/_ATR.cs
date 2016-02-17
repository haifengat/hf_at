#region
using System;

#endregion

namespace HaiFeng
{
	internal class ATR : Indicator
	{
		public ATR(int period) : base(null, period)
		{
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value[0] = High[0] - Low[0];
			}
			else
			{
				decimal trueRange = High[0] - Low[0];
				trueRange = Math.Max(Math.Abs(Low[0] - Close[1]), Math.Max(trueRange, Math.Abs(High[0] - Close[1])));
				Value[0] = ((Math.Min(CurrentBar + 1, Period) - 1)*Value[1] + trueRange)/Math.Min(CurrentBar + 1, Period);
			}
		}
	}
}
