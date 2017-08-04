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
	/// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods.
	/// The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range 
	/// from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate 
	/// oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
	/// </summary>
	public class UltimateOscillator : Indicator
	{
		private DataSeries buyingPressure;
		private double constant1;
		private double constant2;
		private double constant3;
		private SUM sumBpFast;
		private SUM sumBpIntermediate;
		private SUM sumBpSlow;
		private SUM sumTrFast;
		private SUM sumTrIntermediate;
		private SUM sumTrSlow;
		private DataSeries trueRange;

		internal DataSeries High, Low;
		DataSeries Close;

		protected override void Init()
		{
			Fast = 7;
			Intermediate = 14;
			Slow = 28;
			Close = Input;

			constant1 = Slow / Fast;
			constant2 = Slow / Intermediate;
			constant3 = constant1 + constant2 + 1;

			buyingPressure = new DataSeries(Input);
			trueRange = new DataSeries(Input);
			sumBpFast = SUM(buyingPressure, Fast);
			sumBpIntermediate = SUM(buyingPressure, Intermediate);
			sumBpSlow = SUM(buyingPressure, Slow);
			sumTrFast = SUM(trueRange, Fast);
			sumTrIntermediate = SUM(trueRange, Intermediate);
			sumTrSlow = SUM(trueRange, Slow);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value[0] = 0;

			else
			{
				double high0 = High[0];
				double low0 = Low[0];
				double close0 = Close[0];
				double close1 = Close[1];

				buyingPressure[0] = close0 - Math.Min(low0, close1);
				trueRange[0] = Math.Max(Math.Max(high0 - low0, high0 - close1), close1 - low0);

				// Use previous value if we get into trouble
				if (sumTrFast[0] == 0 || sumTrIntermediate[0] == 0 || sumTrSlow[0] == 0)
				{
					Value[0] = Value[1];
					return;
				}

				Value[0] = (((sumBpFast[0] / sumTrFast[0]) * constant1
								+ (sumBpIntermediate[0] / sumTrIntermediate[0]) * constant2
								+ (sumBpSlow[0] / sumTrSlow[0]))
								/ (constant3)) * 100;
			}
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Fast")]
		public int Fast
		{ get; set; }
		[Range(1, int.MaxValue)]
		[Parameter("Intermediate")]
		public int Intermediate
		{ get; set; }
		[Range(1, int.MaxValue)]
		[Parameter("Slow")]
		public int Slow { get; set; }
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private UltimateOscillator[] cacheUltimateOscillator;

		public UltimateOscillator UltimateOscillator(DataSeries high, DataSeries low, DataSeries close, int fast, int intermediate, int slow)
		{
			var cat = cacheUltimateOscillator;
			if (cacheUltimateOscillator != null)
				for (int idx = 0; idx < cacheUltimateOscillator.Length; idx++)
					if (cacheUltimateOscillator[idx] != null && cacheUltimateOscillator[idx].Fast == fast && cacheUltimateOscillator[idx].Intermediate == intermediate && cacheUltimateOscillator[idx].Slow == slow && cat[idx].High == high && cat[idx].Low == low && cacheUltimateOscillator[idx].EqualsInput(close))
						return cacheUltimateOscillator[idx];
			return CacheIndicator(new UltimateOscillator() { Fast = fast, Intermediate = intermediate, Slow = slow, High = high, Low = low, Input = close }, ref cacheUltimateOscillator);
		}
	}

	public partial class Strategy
	{
		public UltimateOscillator UltimateOscillator(int fast, int intermediate, int slow)
		{
			return UltimateOscillator(Datas[0], fast, intermediate, slow);
		}

		public UltimateOscillator UltimateOscillator(Data data, int fast, int intermediate, int slow)
		{
			return indicator.UltimateOscillator(data.H, data.L, data.C, fast, intermediate, slow);
		}
	}
}

#endregion
