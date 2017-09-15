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
	/// Double stochastics
	/// </summary>
	public class DoubleStochastics : Indicator
	{
		private EMA emaP1;
		private EMA emaP3;
		private Lowest minLow;
		private Lowest minP2;
		private Highest maxHigh;
		private Highest maxP2;
		private DataSeries p1;
		private DataSeries p2;
		private DataSeries p3;
		DataSeries High, Low, Close;

		protected override void Init()
		{
			Period = 10;
			High = Inputs[0];
			Low = Inputs[1];
			Close = Inputs[2];

			p1 = new DataSeries(Input);
			p2 = new DataSeries(Input);
			p3 = new DataSeries(Input);
			emaP1 = EMA(p1, 3);
			emaP3 = EMA(p3, 3);
			maxHigh = Highest(High, Period);
			maxP2 = Highest(p2, Period);
			minLow = Lowest(Low, Period);
			minP2 = Lowest(p2, Period);
		}

		protected override void OnBarUpdate()
		{
			double maxHigh0 = maxHigh[0];
			double minLow0 = minLow[0];
			double r = maxHigh0 - minLow0;
			r = r.ApproxCompare(0) == 0 ? 0 : r;

			if (r == 0)
				p1[0] = CurrentBar == 0 ? 50 : p1[1];
			else
				p1[0] = Math.Min(100, Math.Max(0, 100 * (Close[0] - minLow0) / r));

			p2[0] = emaP1[0];
			double minP20 = minP2[0];
			double s = maxP2[0] - minP20;
			s = s.ApproxCompare(0) == 0 ? 0 : s;

			if (s == 0)
				p3[0] = CurrentBar == 0 ? 50 : p3[1];
			else
				p3[0] = Math.Min(100, Math.Max(0, 100 * (p2[0] - minP20) / s));

			K[0] = emaP3[0];
		}

		#region Properties
		[Browsable(false)]
		public DataSeries K { get { return Values[0]; } }

		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private DoubleStochastics[] cacheDoubleStochastics;

		public DoubleStochastics DoubleStochastics(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			if (cacheDoubleStochastics != null)
				for (int idx = 0; idx < cacheDoubleStochastics.Length; idx++)
					if (cacheDoubleStochastics[idx] != null && cacheDoubleStochastics[idx].Period == period && cacheDoubleStochastics[idx].EqualsInput(high, low, close))
						return cacheDoubleStochastics[idx];
			return CacheIndicator<DoubleStochastics>(new DoubleStochastics() { Period = period, Inputs = new[] { high, low, close } }, ref cacheDoubleStochastics);
		}
	}

	public partial class Strategy
	{
		public DoubleStochastics DoubleStochastics(int period)
		{
			return DoubleStochastics(Datas[0], period);
		}
		public DoubleStochastics DoubleStochastics(Data data,int period)
		{
			return indicator.DoubleStochastics(data.H, data.L, data.C, period);
		}
	}
}

#endregion
