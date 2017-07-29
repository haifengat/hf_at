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
	/// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average 
	/// value of a security's price over a period of time with special emphasis on the more recent 
	/// portions of the time period under analysis as opposed to the earlier.
	/// </summary>
	public class WMA : Indicator
	{
		private int myPeriod;
		private double priorSum;
		private double priorWsum;
		private double sum;
		private double wsum;

		protected override void Init()
		{
			Period = 14;

			priorSum = 0;
			priorWsum = 0;
			sum = 0;
			wsum = 0;
		}

		protected override void OnBarUpdate()
		{

			if (IsFirstTickOfBar)
			{
				priorWsum = wsum;
				priorSum = sum;
				myPeriod = Math.Min(CurrentBar + 1, Period);
			}

			wsum = priorWsum - (CurrentBar >= Period ? priorSum : 0) + myPeriod * Input[0];
			sum = priorSum + Input[0] - (CurrentBar >= Period ? Input[Period] : 0);
			Value[0] = wsum / (0.5 * myPeriod * (myPeriod + 1));
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}

	#region  generated code. Neither change nor remove.

	public partial class Indicator
	{
		private WMA[] cacheWMA;

		public WMA WMA(DataSeries input, int period)
		{
			if (cacheWMA != null)
				for (int idx = 0; idx < cacheWMA.Length; idx++)
					if (cacheWMA[idx] != null && cacheWMA[idx].Period == period && cacheWMA[idx].EqualsInput(input))
						return cacheWMA[idx];
			return CacheIndicator<WMA>(new WMA() { Period = period, Input = input }, ref cacheWMA);
		}
	}

	public partial class Strategy
	{
		public WMA WMA(DataSeries input, int period)
		{
			return indicator.WMA(input, period);
		}
	}
}

#endregion
