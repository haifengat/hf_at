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
	/// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
	/// </summary>
	public class RSS : Indicator
	{
		private EMA ema1;
		private EMA ema2;
		private RSI rsi;
		private SMA sma;
		private DataSeries spread;
		private DataSeries rs;

		protected override void Init()
		{
			EMA1 = 10;
			EMA2 = 40;
			Length = 5;

			spread = new DataSeries(Input);
			rs = new DataSeries(Input);
			ema1 = EMA(Input, EMA1);
			ema2 = EMA(Input, EMA2);
			rsi = RSI(spread, Length, 1);
			sma = SMA(rs, 5);
		}

		protected override void OnBarUpdate()
		{
			spread[0] = ema1[0] - ema2[0];
			rs[0] = rsi[0];
			Value[0] = sma[0];
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("EMA1")]
		public int EMA1 { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("EMA2")]
		public int EMA2 { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Length")]
		public int Length { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private RSS[] cacheRSS;

		public RSS RSS(DataSeries input, int eMA1, int eMA2, int length)
		{
			if (cacheRSS != null)
				for (int idx = 0; idx < cacheRSS.Length; idx++)
					if (cacheRSS[idx] != null && cacheRSS[idx].EMA1 == eMA1 && cacheRSS[idx].EMA2 == eMA2 && cacheRSS[idx].Length == length && cacheRSS[idx].EqualsInput(input))
						return cacheRSS[idx];
			return CacheIndicator<RSS>(new RSS() { EMA1 = eMA1, EMA2 = eMA2, Length = length, Input = input }, ref cacheRSS);
		}
	}

	public partial class Strategy
	{
		public RSS RSS(DataSeries input, int eMA1, int eMA2, int length)
		{
			return indicator.RSS(input, eMA1, eMA2, length);
		}
	}
}

#endregion
