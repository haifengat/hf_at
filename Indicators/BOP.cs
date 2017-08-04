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
	/// The balance of power indicator measures the strength of the bulls vs. bears by
	///  assessing the ability of each to push price to an extreme level.
	/// </summary>
	public class BOP : Indicator
	{
		private DataSeries bop;
		private SMA sma;
		internal DataSeries Open, High, Low;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			Smooth = 14;
			Close = Input;
			bop = new DataSeries(this.Input);
			sma = SMA(bop, Smooth);
		}

		protected override void OnBarUpdate()
		{
			double high0 = High[0];
			double low0 = Low[0];

			if ((high0 - low0).ApproxCompare(0) == 0)
				bop[0] = 0;
			else
				bop[0] = (Close[0] - Open[0]) / (high0 - low0);

			Value[0] = sma[0];
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Smooth", "Parameters")]
		public int Smooth { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.
	public partial class Indicator
	{
		private BOP[] cacheBOP;

		public BOP BOP(DataSeries open, DataSeries high, DataSeries low, DataSeries close, int smooth)
		{
			var cat = cacheBOP;
			if (cacheBOP != null)
				for (int idx = 0; idx < cacheBOP.Length; idx++)
					if (cacheBOP[idx] != null && cacheBOP[idx].Smooth == smooth && cat[idx].High == high && cat[idx].Low == low && cat[idx].Open == open && cacheBOP[idx].EqualsInput(close))
						return cacheBOP[idx];
			return CacheIndicator<BOP>(new BOP() { Smooth = smooth, Open = open, High = high, Low = low, Input = close }, ref cacheBOP);
		}
	}

	public partial class Strategy
	{
		public BOP BOP(int smooth)
		{
			return BOP(Datas[0], smooth);
		}
		public BOP BOP(Data data, int smooth)
		{
			return Indicator.BOP(data.O, data.H, data.L, data.C, smooth);
		}
	}
}

#endregion
