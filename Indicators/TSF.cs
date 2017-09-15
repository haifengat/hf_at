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
	/// The TSF (Time Series Forecast) calculates probable future values for the price 
	/// by fitting a linear regression line over a given number of price bars and following
	///  that line forward into the future. A linear regression line is a straight line which
	///  is as close to all of the given price points as possible. Also see the Linear Regression indicator.
	/// </summary>
	public class TSF : Indicator
	{
		private double avg;
		private double divisor;
		private double intercept;
		private double myPeriod;
		private double priorSumXY;
		private double priorSumY;
		private double slope;
		private double sumX2;
		private double sumX;
		private double sumXY;
		private double sumY;
		private SUM sum;
		private DataSeries y;

		protected override void Init()
		{
			Period = 14;
			Forecast = 3;

			avg = divisor = intercept = myPeriod = priorSumXY = priorSumY = slope = sumX = sumX2 = sumY = sumXY = 0;

			sum = SUM(Input, Period);
			y = new DataSeries(Input);
		}

		protected override void OnBarUpdate()
		{
			if (IsFirstTickOfBar)
			{
				priorSumY = sumY;
				priorSumXY = sumXY;
				myPeriod = Math.Min(CurrentBar + 1, Period);
				sumX = myPeriod * (myPeriod - 1) * 0.5;
				sumX2 = myPeriod * (myPeriod + 1) * 0.5;
				divisor = myPeriod * (myPeriod + 1) * (2 * myPeriod + 1) / 6 - sumX2 * sumX2 / myPeriod;
			}

			double input0 = Input[0];
			sumXY = priorSumXY - (CurrentBar >= Period ? priorSumY : 0) + myPeriod * input0;
			sumY = priorSumY + input0 - (CurrentBar >= Period ? Input[Period] : 0);
			avg = sumY / myPeriod;
			slope = (sumXY - sumX2 * avg) / divisor;
			intercept = (sum[0] - slope * sumX) / myPeriod;
			Value[0] = CurrentBar == 0 ? input0 : intercept + slope * ((myPeriod - 1) + Forecast);
		}


		#region Properties
		[Range(-10, int.MaxValue)]
		[Parameter("Forecast")]
		public int Forecast { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private TSF[] cacheTSF;

		public TSF TSF(DataSeries input, int forecast, int period)
		{
			if (cacheTSF != null)
				for (int idx = 0; idx < cacheTSF.Length; idx++)
					if (cacheTSF[idx] != null && cacheTSF[idx].Forecast == forecast && cacheTSF[idx].Period == period && cacheTSF[idx].EqualsInput(input))
						return cacheTSF[idx];
			return CacheIndicator<TSF>(new TSF() { Forecast = forecast, Period = period, Input = input }, ref cacheTSF);
		}
	}

	public partial class Strategy
	{
		public TSF TSF(DataSeries input, int forecast, int period)
		{
			return indicator.TSF(input, forecast, period);
		}
	}
}

#endregion
