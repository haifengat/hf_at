#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator,
	///  the Aroon Oscillator measures the strength of a trend.
	/// </summary>
	public class AroonOscillator : Indicator
	{
		protected override void Init()
		{
			Period = 14;
			High = Inputs[0];
			Low = Inputs[1];
		}

		DataSeries High, Low;

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value[0] = 0;
			else
			{
				int back = Math.Min(Period, CurrentBar);
				int idxMax = -1;
				int idxMin = -1;
				double max = double.MinValue;
				double min = double.MaxValue;

				for (int idx = back; idx >= 0; idx--)
				{
					if (High[back - idx].ApproxCompare(max) >= 0)
					{
						max = High[back - idx];
						idxMax = CurrentBar - back + idx;
					}

					if (Low[back - idx].ApproxCompare(min) <= 0)
					{
						min = Low[back - idx];
						idxMin = CurrentBar - back + idx;
					}
				}

				Value[0] = 100 * ((double)(back - (CurrentBar - idxMax)) / back) - 100 * ((double)(back - (CurrentBar - idxMin)) / back);
			}
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}
	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private AroonOscillator[] cacheAroonOscillator;

		public AroonOscillator AroonOscillator(DataSeries high, DataSeries low, int period)
		{
			if (cacheAroonOscillator != null)
				for (int idx = 0; idx < cacheAroonOscillator.Length; idx++)
					if (cacheAroonOscillator[idx] != null && cacheAroonOscillator[idx].Period == period && cacheAroonOscillator[idx].EqualsInput(high, low))
						return cacheAroonOscillator[idx];
			return CacheIndicator<AroonOscillator>(new AroonOscillator() { Period = period, Inputs = new[] { high, low } }, ref cacheAroonOscillator);
		}
	}

	public partial class Strategy
	{
		public AroonOscillator AroonOscillator(int period)
		{
			return indicator.AroonOscillator(H, L, period);
		}
	}
}

#endregion
