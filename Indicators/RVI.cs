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
	/// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
	/// </summary>
	public class RVI : Indicator
	{
		private double dnAvgH;
		private double dnAvgL;
		private double upAvgH;
		private double upAvgL;
		private double lastDnAvgH;
		private double lastDnAvgL;
		private double lastUpAvgH;
		private double lastUpAvgL;
		private int savedCurrentBar;
		private StdDev stdDevHigh;
		private StdDev stdDevLow;

		internal DataSeries High, Low;
		//DataSeries Close;

		protected override void Init()
		{
			Period = 14;
			savedCurrentBar = -1;
			dnAvgH = dnAvgL = upAvgH = upAvgL = lastDnAvgH
							= lastDnAvgL = lastUpAvgH = lastUpAvgL = 0;

			stdDevHigh = StdDev(High, 10);
			stdDevLow = StdDev(Low, 10);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value[0] = 50;
				return;
			}

			if (CurrentBar != savedCurrentBar)
			{
				dnAvgH = lastDnAvgH;
				dnAvgL = lastDnAvgL;
				upAvgH = lastUpAvgH;
				upAvgL = lastUpAvgL;
				savedCurrentBar = CurrentBar;
			}

			double high0 = High[0];
			double high1 = High[1];
			double low0 = Low[0];
			double low1 = Low[1];
			double up = 0;
			double dn = 0;

			// RVI(High)
			if (high0 > high1)
				up = stdDevHigh[0];
			else if (high0 < high1)
				dn = stdDevHigh[0];

			double actUpAvgH = lastUpAvgH = (upAvgH * (Period - 1) + up) / Period;
			double actDnAvgH = lastDnAvgH = (dnAvgH * (Period - 1) + dn) / Period;
			double rviH = 100 * (actUpAvgH / (actUpAvgH + actDnAvgH));

			// RVI(Low)
			up = 0;
			dn = 0;

			if (low0 > low1)
				up = stdDevLow[0];
			else if (low0 < low1)
				dn = stdDevLow[0];

			double actUpAvgL = lastUpAvgL = (upAvgL * (Period - 1) + up) / Period;
			double actDnAvgL = lastDnAvgL = (dnAvgL * (Period - 1) + dn) / Period;
			double rviL = 100 * (actUpAvgL / (actUpAvgL + actDnAvgL));

			if (CurrentBar == 1)
				Value[0] = 50;
			else
				Value[0] = (rviH + rviL) / 2;
		}

		#region Properties
		[Range(1, int.MaxValue),]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private RVI[] cacheRVI;

		public RVI RVI(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			var cat = cacheRVI;
			if (cacheRVI != null)
				for (int idx = 0; idx < cacheRVI.Length; idx++)
					if (cacheRVI[idx] != null && cacheRVI[idx].Period == period && cat[idx].High == high && cat[idx].Low == low && cacheRVI[idx].EqualsInput(close))
						return cacheRVI[idx];
			return CacheIndicator<RVI>(new RVI() { Period = period, High = high, Low = low, Input = close }, ref cacheRVI);
		}
	}

	public partial class Strategy
	{
		public RVI RVI(int period)
		{
			return RVI(Datas[0], period);
		}

		public RVI RVI(Data data, int period)
		{
			return indicator.RVI(data.H, data.L, data.C, period);
		}
	}
}

#endregion
