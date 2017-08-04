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
	/// The StochRSI is an oscillator similar in computation to the stochastic measure, 
	/// except instead of price values as input, the StochRSI uses RSI values. 
	/// The StochRSI computes the current position of the RSI relative to the high and 
	/// low RSI values over a specified number of days. The intent of this measure, 
	/// designed by Tushard Chande and Stanley Kroll, is to provide further information 
	/// about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. 
	/// Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are 
	/// considered to indicate oversold conditions.
	/// </summary>
	public class StochRSI : Indicator
	{
		private Highest max;
		private Lowest min;
		private RSI rsi;

		protected override void Init()
		{
			Period = 14;

			rsi = RSI(Input, Period, 1);
			min = Lowest(rsi.Value, Period);
			max = Highest(rsi.Value, Period);
		}

		protected override void OnBarUpdate()
		{
			double rsi0 = rsi[0];
			double rsiL = min[0];
			double rsiH = max[0];

			if (rsi0 != rsiL && rsiH != rsiL)
				Value[0] = (rsi0 - rsiL) / (rsiH - rsiL);
			else
				Value[0] = 0;
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
		private StochRSI[] cacheStochRSI;

		public StochRSI StochRSI(DataSeries input, int period)
		{
			if (cacheStochRSI != null)
				for (int idx = 0; idx < cacheStochRSI.Length; idx++)
					if (cacheStochRSI[idx] != null && cacheStochRSI[idx].Period == period && cacheStochRSI[idx].EqualsInput(input))
						return cacheStochRSI[idx];
			return CacheIndicator<StochRSI>(new StochRSI() { Period = period, Input = input }, ref cacheStochRSI);
		}
	}

	public partial class Strategy
	{
		public StochRSI StochRSI(DataSeries input, int period)
		{
			return indicator.StochRSI(input, period);
		}
	}
}

#endregion
