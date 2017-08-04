#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#endregion

//This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior 
	/// smoothing, and much less lag, over traditional SMA indicators.
	/// This indicator is based on the reference article found here:
	/// http://www.justdata.com.au/Journals/AlanHull/hull_ma.htm
	/// </summary>
	public class HMA : Indicator
	{
		private DataSeries diffSeries;
		private WMA wma1;
		private WMA wma2;
		private WMA wmaDiffSeries;

		protected override void Init()
		{
			Period = 14;

			diffSeries = new DataSeries(Input);
			wma1 = WMA(Input, (Period / 2));
			wma2 = WMA(Input, Period);
			wmaDiffSeries = WMA(diffSeries, (int)Math.Sqrt(Period));
		}

		protected override void OnBarUpdate()
		{
			diffSeries[0] = 2 * wma1[0] - wma2[0];
			Value[0] = wmaDiffSeries[0];
		}

		#region Properties
		[Range(2, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private HMA[] cacheHMA;

		public HMA HMA(DataSeries input, int period)
		{
			if (cacheHMA != null)
				for (int idx = 0; idx < cacheHMA.Length; idx++)
					if (cacheHMA[idx] != null && cacheHMA[idx].Period == period && cacheHMA[idx].EqualsInput(input))
						return cacheHMA[idx];
			return CacheIndicator<HMA>(new HMA() { Period = period, Input = input }, ref cacheHMA);
		}
	}

	public partial class Strategy
	{
		public HMA HMA(DataSeries input, int period)
		{
			return Indicator.HMA(input, period);
		}
	}
}

#endregion
