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
	/// 范围指标
	/// RIND (Range Indicator) compares the intraday range (high - low) to the 
	/// inter-day (close - previous close) range. When the intraday range is greater
	/// than the inter-day range, the Range Indicator will be a high value. This
	/// signals an end to the current trend. When the Range Indicator is at a low
	/// level, a new trend is about to start.
	/// </summary>
	public class RIND : Indicator
	{
		private EMA ema;
		private Lowest min;
		private Highest max;
		private DataSeries stochRange;
		private DataSeries val1;

		internal DataSeries High, Low;
		DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
			PeriodQ = 3;
			Smooth = 10;
			Close = Input;

			stochRange = new DataSeries(Input);
			val1 = new DataSeries(Input);
			ema = EMA(stochRange, Smooth);
			min = Lowest(val1, PeriodQ);
			max = Highest(val1, PeriodQ);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				stochRange[0] = 50;
				return;
			}

			double high0 = High[0];
			double low0 = Low[0];
			double close0 = Close[0];
			double close1 = Close[1];
			double trueRange = Math.Max(high0, close1) - Math.Min(low0, close1);

			if (close0 > close1)
				val1[0] = trueRange / (close0 - close1);
			else
				val1[0] = trueRange;

			double min0 = min[0];
			double max0 = max[0];
			double val10 = val1[0];

			if ((max0 - min0) > 0)
				stochRange[0] = 100 * ((val10 - min0) / (max0 - min0));
			else
				stochRange[0] = 100 * (val10 - min0);

			Value[0] = ema[0];
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("PeriodQ")]
		public int PeriodQ { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Smooth")]
		public int Smooth { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private RIND[] cacheRIND;

		public RIND RIND(DataSeries high, DataSeries low, DataSeries close, int periodQ, int smooth)
		{
			var cat = cacheRIND;
			if (cacheRIND != null)
				for (int idx = 0; idx < cacheRIND.Length; idx++)
					if (cacheRIND[idx] != null && cacheRIND[idx].PeriodQ == periodQ && cacheRIND[idx].Smooth == smooth && cat[idx].High == high && cat[idx].Low == low && cacheRIND[idx].EqualsInput(close))
						return cacheRIND[idx];
			return CacheIndicator<RIND>(new RIND() { PeriodQ = periodQ, Smooth = smooth, High = high, Low = low, Input = close }, ref cacheRIND);
		}
	}

	public partial class Strategy
	{
		public RIND RIND(int periodQ, int smooth)
		{
			return RIND(Datas[0], periodQ, smooth);
		}

		public RIND RIND(Data data, int periodQ, int smooth)
		{
			return indicator.RIND(data.H, data.L, data.C, periodQ, smooth);
		}
	}
}

#endregion
