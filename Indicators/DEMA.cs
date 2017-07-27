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
	/// Double Exponential Moving Average
	/// </summary>
	public class DEMA : Indicator
	{
		private EMA ema;
		private EMA emaEma;

		protected override void Init()
		{
			Period = 14;

			ema = EMA(Inputs[0], Period);
			emaEma = EMA(ema.Value, Period);
		}

		protected override void OnBarUpdate()
		{
			Value[0] = 2 * ema[0] - emaEma[0];
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
		private DEMA[] cacheDEMA;

		public DEMA DEMA(DataSeries input, int period)
		{
			if (cacheDEMA != null)
				for (int idx = 0; idx < cacheDEMA.Length; idx++)
					if (cacheDEMA[idx] != null && cacheDEMA[idx].Period == period && cacheDEMA[idx].EqualsInput(input))
						return cacheDEMA[idx];
			return CacheIndicator<DEMA>(new DEMA() { Period = period, Input = input }, ref cacheDEMA);
		}
	}

	public partial class Strategy
	{
		public DEMA DEMA(DataSeries input, int period)
		{
			return indicator.DEMA(input, period);
		}
	}
}

#endregion
