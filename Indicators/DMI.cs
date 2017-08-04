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
	/// Directional Movement Index. Directional Movement Index is quite similiar 
	/// to Welles Wilder's Relative Strength Index. The difference is the DMI 
	/// uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
	/// </summary>
	public class DMI : Indicator
	{
		private DataSeries dmMinus;
		private DataSeries dmPlus;
		private DataSeries tr;
		private SMA smaTr;
		private SMA smaDmPlus;
		private SMA smaDmMinus;

		internal DataSeries High, Low;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Period = 14;
			Close = Input;

			dmMinus = new DataSeries(this.Input);
			dmPlus = new DataSeries(this.Input);
			tr = new DataSeries(this.Input);
			smaTr = SMA(tr, Period);
			smaDmMinus = SMA(dmMinus, Period);
			smaDmPlus = SMA(dmPlus, Period);
		}

		protected override void OnBarUpdate()
		{
			double high0 = High[0];
			double low0 = Low[0];

			if (CurrentBar == 0)
			{
				dmMinus[0] = 0;
				dmPlus[0] = 0;
				tr[0] = high0 - low0;
				Value[0] = 0;
			}
			else
			{
				double low1 = Low[1];
				double High1 = High[1];
				double close1 = Close[1];

				dmMinus[0] = low1 - low0 > high0 - High1 ? Math.Max(low1 - low0, 0) : 0;
				dmPlus[0] = high0 - High1 > low1 - low0 ? Math.Max(high0 - High1, 0) : 0;
				tr[0] = Math.Max(high0 - low0, Math.Max(Math.Abs(high0 - close1), Math.Abs(low0 - close1)));

				double smaTr0 = smaTr[0];
				double smaDmMinus0 = smaDmMinus[0];
				double smaDmPlus0 = smaDmPlus[0];
				double diMinus = (smaTr0 == 0) ? 0 : smaDmMinus0 / smaTr0;
				double diPlus = (smaTr0 == 0) ? 0 : smaDmPlus0 / smaTr0;

				Value[0] = (diPlus + diMinus == 0) ? 0 : (diPlus - diMinus) / (diPlus + diMinus);
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
		private DMI[] cacheDMI;

		public DMI DMI(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			var cat = cacheDMI;
			if (cacheDMI != null)
				for (int idx = 0; idx < cacheDMI.Length; idx++)
					if (cacheDMI[idx] != null && cacheDMI[idx].Period == period && cat[idx].High == high && cat[idx].Low == low && cacheDMI[idx].EqualsInput(close))
						return cacheDMI[idx];
			return CacheIndicator<DMI>(new DMI() { Period = period, High = high, Low = low, Input = close }, ref cacheDMI);
		}
	}

	public partial class Strategy
	{
		public DMI DMI(int period)
		{
			return DMI(Datas[0], period);
		}
		public DMI DMI(Data data, int period)
		{
			return indicator.DMI(data.H, data.L, data.C, period);
		}
	}
}

#endregion
