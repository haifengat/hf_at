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
	/// Fisher Transform. The Fisher Transform has sharp and distinct turning points
	/// that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
	/// </summary>
	public class FisherTransform : Indicator
	{
		private Highest max;
		private Lowest min;
		private DataSeries tmpSeries;

		protected override void Init()
		{
			Period = 10;
			PriceTick = 1;

			max = Highest(Input, Period);
			min = Lowest(Input, Period);
			tmpSeries = new DataSeries(Input);
		}

		protected override void OnBarUpdate()
		{
			double fishPrev = 0;
			double tmpValuePrev = 0;

			if (CurrentBar > 0)
			{
				fishPrev = Value[1];
				tmpValuePrev = tmpSeries[1];
			}

			double minLo = min[0];
			double num1 = max[0] - minLo;

			// Guard against infinite numbers and div by zero
			num1 = (num1 < PriceTick / 10 ? PriceTick / 10 : num1);
			double tmpValue = 0.66 * ((Input[0] - minLo) / num1 - 0.5) + 0.67 * tmpValuePrev;

			if (tmpValue > 0.99)
				tmpValue = 0.999;
			else if (tmpValue < -0.99)
				tmpValue = -0.999;

			tmpSeries[0] = tmpValue;
			Value[0] = 0.5 * Math.Log((1 + tmpValue) / (1 - tmpValue)) + 0.5 * fishPrev;
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Period")]
		public int Period { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("PriceTick")]
		public double PriceTick { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private FisherTransform[] cacheFisherTransform;

		public FisherTransform FisherTransform(DataSeries input, int period, double pricetick)
		{
			if (cacheFisherTransform != null)
				for (int idx = 0; idx < cacheFisherTransform.Length; idx++)
					if (cacheFisherTransform[idx] != null && cacheFisherTransform[idx].Period == period && cacheFisherTransform[idx].EqualsInput(input))
						return cacheFisherTransform[idx];
			return CacheIndicator<FisherTransform>(new FisherTransform() { Period = period, PriceTick = pricetick, Input = input }, ref cacheFisherTransform);
		}
	}

	public partial class Strategy
	{
		public FisherTransform FisherTransform(DataSeries input, int period)
		{
			return indicator.FisherTransform(input, period, PriceTick);
		}
	}
}

#endregion
