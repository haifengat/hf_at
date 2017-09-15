using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// Bollinger Bands are plotted at standard deviation levels above and below a moving average. 
	/// Since standard deviation is a measure of volatility, the bands are self-adjusting: 
	/// widening during volatile markets and contracting during calmer periods.
	/// </summary>
	public class Bollinger : Indicator
	{
		private SMA sma;
		private StdDev stdDev;

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			NumStdDev = 2;
			Period = 14;
			sma = SMA(Input, Period);
			stdDev = StdDev(Input, Period);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			double sma0 = sma[0];
			double stdDev0 = stdDev[0];

			Upper[0] = sma0 + NumStdDev * stdDev0;
			Middle[0] = sma0;
			Lower[0] = sma0 - NumStdDev * stdDev0;
		}

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public DataSeries Middle
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Range(0, int.MaxValue)]
		[Parameter("NumStdDev", "Parameters")]
		public double NumStdDev { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public DataSeries Upper { get { return Values[0]; } }
		#endregion
	}
	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private Bollinger[] cacheBollinger;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="numStdDev"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Bollinger Bollinger(DataSeries input, double numStdDev, int period)
		{
			if (cacheBollinger != null)
				for (int idx = 0; idx < cacheBollinger.Length; idx++)
					if (cacheBollinger[idx] != null && cacheBollinger[idx].NumStdDev == numStdDev && cacheBollinger[idx].Period == period && cacheBollinger[idx].EqualsInput(input))
						return cacheBollinger[idx];
			return CacheIndicator<Bollinger>(new Bollinger() { NumStdDev = numStdDev, Period = period, Input = input }, ref cacheBollinger);
		}
	}
	public partial class Strategy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="numStdDev"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Bollinger Bollinger(DataSeries input, double numStdDev, int period)
		{
			return indicator.Bollinger(input, numStdDev, period);
		}
	}
}

#endregion
