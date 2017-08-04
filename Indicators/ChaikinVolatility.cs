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
		private Range range;

		internal DataSeries High, Low;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			MAPeriod = 10;
			ROCPeriod = 10;
			Close = Input;

			range = Range(High, Low, Close);
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close">用于触发指标的计算</param>
		/// <param name="mAPeriod"></param>
		/// <param name="rOCPeriod"></param>
		/// <returns></returns>
		public ChaikinVolatility ChaikinVolatility(DataSeries high, DataSeries low, DataSeries close, int mAPeriod, int rOCPeriod)
		{
			var cat = cacheChaikinVolatility;
			if (cacheChaikinVolatility != null)
				for (int idx = 0; idx < cacheChaikinVolatility.Length; idx++)
					if (cacheChaikinVolatility[idx] != null && cacheChaikinVolatility[idx].MAPeriod == mAPeriod && cacheChaikinVolatility[idx].ROCPeriod == rOCPeriod && cat[idx].High == high && cat[idx].Low == low && cacheChaikinVolatility[idx].EqualsInput(close))
						return cacheChaikinVolatility[idx];
			return CacheIndicator<ChaikinVolatility>(new ChaikinVolatility() { MAPeriod = mAPeriod, ROCPeriod = rOCPeriod, High = high, Low = low, Input = close }, ref cacheChaikinVolatility);
		}
	}

	public partial class Strategy
	{
		public ChaikinVolatility ChaikinVolatility(int mAPeriod, int rOCPeriod)
		{
			return ChaikinVolatility(Datas[0], mAPeriod, rOCPeriod);
		}
		public ChaikinVolatility ChaikinVolatility(Data data, int mAPeriod, int rOCPeriod)
		{
			return indicator.ChaikinVolatility(data.H, data.L, data.C, mAPeriod, rOCPeriod);
		}
	}
}

#endregion
