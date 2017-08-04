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
	/// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator,
	///  the Aroon Oscillator measures the strength of a trend.
	/// </summary>
	public class AroonOscillator : Indicator
	{
		internal DataSeries High, Low;
		//DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Period = 14;
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value[0] = 0;
			else
			{
				int back = Math.Min(Period, CurrentBar);
				int idxMax = -1;
				int idxMin = -1;
				double max = double.MinValue;
				double min = double.MaxValue;

				for (int idx = back; idx >= 0; idx--)
				{
					if (High[back - idx].ApproxCompare(max) >= 0)
					{
						max = High[back - idx];
						idxMax = CurrentBar - back + idx;
					}

					if (Low[back - idx].ApproxCompare(min) <= 0)
					{
						min = Low[back - idx];
						idxMin = CurrentBar - back + idx;
					}
				}

				Value[0] = 100 * ((double)(back - (CurrentBar - idxMax)) / back) - 100 * ((double)(back - (CurrentBar - idxMin)) / back);
			}
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}
	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private AroonOscillator[] cacheAroonOscillator;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close">用于触发指标的计算</param>
		/// <param name="period"></param>
		/// <returns></returns>
		public AroonOscillator AroonOscillator(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			if (cacheAroonOscillator != null)
				for (int idx = 0; idx < cacheAroonOscillator.Length; idx++)
					if (cacheAroonOscillator[idx] != null && cacheAroonOscillator[idx].Period == period && cacheAroonOscillator[idx].High == high && cacheAroonOscillator[idx].Low == low && cacheAroonOscillator[idx].EqualsInput(close))
						return cacheAroonOscillator[idx];
			return CacheIndicator(new AroonOscillator() { Period = period, High = high, Low = low, Input = close }, ref cacheAroonOscillator);
		}
	}

	public partial class Strategy
	{
		public AroonOscillator AroonOscillator(int period)
		{
			return AroonOscillator(Datas[0], period);
		}

		public AroonOscillator AroonOscillator(Data data, int period)
		{
			return Indicator.AroonOscillator(data.H, data.L, data.C, period);
		}
	}
}

#endregion
