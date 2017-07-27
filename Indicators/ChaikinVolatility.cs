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
	/// Chaikin Volatility
	/// http://wiki.mbalib.com/wiki/佳庆离散指标
	/// </summary>
	public class ChaikinVolatility : Indicator
	{
		private EMA ema;
		private DataSeries High, Low;
		private Range range;

		protected override void Init()
		{
			MAPeriod = 10;
			ROCPeriod = 10;
			range = Range(High, Low);
			ema = EMA(range.Value, MAPeriod);
		}

		protected override void OnBarUpdate()
		{
			double emaROCPeriod = ema[Math.Min(CurrentBar, ROCPeriod)];
			Value[0] = CurrentBar == 0 ? ema[0] : ((ema[0] - emaROCPeriod) / emaROCPeriod) * 100;
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("MAPeriod")]
		public int MAPeriod { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("ROCPeriod")]
		public int ROCPeriod { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.
	public partial class Indicator
	{
		private ChaikinVolatility[] cacheChaikinVolatility;

		public ChaikinVolatility ChaikinVolatility(DataSeries high, DataSeries low, int mAPeriod, int rOCPeriod)
		{
			if (cacheChaikinVolatility != null)
				for (int idx = 0; idx < cacheChaikinVolatility.Length; idx++)
					if (cacheChaikinVolatility[idx] != null && cacheChaikinVolatility[idx].MAPeriod == mAPeriod && cacheChaikinVolatility[idx].ROCPeriod == rOCPeriod && cacheChaikinVolatility[idx].EqualsInput(high, low))
						return cacheChaikinVolatility[idx];
			return CacheIndicator<ChaikinVolatility>(new ChaikinVolatility() { MAPeriod = mAPeriod, ROCPeriod = rOCPeriod, Inputs = new[] { high, low } }, ref cacheChaikinVolatility);
		}
	}

	public partial class Strategy
	{
		public ChaikinVolatility ChaikinVolatility(int mAPeriod, int rOCPeriod)
		{
			return indicator.ChaikinVolatility(H, L, mAPeriod, rOCPeriod);
		}
	}
}

#endregion
