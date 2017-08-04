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
	/// Chaikin Money Flow.
	/// </summary>
	public class ChaikinMoneyFlow : Indicator
	{
		DataSeries moneyFlow;
		private SUM sumMoneyFlow;
		private SUM sumVolume;

		internal DataSeries High, Low, Volume;
		DataSeries Close;

		protected override void Init()
		{
			Period = 21;
			Close = Input;

			moneyFlow = new DataSeries(this.Input);
			sumMoneyFlow = SUM(moneyFlow, Period);
			sumVolume = SUM(Volume, Period);
		}

		protected override void OnBarUpdate()
		{
			double close0 = Close[0];
			double low0 = Low[0];
			double high0 = High[0];

			moneyFlow[0] = Volume[0] * ((close0 - low0) - (high0 - close0)) / ((high0 - low0).ApproxCompare(0) == 0 ? 1 : (high0 - low0));
			Value[0] = 100 * sumMoneyFlow[0] / sumVolume[0];
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
		private ChaikinMoneyFlow[] cacheChaikinMoneyFlow;

		public ChaikinMoneyFlow ChaikinMoneyFlow(DataSeries high, DataSeries low, DataSeries close, DataSeries volume, int period)
		{
			var cat = cacheChaikinMoneyFlow;
			if (cacheChaikinMoneyFlow != null)
				for (int idx = 0; idx < cacheChaikinMoneyFlow.Length; idx++)
					if (cacheChaikinMoneyFlow[idx] != null && cacheChaikinMoneyFlow[idx].Period == period && cat[idx].High == high && cat[idx].Low == low&&cat[idx].Volume==volume && cacheChaikinMoneyFlow[idx].EqualsInput(close))
						return cacheChaikinMoneyFlow[idx];
			return CacheIndicator<ChaikinMoneyFlow>(new ChaikinMoneyFlow() { Period = period, High = high, Low = low, Volume = volume, Input = close }, ref cacheChaikinMoneyFlow);
		}
	}

	public partial class Strategy
	{
		public ChaikinMoneyFlow ChaikinMoneyFlow(int period)
		{
			return ChaikinMoneyFlow(Datas[0], period);
		}
		public ChaikinMoneyFlow ChaikinMoneyFlow(Data data, int period)
		{
			return Indicator.ChaikinMoneyFlow(data.H, data.L, data.C, data.V, period);
		}
	}
}

#endregion
