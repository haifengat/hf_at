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
	/// Directional Movement (DM). This is the same indicator as the ADX,
	/// with the addition of the two directional movement indicators +DI
	/// and -DI. +DI and -DI measure upward and downward momentum. A buy 
	/// signal is generated when +DI crosses -DI to the upside. 
	/// A sell signal is generated when -DI crosses +DI to the downside.
	/// </summary>
	public class DM : Indicator
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
			Period = 14;

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
			double trueRange = high0 - low0;

			if (CurrentBar == 0)
			{
				tr[0] = trueRange;
				dmPlus[0] = 0;
				dmMinus[0] = 0;
				sumTr[0] = tr[0];
				sumDmPlus[0] = dmPlus[0];
				sumDmMinus[0] = dmMinus[0];
				Value[0] = 50;
			}
			else
			{
				double low1 = Low[1];
				double high1 = High[1];
				double close1 = Close[1];

				tr[0] = Math.Max(Math.Abs(low0 - close1), Math.Max(trueRange, Math.Abs(high0 - close1)));
				dmPlus[0] = high0 - high1 > low1 - low0 ? Math.Max(high0 - high1, 0) : 0;
				dmMinus[0] = low1 - low0 > high0 - high1 ? Math.Max(low1 - low0, 0) : 0;

				double sumDmPlus1 = sumDmPlus[1];
				double sumDmMinus1 = sumDmMinus[1];
				double sumTr1 = sumTr[1];

				if (CurrentBar < Period)
				{
					sumTr[0] = sumTr1 + tr[0];
					sumDmPlus[0] = sumDmPlus1 + dmPlus[0];
					sumDmMinus[0] = sumDmMinus1 + dmMinus[0];
				}
				else
				{
					sumTr[0] = sumTr1 - sumTr[1] / Period + tr[0];
					sumDmPlus[0] = sumDmPlus1 - sumDmPlus1 / Period + dmPlus[0];
					sumDmMinus[0] = sumDmMinus1 - sumDmMinus1 / Period + dmMinus[0];
				}

				double diPlus = 100 * (sumTr[0] == 0 ? 0 : sumDmPlus[0] / sumTr[0]);
				double diMinus = 100 * (sumTr[0] == 0 ? 0 : sumDmMinus[0] / sumTr[0]);
				double diff = Math.Abs(diPlus - diMinus);
				double sum = diPlus + diMinus;

				Value[0] = sum == 0 ? 50 : ((Period - 1) * Value[1] + 100 * diff / sum) / Period;
				DiPlus[0] = diPlus;
				DiMinus[0] = diMinus;
			}
		}

		#region Properties
		[Browsable(false)]
		public DataSeries DiPlus
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		public DataSeries DiMinus
		{
			get { return Values[2]; }
		}

		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private DM[] cacheDM;

		public DM DM(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			var cat = cacheDM;
			if (cacheDM != null)
				for (int idx = 0; idx < cacheDM.Length; idx++)
					if (cacheDM[idx] != null && cacheDM[idx].Period == period && cat[idx].High == high && cat[idx].Low == low && cacheDM[idx].EqualsInput(close))
						return cacheDM[idx];
			return CacheIndicator<DM>(new DM() { Period = period, High = high, Low = low, Input = close }, ref cacheDM);
		}
	}

	public partial class Strategy
	{
		public DM DM(int period)
		{
			return DM(Datas[0], period);
		}
		public DM DM(Data data, int period)
		{
			return Indicator.DM(data.H, data.L, data.C, period);
		}
	}
}

#endregion
