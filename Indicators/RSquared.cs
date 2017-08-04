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
	/// R-squared indicator
	/// </summary>
	public class RSquared : Indicator
	{
		private double myPeriod;
		private double priorSumXY;
		private double priorSumY;
		private double priorSumY2;
		private double sumX;
		private double sumX2;
		private double sumXY;
		private double sumY;
		private double sumY2;
		private double denominator;
		private double r;

		protected override void Init()
		{
			Period = 8;

			priorSumXY = priorSumY = priorSumY2 = sumX = sumXY = sumX2 = sumY2 = denominator = 0;
		}

		protected override void OnBarUpdate()
		{
			if (IsFirstTickOfBar)
			{
				priorSumXY = sumXY;
				priorSumY = sumY;
				priorSumY2 = sumY2;
				myPeriod = Math.Min(CurrentBar + 1, Period);
				sumX = myPeriod * (myPeriod + 1) * 0.5;
				sumX2 = sumX * (2 * myPeriod + 1) / 3;
			}

			double input0 = Input[0];
			double inputPeriod = Input[Math.Min(Period, CurrentBar)];

			sumXY = priorSumXY - (CurrentBar >= Period ? priorSumY : 0) + myPeriod * input0;
			sumY = priorSumY + input0 - (CurrentBar >= Period ? inputPeriod : 0);
			sumY2 = priorSumY2 + input0 * input0 - (CurrentBar >= Period ? inputPeriod * inputPeriod : 0);
			denominator = (myPeriod * sumX2 - sumX * sumX) * (myPeriod * sumY2 - sumY * sumY);
			r = denominator > 0 ? (myPeriod * sumXY - sumX * sumY) / Math.Sqrt(denominator) : 0;
			Value[0] = (r * r);
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
		private RSquared[] cacheRSquared;

		public RSquared RSquared(DataSeries input, int period)
		{
			if (cacheRSquared != null)
				for (int idx = 0; idx < cacheRSquared.Length; idx++)
					if (cacheRSquared[idx] != null && cacheRSquared[idx].Period == period && cacheRSquared[idx].EqualsInput(input))
						return cacheRSquared[idx];
			return CacheIndicator<RSquared>(new RSquared() { Period = period, Input = input }, ref cacheRSquared);
		}
	}

	public partial class Strategy
	{

		public RSquared RSquared(DataSeries input, int period)
		{
			return Indicator.RSquared(input, period);
		}
	}
}

#endregion
