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

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one 
	/// measuring the number of periods since the most recent x-period high (Aroon Up) and the 
	/// other measuring the number of periods since the most recent x-period low (Aroon Down).
	/// </summary>
	public class Aroon : Indicator
	{
		private double runningMax;
		private int runningMaxBar;
		private double runningMin;
		private int runningMinBar;

		internal DataSeries High, Low;
		//DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Period = 14;

			runningMax = 0;
			runningMaxBar = 0;
			runningMin = 0;
			runningMinBar = 0;
		}

		protected override void OnBarUpdate()
		{
			double high0 = High[0];
			double low0 = Low[0];

			if (CurrentBar == 0)
			{
				Down[0] = 0;
				Up[0] = 0;
				runningMax = high0;
				runningMin = low0;
				runningMaxBar = 0;
				runningMinBar = 0;
				return;
			}

			int back = Math.Min(Period, CurrentBar);
			if (CurrentBar - runningMaxBar >= Period)
			{
				runningMax = double.MinValue;
				for (int barsBack = back; barsBack > 0; barsBack--)
					if (High[barsBack] >= runningMax)
					{
						runningMax = High[barsBack];
						runningMaxBar = CurrentBar - barsBack;
					}
			}

			if (CurrentBar - runningMinBar >= Period)
			{
				runningMin = double.MaxValue;
				for (int barsBack = back; barsBack > 0; barsBack--)
					if (Low[barsBack] <= runningMin)
					{
						runningMin = Low[barsBack];
						runningMinBar = CurrentBar - barsBack;
					}
			}

			if (high0 >= runningMax)
			{
				runningMax = high0;
				runningMaxBar = CurrentBar;
			}

			if (low0 <= runningMin)
			{
				runningMin = low0;
				runningMinBar = CurrentBar;
			}

			Up[0] = 100 * ((double)(back - (CurrentBar - runningMaxBar)) / back);
			Down[0] = 100 * ((double)(back - (CurrentBar - runningMinBar)) / back);
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }

		[Browsable(false)]
		public DataSeries Up { get { return Values[0]; } }

		[Browsable(false)]
		public DataSeries Down { get { return Values[1]; } }
		#endregion
	}

	#region generated code. Neither change nor remove.
	public partial class Indicator
	{
		private Aroon[] cacheAroon;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close">作为指标执行的触发器</param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Aroon Aroon(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			var cat = cacheAroon;
			if (cacheAroon != null)
				for (int idx = 0; idx < cacheAroon.Length; idx++)
					if (cacheAroon[idx] != null && cacheAroon[idx].Period == period && cat[idx].High == high && cat[idx].Low == low && cacheAroon[idx].EqualsInput(close))
						return cacheAroon[idx];
			return CacheIndicator<Aroon>(new Aroon() { Period = period, High = high, Low = low, Input = close }, ref cacheAroon);
		}
	}
	public partial class Strategy
	{
		public Aroon Aroon(int period)
		{
			return Aroon(Datas[0], period);
		}
		public Aroon Aroon(Data data, int period)
		{
			return indicator.Aroon(data.H, data.L, data.C, period);
		}
	}
}

#endregion
