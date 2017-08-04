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
	/// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
	/// </summary>
	public class MFI : Indicator
	{
		private DataSeries negative;
		private DataSeries positive;
		private SUM sumNegative;
		private SUM sumPositive;
		DataSeries Typical;

		internal DataSeries High, Low, Volume;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Period = 14;
			Close = Input;
			Typical = new DataSeries(Input);

			negative = new DataSeries(Input);
			positive = new DataSeries(Input);
			sumNegative = SUM(negative, Period);
			sumPositive = SUM(positive, Period);
		}

		protected override void OnBarUpdate()
		{
			Typical[0] = (High[0] + Low[0] + Close[0]) / 3;

			if (CurrentBar == 0)
				Value[0] = 50;
			else
			{
				double typical0 = Typical[0];
				double typical1 = Typical[1];
				negative[0] = typical0 < typical1 ? typical0 * Volume[0] : 0;
				positive[0] = typical0 > typical1 ? typical0 * Volume[0] : 0;

				double sumNegative0 = sumNegative[0];
				Value[0] = sumNegative0 == 0 ? 50 : 100.0 - (100.0 / (1 + sumPositive[0] / sumNegative0));
			}
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private MFI[] cacheMFI;

		public MFI MFI(DataSeries high, DataSeries low, DataSeries close, DataSeries volume, int period)
		{
			var cat = cacheMFI;
			if (cacheMFI != null)
				for (int idx = 0; idx < cacheMFI.Length; idx++)
					if (cacheMFI[idx] != null && cacheMFI[idx].Period == period && cat[idx].High == high && cat[idx].Low == low&&cat[idx].Volume==volume && cacheMFI[idx].EqualsInput(close))
						return cacheMFI[idx];
			return CacheIndicator<MFI>(new MFI() { Period = period, High = high, Low = low, Volume = volume, Input = close }, ref cacheMFI);
		}
	}
	#endregion

	public partial class Strategy
	{
		public MFI MFI(int period)
		{
			return MFI(Datas[0], period);
		}
		public MFI MFI(Data data, int period)
		{
			return indicator.MFI(data.H, data.L, data.C, data.V, period);
		}
	}
}

