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
	/// Average Directional Movement Rating quantifies momentum change in the ADX. 
	/// It is calculated by adding two values of ADX (the current value and a value n periods back), 
	/// then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. 
	/// The interpretation is the same as the ADX; the higher the value, the stronger the trend.
	/// </summary>
	public class ADXR : Indicator
	{
		private ADX adx;

		internal DataSeries High, Low;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Period = 14;
			Interval = 10;
			Close = Input;
			adx = ADX(High, Low, Close, Period);
		}

		protected override void OnBarUpdate()
		{
			Value[0] = CurrentBar < Interval ? ((adx[0] + adx[CurrentBar]) / 2) : ((adx[0] + adx[Interval]) / 2);
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Interval", "Parameters")]
		public int Interval { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}

	#region  generated code. Neither change nor remove.

	public partial class Indicator
	{
		private ADXR[] cacheADXR;

		public ADXR ADXR(DataSeries high, DataSeries low, DataSeries close, int interval, int period)
		{
			if (cacheADXR != null)
				for (int idx = 0; idx < cacheADXR.Length; idx++)
					if (cacheADXR[idx] != null && cacheADXR[idx].Interval == interval && cacheADXR[idx].Period == period && cacheADXR[idx].EqualsInput(high, low, close))
						return cacheADXR[idx];
			return CacheIndicator<ADXR>(new ADXR() { Interval = interval, Period = period,High=high,Low=low, Input=close }, ref cacheADXR);
		}
	}

	public partial class Strategy
	{
		public ADXR ADXR(int interval, int period)
		{
			return ADXR(Datas[0], interval, period);
		}
		public ADXR ADXR(Data data, int interval, int period)
		{
			return indicator.ADXR(data.H, data.L, data.C, interval, period);
		}
	}
}

#endregion
