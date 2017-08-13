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
	/// The Sum shows the summation of the last n data points.
	/// </summary>
	public class SUM : Indicator
	{
		protected override void Init()
		{
		}

		protected override void OnBarUpdate()
		{
			Value[0] = Input[0] + (CurrentBar > 0 ? Value[1] : 0) - (CurrentBar >= Period ? Input[Period] : 0);
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
		private SUM[] cacheSUM;

		public SUM SUM(DataSeries input, int period)
		{
			if (cacheSUM != null)
				for (int idx = 0; idx < cacheSUM.Length; idx++)
					if (cacheSUM[idx] != null && cacheSUM[idx].Period == period && cacheSUM[idx].EqualsInput(input))
						return cacheSUM[idx];
			return CacheIndicator<SUM>(new SUM() { Period = period, Input = input }, ref cacheSUM);
		}
	}

	public partial class Strategy
	{
		public SUM SUM(DataSeries input, int period)
		{
			return Indicator.SUM(input, period);
		}
	}
}

#endregion
