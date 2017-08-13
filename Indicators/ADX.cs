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
	/// The Average Directional Index measures the strength of a prevailing trend as well as whether movement 
	/// exists in the market. The ADX is measured on a scale of 0  100. A low ADX value (generally less than 20) 
	/// can indicate a non-trending market with low volumes whereas a cross above 20 may indicate the start of
	///  a trend (either up or down). If the ADX is over 40 and begins to fall, it can indicate the slowdown of a current trend.
	/// </summary>
	public class ADX : Indicator
	{
		private DataSeries dmPlus;
		private DataSeries dmMinus;
		private DataSeries sumDmPlus;
		private DataSeries sumDmMinus;
		private DataSeries sumTr;
		private DataSeries tr;

		internal DataSeries High, Low;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Close = Input;

			dmPlus = new DataSeries(this.Input);
			dmMinus = new DataSeries(this.Input);
			sumDmPlus = new DataSeries(this.Input);
			sumDmMinus = new DataSeries(this.Input);
			sumTr = new DataSeries(this.Input);
			tr = new DataSeries(this.Input);
		}

		protected override void OnBarUpdate()
		{
			double high0 = High[0];
			double low0 = Low[0];

			if (CurrentBar == 0)
			{
				tr[0] = high0 - low0;
				dmPlus[0] = 0;
				dmMinus[0] = 0;
				sumTr[0] = tr[0];
				sumDmPlus[0] = dmPlus[0];
				sumDmMinus[0] = dmMinus[0];
				Value[0] = 50;
			}
			else
			{
				double high1 = High[1];
				double low1 = Low[1];
				double close1 = Close[1];

				tr[0] = Math.Max(Math.Abs(low0 - close1), Math.Max(high0 - low0, Math.Abs(high0 - close1)));
				dmPlus[0] = high0 - high1 > low1 - low0 ? Math.Max(high0 - high1, 0) : 0;
				dmMinus[0] = low1 - low0 > high0 - high1 ? Math.Max(low1 - low0, 0) : 0;

				if (CurrentBar < Period)
				{
					sumTr[0] = sumTr[1] + tr[0];
					sumDmPlus[0] = sumDmPlus[1] + dmPlus[0];
					sumDmMinus[0] = sumDmMinus[1] + dmMinus[0];
				}
				else
				{
					double sumTr1 = sumTr[1];
					double sumDmPlus1 = sumDmPlus[1];
					double sumDmMinus1 = sumDmMinus[1];

					sumTr[0] = sumTr1 - sumTr1 / Period + tr[0];
					sumDmPlus[0] = sumDmPlus1 - sumDmPlus1 / Period + dmPlus[0];
					sumDmMinus[0] = sumDmMinus1 - sumDmMinus1 / Period + dmMinus[0];
				}

				double sumTr0 = sumTr[0];
				double diPlus = 100 * (sumTr0.ApproxCompare(0) == 0 ? 0 : sumDmPlus[0] / sumTr[0]);
				double diMinus = 100 * (sumTr0.ApproxCompare(0) == 0 ? 0 : sumDmMinus[0] / sumTr[0]);
				double diff = Math.Abs(diPlus - diMinus);
				double sum = diPlus + diMinus;

				Value[0] = sum.ApproxCompare(0) == 0 ? 50 : ((Period - 1) * Value[1] + 100 * diff / sum) / Period;
			}
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; } = 14;
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private ADX[] cacheADX;

		public ADX ADX(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			if (cacheADX != null)
				for (int idx = 0; idx < cacheADX.Length; idx++)
					if (cacheADX[idx] != null && cacheADX[idx].Period == period && cacheADX[idx].High == high && cacheADX[idx].Low == low && cacheADX[idx].EqualsInput(close))
						return cacheADX[idx];
			return CacheIndicator<ADX>(new ADX() { Period = period, High = high, Low = low, Input = close }, ref cacheADX);
		}
	}
	public partial class Strategy
	{
		public ADX ADX(int period)
		{
			return ADX(Datas[0], period);
		}
		public ADX ADX(Data data, int period)
		{
			return Indicator.ADX(data.H, data.L, data.C, period);
		}
	}
}

#endregion
