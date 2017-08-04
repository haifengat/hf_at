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
	/// 极化分形效率指标
	/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal
	///  geometry to determine how efficiently the price is moving.
	/// </summary>
	public class PFE : Indicator
	{
		private DataSeries div;
		private EMA ema;
		private DataSeries pfeSeries;
		private DataSeries singlePfeSeries;

		protected override void Init()
		{
			Period = 14;
			Smooth = 10;

			div = new DataSeries(Input);
			pfeSeries = new DataSeries(Input);
			singlePfeSeries = new DataSeries(Input);
			ema = EMA(pfeSeries, Smooth);
		}

		protected override void OnBarUpdate()
		{
			double input0 = Input[0];

			if (CurrentBar < Period)
			{
				singlePfeSeries[0] = CurrentBar == 0 ? 1 : Math.Sqrt(Math.Pow((Input[1] - input0), 2) + 1);
				div[0] = singlePfeSeries[0] + (CurrentBar > 0 ? div[1] : 0);
				return;
			}

			double input1 = Input[1];
			double inputPeriod = Input[Period];

			singlePfeSeries[0] = Math.Sqrt(Math.Pow((input1 - input0), 2) + 1);
			div[0] = singlePfeSeries[0] + div[1] - singlePfeSeries[Period];
			pfeSeries[0] = (input0 < inputPeriod ? -1 : 1) * (Math.Sqrt(Math.Pow(input0 - inputPeriod, 2) + Math.Pow(Period, 2)) / div[0]);
			Value[0] = ema[0];
		}

		#region Properties
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
		private PFE[] cachePFE;

		public PFE PFE(DataSeries input, int period, int smooth)
		{
			if (cachePFE != null)
				for (int idx = 0; idx < cachePFE.Length; idx++)
					if (cachePFE[idx] != null && cachePFE[idx].Period == period && cachePFE[idx].Smooth == smooth && cachePFE[idx].EqualsInput(input))
						return cachePFE[idx];
			return CacheIndicator<PFE>(new PFE() { Period = period, Smooth = smooth, Input = input }, ref cachePFE);
		}
	}

	public partial class Strategy
	{

		public PFE PFE(DataSeries input, int period, int smooth)
		{
			return Indicator.PFE(input, period, smooth);
		}
	}
}

#endregion
