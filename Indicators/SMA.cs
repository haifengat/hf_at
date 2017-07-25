using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class SMA : Indicator
	{
		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			Period = 14;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value[0] = Input[0];
			else
			{
				double last = Value[1] * Math.Min(CurrentBar, Period);

				if (CurrentBar >= Period)
					Value[0] = (last + Input[0] - Input[Period]) / Math.Min(CurrentBar, Period);
				else
					Value[0] = ((last + Input[0]) / (Math.Min(CurrentBar, Period) + 1));
			}
		}


		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}

	public partial class Indicator
	{
		private SMA[] cacheSMA;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public SMA SMA(DataSeries input, int period)
		{
			if (cacheSMA != null)
				for (int idx = 0; idx < cacheSMA.Length; idx++)
					if (cacheSMA[idx] != null && cacheSMA[idx].Period == period && cacheSMA[idx].EqualsInput(input))
						return cacheSMA[idx];
			return CacheIndicator(new SMA { Period = period, Input = input }, ref cacheSMA);
		}
	}

	public partial class Strategy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public SMA SMA(DataSeries input, int period)
		{
			return indicator.SMA(input, period);
		}
	}
}
