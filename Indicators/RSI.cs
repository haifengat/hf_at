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
	/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
	/// </summary>
	public class RSI : Indicator
	{
		private DataSeries avgDown;
		private DataSeries avgUp;
		private double constant1;
		private double constant2;
		private double constant3;
		private DataSeries down;
		private SMA smaDown;
		private SMA smaUp;
		private DataSeries up;

		protected override void Init()
		{
			Period = 14;
			Smooth = 3;

			constant1 = 2.0 / (1 + Smooth);
			constant2 = (1 - (2.0 / (1 + Smooth)));
			constant3 = (Period - 1);

			avgUp = new DataSeries(this.Input);
			avgDown = new DataSeries(this.Input);
			down = new DataSeries(this.Input);
			up = new DataSeries(this.Input);
			smaDown = SMA(down, Period);
			smaUp = SMA(up, Period);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				down[0] = 0;
				up[0] = 0;

				if (Period < 3)
					Avg[0] = 50;

				return;
			}

			double input0 = Input[0];
			double input1 = Input[1];
			down[0] = Math.Max(input1 - input0, 0);
			up[0] = Math.Max(input0 - input1, 0);

			if (CurrentBar + 1 < Period)
			{
				if (CurrentBar + 1 == Period - 1)
					Avg[0] = 50;
				return;
			}

			if ((CurrentBar + 1) == Period)
			{
				// First averages 
				avgDown[0] = smaDown[0];
				avgUp[0] = smaUp[0];
			}
			else
			{
				// Rest of averages are smoothed
				avgDown[0] = (avgDown[1] * constant3 + down[0]) / Period;
				avgUp[0] = (avgUp[1] * constant3 + up[0]) / Period;
			}

			double avgDown0 = avgDown[0];
			double value0 = avgDown0 == 0 ? 100 : 100 - 100 / (1 + avgUp[0] / avgDown0);
			Default[0] = value0;
			Avg[0] = constant1 * value0 + constant2 * Avg[1];
		}

		#region Properties
		[Browsable(false)]
		public DataSeries Avg
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		public DataSeries Default
		{
			get { return Values[0]; }
		}

		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Smooth")]
		public int Smooth { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private RSI[] cacheRSI;

		public RSI RSI(DataSeries input, int period, int smooth)
		{
			if (cacheRSI != null)
				for (int idx = 0; idx < cacheRSI.Length; idx++)
					if (cacheRSI[idx] != null && cacheRSI[idx].Period == period && cacheRSI[idx].Smooth == smooth && cacheRSI[idx].EqualsInput(input))
						return cacheRSI[idx];
			return CacheIndicator<RSI>(new RSI() { Period = period, Smooth = smooth, Input = input }, ref cacheRSI);
		}
	}

	public partial class Strategy
	{
		public RSI RSI(DataSeries input, int period, int smooth)
		{
			return Indicator.RSI(input, period, smooth);
		}
	}
}

#endregion
