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
	/// Keltner Channel. The Keltner Channel is a similar indicator to Bollinger Bands. 
	/// Here the midline is a standard moving average with the upper and lower bands offset
	/// by the SMA of the difference between the high and low of the previous bars. 
	/// The offset multiplier as well as the SMA period is configurable.
	/// </summary>
	public class KeltnerChannel : Indicator
	{
		private DataSeries diff;
		private SMA smaDiff;
		private SMA smaTypical;
		DataSeries High, Low, Close, Typical;

		protected override void Init()
		{
			Period = 10;
			OffsetMultiplier = 1.5;
			High = Inputs[0];
			Low = Inputs[1];
			Close = Inputs[2];

			Typical = new DataSeries(Input);

			diff = new DataSeries(Input);
			smaDiff = SMA(diff, Period);
			smaTypical = SMA(Typical, Period);
		}

		protected override void OnBarUpdate()
		{
			Typical[0] = (High[0] + Low[0] + Close[0]) / 3;

			diff[0] = High[0] - Low[0];

			double middle = smaTypical[0];
			double offset = smaDiff[0] * OffsetMultiplier;

			double upper = middle + offset;
			double lower = middle - offset;

			Midline[0] = middle;
			Upper[0] = upper;
			Lower[0] = lower;
		}

		#region Properties
		[Browsable(false)]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		public DataSeries Midline
		{
			get { return Values[0]; }
		}

		[Range(0.01, int.MaxValue)]
		[Parameter("OffsetMultiplier")]
		public double OffsetMultiplier { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }

		[Browsable(false)]
		public DataSeries Upper
		{
			get { return Values[1]; }
		}
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private KeltnerChannel[] cacheKeltnerChannel;

		public KeltnerChannel KeltnerChannel(DataSeries high, DataSeries low, DataSeries close, double offsetMultiplier, int period)
		{
			if (cacheKeltnerChannel != null)
				for (int idx = 0; idx < cacheKeltnerChannel.Length; idx++)
					if (cacheKeltnerChannel[idx] != null && cacheKeltnerChannel[idx].OffsetMultiplier == offsetMultiplier && cacheKeltnerChannel[idx].Period == period && cacheKeltnerChannel[idx].EqualsInput(high, low, close))
						return cacheKeltnerChannel[idx];
			return CacheIndicator<KeltnerChannel>(new KeltnerChannel() { OffsetMultiplier = offsetMultiplier, Period = period, Inputs = new[] { high, low, close } }, ref cacheKeltnerChannel);
		}
	}

	public partial class Strategy
	{
		public KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
		{
			return KeltnerChannel(Datas[0], offsetMultiplier, period);
		}
		public KeltnerChannel KeltnerChannel(Data data, double offsetMultiplier, int period)
		{
			return indicator.KeltnerChannel(data.H, data.L, data.C, offsetMultiplier, period);
		}
	}
}

#endregion
