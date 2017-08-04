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
	/// The ROC (Rate-of-Change) indicator displays the percent change between the current price and the price x-time periods ago.
	/// </summary>
	public class ROC : Indicator
	{
		protected override void Init()
		{
			Period = 14;
		}

		protected override void OnBarUpdate()
		{
			double inputPeriod = Input[Math.Min(CurrentBar, Period)];
			Value[0] = ((Input[0] - inputPeriod) / inputPeriod) * 100;
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private ROC[] cacheROC;

		public ROC ROC(DataSeries input, int period)
		{
			if (cacheROC != null)
				for (int idx = 0; idx < cacheROC.Length; idx++)
					if (cacheROC[idx] != null && cacheROC[idx].Period == period && cacheROC[idx].EqualsInput(input))
						return cacheROC[idx];
			return CacheIndicator<ROC>(new ROC() { Period = period, Input = input }, ref cacheROC);
		}
	}

	public partial class Strategy
	{
		public ROC ROC(DataSeries input, int period)
		{
			return Indicator.ROC(input, period);
		}
	}
}

#endregion
