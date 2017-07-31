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
	/// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average
	/// for the specified price series and period. VWMA is similar to a Simple Moving Average
	/// (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance 
	/// on the days with the largest volume and the least for the days with lowest volume for the period specified.
	/// </summary>
	public class VWMA : Indicator
	{
		private double priorVolPriceSum;
		private double volPriceSum;
		private DataSeries volSum;
		DataSeries Volume;

		protected override void Init()
		{
			Period = 14;
			Volume = Inputs[1];

			volSum = new DataSeries(Input);
		}

		protected override void OnBarUpdate()
		{
			if (IsFirstTickOfBar)
				priorVolPriceSum = volPriceSum;

			double volume0 = Volume[0];
			double volumePeriod = Volume[Math.Min(Period, CurrentBar)];
			volPriceSum = priorVolPriceSum + Input[0] * volume0 - (CurrentBar >= Period ? Input[Period] * volumePeriod : 0);
			volSum[0] = volume0 + (CurrentBar > 0 ? volSum[1] : 0) - (CurrentBar >= Period ? volumePeriod : 0);
			Value[0] = volSum[0].ApproxCompare(0) == 0 ? volPriceSum : volPriceSum / volSum[0];
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
		private VWMA[] cacheVWMA;

		public VWMA VWMA(DataSeries volume, DataSeries input, int period)
		{
			if (cacheVWMA != null)
				for (int idx = 0; idx < cacheVWMA.Length; idx++)
					if (cacheVWMA[idx] != null && cacheVWMA[idx].Period == period && cacheVWMA[idx].EqualsInput(input))
						return cacheVWMA[idx];
			return CacheIndicator<VWMA>(new VWMA() { Period = period, Inputs = new[] { input, volume } }, ref cacheVWMA);
		}
	}

	public partial class Strategy
	{
		public VWMA VWMA(DataSeries input, int period)
		{
			return VWMA(Datas[0], input, period);
		}
		public VWMA VWMA(Data data, DataSeries input, int period)
		{
			return indicator.VWMA(data.V, input, period);
		}
	}
}

#endregion
