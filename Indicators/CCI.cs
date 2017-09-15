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
	/// The Commodity Channel Index (CCI) measures the variation of a security's price 
	/// from its statistical mean. High values show that prices are unusually high 
	/// compared to average prices whereas low values indicate that prices are unusually low.
	/// </summary>
	public class CCI : Indicator
	{
		private SMA sma;
		DataSeries Typical, High, Low, Close;

		protected override void Init()
		{
			Period = 14;
			High = Inputs[0];
			Low = Inputs[1];
			Close = Inputs[2];
			Typical = new DataSeries(this.Input);
			sma = SMA(Typical, Period);
		}

		protected override void OnBarUpdate()
		{
			Typical[0] = (High[0] + Low[0] + Close[0]) / 3;
			if (CurrentBar == 0)
				Value[0] = 0;
			else
			{
				double mean = 0;
				double sma0 = sma[0];

				for (int idx = Math.Min(CurrentBar, Period - 1); idx >= 0; idx--)
					mean += Math.Abs(Typical[idx] - sma0);

				Value[0] = (Typical[0] - sma0) / (mean.ApproxCompare(0) == 0 ? 1 : (0.015 * (mean / Math.Min(Period, CurrentBar + 1))));
			}
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.
	public partial class Indicator
	{
		private CCI[] cacheCCI;

		public CCI CCI(DataSeries high, DataSeries low, DataSeries close, int period)
		{
			if (cacheCCI != null)
				for (int idx = 0; idx < cacheCCI.Length; idx++)
					if (cacheCCI[idx] != null && cacheCCI[idx].Period == period && cacheCCI[idx].EqualsInput(high, low, close))
						return cacheCCI[idx];
			return CacheIndicator<CCI>(new CCI() { Period = period, Inputs = new[] { high, low, close } }, ref cacheCCI);
		}
	}

	public partial class Strategy
	{
		public CCI CCI(int period)
		{
			return CCI(Datas[0], period);
		}
		public CCI CCI(Data data, int period)
		{
			return indicator.CCI(data.H, data.L, data.C, period);
		}
	}
}

#endregion
