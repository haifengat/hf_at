using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// Standard Deviation is a statistical measure of volatility. 
	/// Standard Deviation is typically used as a component of other indicators, 
	/// rather than as a stand-alone indicator. For example, Bollinger Bands are 
	/// calculated by adding a security's Standard Deviation to a moving average.
	/// </summary>
	public class StdDev : Indicator
	{
		private DataSeries sumSeries;

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			Period = 14;
			sumSeries = new DataSeries(this.Input);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1)
			{
				Value[0] = 0;
				sumSeries[0] = Input[0];
			}
			else
			{
				sumSeries[0] = Input[0] + sumSeries[1] - (CurrentBar >= Period ? Input[Period] : 0);
				double avg = sumSeries[0] / Math.Min(CurrentBar + 1, Period);
				double sum = 0;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack >= 0; barsBack--)
					sum += (Input[barsBack] - avg) * (Input[barsBack] - avg);

				Value[0] = Math.Sqrt(sum / Math.Min(CurrentBar + 1, Period));
			}
		}

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private StdDev[] cacheStdDev;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public StdDev StdDev(DataSeries input, int period)
		{
			if (cacheStdDev != null)
				for (int idx = 0; idx < cacheStdDev.Length; idx++)
					if (cacheStdDev[idx] != null && cacheStdDev[idx].Period == period && cacheStdDev[idx].EqualsInput(input))
						return cacheStdDev[idx];
			return CacheIndicator<StdDev>(new StdDev() { Period = period, Input = input }, ref cacheStdDev);
		}
	}
	public partial class Strategy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public StdDev StdDev(DataSeries input, int period)
		{
			return Indicator.StdDev(input, period);
		}
	}
}

#endregion
