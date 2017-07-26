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
		DataSeries Open, High, Low, Close;

		protected override void Init()
		{
			Smooth = 14;
			Open = Inputs[0];
			High = Inputs[1];
			Low = Inputs[1];
			Close = Inputs[2];
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
			if (cacheBOP != null)
				for (int idx = 0; idx < cacheBOP.Length; idx++)
					if (cacheBOP[idx] != null && cacheBOP[idx].Smooth == smooth && cacheBOP[idx].EqualsInput(open, high, low, close))
						return cacheBOP[idx];
			return CacheIndicator<BOP>(new BOP() { Smooth = smooth, Inputs = new[] { open, high, low, close } }, ref cacheBOP);
		}
	}

	public partial class Strategy
	{
		public BOP BOP(int smooth)
		{
			return indicator.BOP(O, H, L, C, smooth);
		}
	}
}

#endregion
