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
	/// The Stochastic Oscillator is made up of two lines that oscillate between 
	/// a vertical scale of 0 to 100. The %K is the main line and it is drawn as 
	/// a solid line. The second is the %D line and is a moving average of %K. 
	/// The %D line is drawn as a dotted line. Use as a buy/sell signal generator, 
	/// buying when fast moves above slow and selling when fast moves below slow.
	/// </summary>
	public class Stochastics : Indicator
	{
		private DataSeries den;
		private DataSeries fastK;
		private Lowest min;
		private Highest max;
		private DataSeries nom;
		private SMA smaFastK;
		private SMA smaK;

		internal DataSeries High, Low;
		DataSeries Close;

		protected override void Init()
		{
			PeriodD = 7;
			PeriodK = 14;
			Smooth = 3;
			Close = Input;

			den = new DataSeries(Input);
			nom = new DataSeries(Input);
			fastK = new DataSeries(Input);
			min = Lowest(Low, PeriodK);
			max = Highest(High, PeriodK);
			smaFastK = SMA(fastK, Smooth);
			smaK = SMA(K, PeriodD);
		}

		protected override void OnBarUpdate()
		{
			double min0 = min[0];
			nom[0] = Close[0] - min0;
			den[0] = max[0] - min0;

			if (den[0].ApproxCompare(0) == 0)
				fastK[0] = CurrentBar == 0 ? 50 : fastK[1];
			else
				fastK[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

			// Slow %K == Fast %D
			K[0] = smaFastK[0];
			D[0] = smaK[0];
		}

		#region Properties
		[Browsable(false)]
		public DataSeries D
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		public DataSeries K
		{
			get { return Values[1]; }
		}

		[Range(1, int.MaxValue)]
		[Parameter("PeriodD")]
		public int PeriodD { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("PeriodK")]
		public int PeriodK { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Smooth")]
		public int Smooth { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private Stochastics[] cacheStochastics;

		public Stochastics Stochastics(DataSeries high, DataSeries low, DataSeries close, int periodD, int periodK, int smooth)
		{
			var cat = cacheStochastics;
			if (cacheStochastics != null)
				for (int idx = 0; idx < cacheStochastics.Length; idx++)
					if (cacheStochastics[idx] != null && cacheStochastics[idx].PeriodD == periodD && cacheStochastics[idx].PeriodK == periodK && cacheStochastics[idx].Smooth == smooth &&cat[idx].High==high&&cat[idx].Low==low && cacheStochastics[idx].EqualsInput(close))
						return cacheStochastics[idx];
			return CacheIndicator(new Stochastics() { PeriodD = periodD, PeriodK = periodK, Smooth = smooth, High = high, Low = low, Input = close }, ref cacheStochastics);
		}
	}

	public partial class Strategy
	{
		public Stochastics Stochastics(int periodD, int periodK, int smooth)
		{
			return Stochastics(Datas[0], periodD, periodK, smooth);
		}

		public Stochastics Stochastics(Data data, int periodD, int periodK, int smooth)
		{
			return indicator.Stochastics(data.H, data.L, data.C, periodD, periodK, smooth);
		}

		public Stochastics KD(int periodD, int periodK, int smooth)
		{
			return Stochastics(Datas[0], periodD, periodK, smooth);
		}

		public Stochastics KD(Data data, int periodD, int periodK, int smooth)
		{
			return indicator.Stochastics(data.H, data.L, data.C, periodD, periodK, smooth);
		}
	}
}

#endregion
