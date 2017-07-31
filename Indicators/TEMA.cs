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
	/// Triple Exponential Moving Average
	/// </summary>
	public class TEMA : Indicator
	{
		private EMA ema1;
		private EMA ema2;
		private EMA ema3;

		protected override void Init()
		{
			Period = 14;

			ema1 = EMA(Inputs[0], Period);
			ema2 = EMA(ema1.Value, Period);
			ema3 = EMA(ema2.Value, Period);
		}

		protected override void OnBarUpdate()
		{
			Value[0] = 3 * ema1[0] - 3 * ema2[0] + ema3[0];
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
		private TEMA[] cacheTEMA;

		public TEMA TEMA(DataSeries input, int period)
		{
			if (cacheTEMA != null)
				for (int idx = 0; idx < cacheTEMA.Length; idx++)
					if (cacheTEMA[idx] != null && cacheTEMA[idx].Period == period && cacheTEMA[idx].EqualsInput(input))
						return cacheTEMA[idx];
			return CacheIndicator<TEMA>(new TEMA() { Period = period, Input = input }, ref cacheTEMA);
		}
	}

	public partial class Strategy
	{
		public TEMA TEMA(DataSeries input, int period)
		{
			return indicator.TEMA(input, period);
		}
	}
}

#endregion
