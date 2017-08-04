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
	/// 价格摆动指标
	/// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
	/// </summary>
	public class PriceOscillator : Indicator
	{
		private EMA emaFast;
		private EMA emaSlow;
		private EMA emaSmooth;
		private DataSeries smoothEma;

		protected override void Init()
		{
			Fast = 12;
			Slow = 26;
			Smooth = 9;

			smoothEma = new DataSeries(Input);
			emaFast = EMA(Input, Fast);
			emaSlow = EMA(Input, Slow);
			emaSmooth = EMA(smoothEma, Smooth);
		}

		protected override void OnBarUpdate()
		{
			smoothEma[0] = emaFast[0] - emaSlow[0];
			Value[0] = emaSmooth[0];
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Fast")]
		public int Fast { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Slow")]
		public int Slow { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Smooth")]
		public int Smooth { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private PriceOscillator[] cachePriceOscillator;

		public PriceOscillator PriceOscillator(DataSeries input, int fast, int slow, int smooth)
		{
			if (cachePriceOscillator != null)
				for (int idx = 0; idx < cachePriceOscillator.Length; idx++)
					if (cachePriceOscillator[idx] != null && cachePriceOscillator[idx].Fast == fast && cachePriceOscillator[idx].Slow == slow && cachePriceOscillator[idx].Smooth == smooth && cachePriceOscillator[idx].EqualsInput(input))
						return cachePriceOscillator[idx];
			return CacheIndicator<PriceOscillator>(new PriceOscillator() { Fast = fast, Slow = slow, Smooth = smooth, Input = input }, ref cachePriceOscillator);
		}
	}

	public partial class Strategy
	{
		public PriceOscillator PriceOscillator(DataSeries input, int fast, int slow, int smooth)
		{
			return Indicator.PriceOscillator(input, fast, slow, smooth);
		}
	}
}

#endregion
