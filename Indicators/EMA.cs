
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// Exponential Moving Average. The Exponential Moving Average is an indicator that
	/// shows the average value of a security's price over a period of time. When calculating
	/// a moving average. The EMA applies more weight to recent prices than the SMA.
	/// </summary>
	public class EMA : Indicator
	{
		private double constant1;
		private double constant2;

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			Period = 14;
			constant1 = 2.0 / (1 + Period);
			constant2 = 1 - (2.0 / (1 + Period));
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value[0] = (CurrentBar == 0 ? Input[0] : Input[0] * constant1 + constant2 * Value[1]);
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


	public partial class Indicator
	{
		private EMA[] cacheEMA;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public EMA EMA(DataSeries input, int period)
		{
			if (cacheEMA != null)
				for (int idx = 0; idx < cacheEMA.Length; idx++)
					if (cacheEMA[idx] != null && cacheEMA[idx].Period == period && cacheEMA[idx].EqualsInput(input))
						return cacheEMA[idx];
			return CacheIndicator<EMA>(new EMA() { Period = period, Input = input }, ref cacheEMA);
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
		public EMA EMA(DataSeries input, int period)
		{
			return indicator.EMA(input, period);
		}
	}
}

