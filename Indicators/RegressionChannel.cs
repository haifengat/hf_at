#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

#endregion

//This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// 回归通道
	/// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
	/// </summary>
	public class RegressionChannel : Indicator
	{
		private DataSeries interceptSeries;
		private DataSeries slopeSeries;
		private DataSeries stdDeviationSeries;

		protected override void Init()
		{
			Period = 35;
			Width = 2;

			interceptSeries = new DataSeries(Input);
			slopeSeries = new DataSeries(Input);
			stdDeviationSeries = new DataSeries(Input);
		}

		protected override void OnBarUpdate()
		{
			// First we calculate the linear regression parameters

			double sumX = (double)Period * (Period - 1) * .5;
			double divisor = sumX * sumX -
								(double)Period * Period * (Period - 1) * (2 * Period - 1) / 6;
			double sumXY = 0;
			double sumY = 0;
			int barCount = Math.Min(Period, CurrentBar);

			for (int count = 0; count < barCount; count++)
			{
				sumXY += count * Input[count];
				sumY += Input[count];
			}

			if (divisor.ApproxCompare(0) == 0 && Period == 0) return;

			double slope = (Period * sumXY - sumX * sumY) / divisor;
			double intercept = (sumY - slope * sumX) / Period;

			slopeSeries[0] = slope;
			interceptSeries[0] = intercept;

			// Next we calculate the standard deviation of the 
			// residuals (vertical distances to the regression line).

			double sumResiduals = 0;

			for (int count = 0; count < barCount; count++)
			{
				double regressionValue = intercept + slope * (Period - 1 - count);
				double residual = Math.Abs(Input[count] - regressionValue);
				sumResiduals += residual;
			}

			double avgResiduals = sumResiduals / Math.Min(CurrentBar - 1, Period);

			sumResiduals = 0;
			for (int count = 0; count < barCount; count++)
			{
				double regressionValue = intercept + slope * (Period - 1 - count);
				double residual = Math.Abs(Input[count] - regressionValue);
				sumResiduals += (residual - avgResiduals) * (residual - avgResiduals);
			}

			double stdDeviation = Math.Sqrt(sumResiduals / Math.Min(CurrentBar + 1, Period));
			stdDeviationSeries[0] = stdDeviation;

			double middle = intercept + slope * (Period - 1);
			Middle[0] = CurrentBar == 0 ? Input[0] : middle;
			Upper[0] = stdDeviation.ApproxCompare(0) == 0 || Double.IsInfinity(stdDeviation) ? Input[0] : middle + stdDeviation * Width;
			Lower[0] = stdDeviation.ApproxCompare(0) == 0 || Double.IsInfinity(stdDeviation) ? Input[0] : middle - stdDeviation * Width;
		}

		#region Properties
		[Browsable(false)]
		public DataSeries Middle { get { return Values[0]; } }

		[Browsable(false)]
		public DataSeries Upper { get { return Values[1]; } }

		[Browsable(false)]
		public DataSeries Lower { get { return Values[2]; } }

		[Range(2, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }

		[Range(1, double.MaxValue)]
		[Parameter("Width")]
		public double Width { get; set; }
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private RegressionChannel[] cacheRegressionChannel;

		public RegressionChannel RegressionChannel(DataSeries input, int period, double width)
		{
			if (cacheRegressionChannel != null)
				for (int idx = 0; idx < cacheRegressionChannel.Length; idx++)
					if (cacheRegressionChannel[idx] != null && cacheRegressionChannel[idx].Period == period && cacheRegressionChannel[idx].Width == width && cacheRegressionChannel[idx].EqualsInput(input))
						return cacheRegressionChannel[idx];
			return CacheIndicator<RegressionChannel>(new RegressionChannel() { Period = period, Width = width, Input = input }, ref cacheRegressionChannel);
		}
	}

	public partial class Strategy
	{
		public RegressionChannel RegressionChannel(DataSeries input, int period, double width)
		{
			return Indicator.RegressionChannel(input, period, width);
		}
	}
}

#endregion
