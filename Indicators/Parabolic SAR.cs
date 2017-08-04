// 
// Copyright (C) 2017, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
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
	/// 抛物线指标
	/// Parabolic SAR according to Stocks and Commodities magazine V 11:11 (477-479).
	/// </summary>
	public class ParabolicSAR : Indicator
	{
		private double af;      // Acceleration factor
		private bool afIncreased;
		private bool longPosition;
		private int prevBar;
		private double prevSAR;
		private int reverseBar;
		private double reverseValue;
		private double todaySAR;        // SAR value
		private double xp;      // Extreme Price

		internal DataSeries High, Low;
		//DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Acceleration = 0.02;
			AccelerationStep = 0.02;
			AccelerationMax = 0.2;

			xp = 0.0;
			af = 0;
			todaySAR = 0;
			prevSAR = 0;
			reverseBar = 0;
			reverseValue = 0;
			prevBar = 0;
			afIncreased = false;
		}

		protected override void OnBarUpdate()
		{
			int CurrentBar = Input.Count - 1;

			if (CurrentBar < 3)
				return;

			if (CurrentBar == 3)
			{
				// Determine initial position
				longPosition = High[0] > High[1];
				xp = longPosition ? High.Highest(0, CurrentBar) : Low.Lowest(0, CurrentBar);
				af = Acceleration;
				Value[0] = xp + (longPosition ? -1 : 1) * (High.Highest(0, CurrentBar) - Low.Lowest(0, CurrentBar)) * af;
				return;
			}

			// Reset accelerator increase limiter on new bars
			if (afIncreased && prevBar != CurrentBar)
				afIncreased = false;

			// Current event is on a bar not marked as a reversal bar yet
			if (reverseBar != CurrentBar)
			{
				// SAR = SAR[1] + af * (xp - SAR[1])
				todaySAR = TodaySAR(Value[1] + af * (xp - Value[1]));
				for (int x = 1; x <= 2; x++)
				{
					if (longPosition)
					{
						if (todaySAR > Low[x])
							todaySAR = Low[x];
					}
					else
					{
						if (todaySAR < High[x])
							todaySAR = High[x];
					}
				}

				// Holding long position
				if (longPosition)
				{
					// Process a new SAR value only on a new bar or if SAR value was penetrated.
					if (prevBar != CurrentBar || Low[0] < prevSAR)
					{
						Value[0] = todaySAR;
						prevSAR = todaySAR;
					}
					else
						Value[0] = prevSAR;

					if (High[0] > xp)
					{
						xp = High[0];
						AfIncrease();
					}
				}

				// Holding short position
				else if (!longPosition)
				{
					// Process a new SAR value only on a new bar or if SAR value was penetrated.
					if (prevBar != CurrentBar || High[0] > prevSAR)
					{
						Value[0] = todaySAR;
						prevSAR = todaySAR;
					}
					else
						Value[0] = prevSAR;

					if (Low[0] < xp)
					{
						xp = Low[0];
						AfIncrease();
					}
				}
			}

			// Current event is on the same bar as the reversal bar
			else
			{
				// Only set new xp values. No increasing af since this is the first bar.
				if (longPosition && High[0] > xp)
					xp = High[0];
				else if (!longPosition && Low[0] < xp)
					xp = Low[0];

				Value[0] = prevSAR;

				// SAR = SAR[1] + af * (xp - SAR[1])
				todaySAR = TodaySAR(longPosition ? Math.Min(reverseValue, Low[0]) : Math.Max(reverseValue, High[0]));
			}

			prevBar = CurrentBar;

			// Reverse position
			if ((longPosition && (Low[0] < todaySAR || Low[1] < todaySAR))
				|| (!longPosition && (High[0] > todaySAR || High[1] > todaySAR)))
				Value[0] = Reverse();

		}

		#region Miscellaneous
		// Only raise accelerator if not raised for current bar yet
		private void AfIncrease()
		{
			if (!afIncreased)
			{
				af = Math.Min(AccelerationMax, af + AccelerationStep);
				afIncreased = true;
			}
		}

		// Additional rule. SAR for today can't be placed inside the bar of day - 1 or day - 2.
		private double TodaySAR(double todaySAR)
		{
			if (longPosition)
			{
				double lowestSAR = Math.Min(Math.Min(todaySAR, Low[0]), Low[1]);
				if (Low[0] > lowestSAR)
					todaySAR = lowestSAR;
			}
			else
			{
				double highestSAR = Math.Max(Math.Max(todaySAR, High[0]), High[1]);
				if (High[0] < highestSAR)
					todaySAR = highestSAR;
			}
			return todaySAR;
		}

		private double Reverse()
		{
			double todaySAR = xp;

			if ((longPosition && prevSAR > Low[0]) || (!longPosition && prevSAR < High[0]) || prevBar != CurrentBar)
			{
				longPosition = !longPosition;
				reverseBar = CurrentBar;
				reverseValue = xp;
				af = Acceleration;
				xp = longPosition ? High[0] : Low[0];
				prevSAR = todaySAR;
			}
			else
				todaySAR = prevSAR;
			return todaySAR;
		}
		#endregion

		#region 
		[Range(0.00, double.MaxValue)]
		[Parameter("Acceleration")]
		public double Acceleration { get; set; }

		[Range(0.001, double.MaxValue)]
		[Parameter("AccelerationMax")]
		public double AccelerationMax { get; set; }

		[Range(0.001, double.MaxValue)]
		[Parameter("AccelerationStep")]
		public double AccelerationStep { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private ParabolicSAR[] cacheParabolicSAR;

		public ParabolicSAR ParabolicSAR(DataSeries high, DataSeries low, DataSeries close, double acceleration, double accelerationMax, double accelerationStep)
		{
			var cat = cacheParabolicSAR;
			if (cacheParabolicSAR != null)
				for (int idx = 0; idx < cacheParabolicSAR.Length; idx++)
					if (cacheParabolicSAR[idx] != null && cacheParabolicSAR[idx].Acceleration == acceleration && cacheParabolicSAR[idx].AccelerationMax == accelerationMax&& cat[idx].High==high&&cat[idx].Low==low && cacheParabolicSAR[idx].AccelerationStep == accelerationStep && cat[idx].High == high && cat[idx].Low == low && cacheParabolicSAR[idx].EqualsInput(close))
						return cacheParabolicSAR[idx];
			return CacheIndicator<ParabolicSAR>(new ParabolicSAR() { Acceleration = acceleration, AccelerationMax = accelerationMax, AccelerationStep = accelerationStep, High = high, Low = low, Input = close }, ref cacheParabolicSAR);
		}
	}

	public partial class Strategy
	{
		public ParabolicSAR ParabolicSAR(double acceleration, double accelerationMax, double accelerationStep)
		{
			return ParabolicSAR(Datas[0], acceleration, accelerationMax, accelerationStep);
		}

		public ParabolicSAR ParabolicSAR(Data data, double acceleration, double accelerationMax, double accelerationStep)
		{
			return indicator.ParabolicSAR(data.H, data.L, data.C, acceleration, accelerationMax, accelerationStep);
		}
	}
}

#endregion
