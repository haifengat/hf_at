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
	/// 唐奇安通道指标
	/// Donchian Channel. The Donchian Channel indicator was created by Richard Donchian.
	///  It uses the highest high and the lowest low of a period of time to plot the channel.
	/// </summary>
	public class DonchianChannel : Indicator
	{
		private Highest max;
		private Lowest min;
		DataSeries High, Low;

		protected override void Init()
		{
			Period = 14;
			High = Inputs[0];
			Low = Inputs[1];

			max = Highest(High, Period);
			min = Lowest(Low, Period);
		}

		protected override void OnBarUpdate()
		{
			double max0 = max[0];
			double min0 = min[0];

			Value[0] = (max0 + min0) / 2;
			Upper[0] = max0;
			Lower[0] = min0;
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }

		[Browsable(false)]
		public DataSeries Lower { get { return Values[2]; } }

		[Browsable(false)]
		public DataSeries Mean { get { return Values[0]; } }

		[Browsable(false)]
		public DataSeries Upper { get { return Values[1]; } }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private DonchianChannel[] cacheDonchianChannel;

		public DonchianChannel DonchianChannel(DataSeries high, DataSeries low, int period)
		{
			if (cacheDonchianChannel != null)
				for (int idx = 0; idx < cacheDonchianChannel.Length; idx++)
					if (cacheDonchianChannel[idx] != null && cacheDonchianChannel[idx].Period == period && cacheDonchianChannel[idx].EqualsInput(high, low))
						return cacheDonchianChannel[idx];
			return CacheIndicator<DonchianChannel>(new DonchianChannel() { Period = period, Inputs = new[] { high, low } }, ref cacheDonchianChannel);
		}
	}

	public partial class Strategy
	{
		public DonchianChannel DonchianChannel(int period)
		{
			return indicator.DonchianChannel(H, L, period);
		}
	}
}

#endregion
