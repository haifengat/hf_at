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
	public class StochasticsFast : Indicator
	{
		private DataSeries den;
		private Highest max;
		private Lowest min;
		private DataSeries nom;
		private SMA smaK;

		internal DataSeries High, Low;
		DataSeries Close;

		protected override void Init()
		{
			PeriodD = 3;
			PeriodK = 14;
			Close = Input;

			den = new DataSeries(Input);
			nom = new DataSeries(Input);
			min = Lowest(Low, PeriodK);
			max = Highest(High, PeriodK);
			smaK = SMA(K, PeriodD);
		}

		protected override void OnBarUpdate()
		{
			double min0 = min[0];
			nom[0] = Close[0] - min0;
			den[0] = max[0] - min0;

			if (den[0].ApproxCompare(0) == 0)
				K[0] = CurrentBar == 0 ? 50 : K[1];
			else
				K[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

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
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private StochasticsFast[] cacheStochasticsFast;

		public StochasticsFast StochasticsFast(DataSeries high, DataSeries low, DataSeries close, int periodD, int periodK)
		{
			var cat = cacheStochasticsFast;
			if (cacheStochasticsFast != null)
				for (int idx = 0; idx < cacheStochasticsFast.Length; idx++)
					if (cacheStochasticsFast[idx] != null && cacheStochasticsFast[idx].PeriodD == periodD && cacheStochasticsFast[idx].PeriodK == periodK && cat[idx].High == high && cat[idx].Low == low && cacheStochasticsFast[idx].EqualsInput(close))
						return cacheStochasticsFast[idx];
			return CacheIndicator<StochasticsFast>(new StochasticsFast() { PeriodD = periodD, PeriodK = periodK, High = high, Low = low, Input = close }, ref cacheStochasticsFast);
		}
	}

	public partial class Strategy
	{
		public StochasticsFast StochasticsFast(int periodD, int periodK)
		{
			return StochasticsFast(Datas[0], periodD, periodK);
		}

		public StochasticsFast StochasticsFast(Data data, int periodD, int periodK)
		{
			return indicator.StochasticsFast(data.H, data.L, data.C, periodD, periodK);
		}
		public StochasticsFast KDFast(int periodD, int periodK)
		{
			return StochasticsFast(Datas[0], periodD, periodK);
		}

		public StochasticsFast KDFast(Data data, int periodD, int periodK)
		{
			return indicator.StochasticsFast(data.H, data.L, data.C, periodD, periodK);
		}
	}
}

#endregion
