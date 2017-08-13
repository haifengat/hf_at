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
	/// The Dynamic Momentum Index is a variable term RSI. The RSI term varies
	///  from 3 to 30. The variable time period makes the RSI more responsive to 
	/// short-term moves. The more volatile the price is, the shorter the time period is.
	///  It is interpreted in the same way as the RSI, but provides signals earlier.
	/// </summary>
	public class DMIndex : Indicator
	{
		private SMA sma;
		private StdDev stdDev;

		protected override void Init()
		{
			stdDev = StdDev(this.Input, 5);
			sma = SMA(stdDev.Value, 10);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value[0] = Input[0];
				return;
			}
			int rsiPeriod = (int)(14 / (stdDev[0] / sma[0])) < 1 ? 1 : (int)(14 / (stdDev[0] / sma[0]));
			Value[0] = RSI(Input, rsiPeriod, Smooth)[0];
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Smooth")]
		public int Smooth { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private DMIndex[] cacheDMIndex;

		public DMIndex DMIndex(DataSeries input, int smooth)
		{
			if (cacheDMIndex != null)
				for (int idx = 0; idx < cacheDMIndex.Length; idx++)
					if (cacheDMIndex[idx] != null && cacheDMIndex[idx].Smooth == smooth && cacheDMIndex[idx].EqualsInput(input))
						return cacheDMIndex[idx];
			return CacheIndicator<DMIndex>(new DMIndex() { Smooth = smooth, Input = input }, ref cacheDMIndex);
		}

	}
	public partial class Strategy
	{
		public DMIndex DMIndex(DataSeries input, int smooth)
		{
			return Indicator.DMIndex(input, smooth);
		}
	}
}

#endregion
