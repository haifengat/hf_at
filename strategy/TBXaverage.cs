using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TBXaverage : Indicator
	{
		/*sFcactor = 2 / ( Length + 1 );
	if (CurrentBar == 0 )
	{
		XAvgValue = Price;
	}else
	{
		XAvgValue = XAvgValue[1] + sFcactor * ( Price - XAvgValue[1] ) ;
	}	*/

		double sFcactor;
		protected override void Init()
		{
			sFcactor = 2.0 / (Period + 1);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value[0] = Input[0];
			else
				Value[0] = Value[1] + sFcactor * (Input[0] - Value[1]);
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

	public static partial class TBExtension
	{ 
		private static TBXaverage[] cacheXAverage;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public static TBXaverage TBXAverage(this Indicator indicator, DataSeries input, int period)
		{
			if (cacheXAverage != null)
				for (int idx = 0; idx < cacheXAverage.Length; idx++)
					if (cacheXAverage[idx] != null && cacheXAverage[idx].Period == period && cacheXAverage[idx].EqualsInput(input))
						return cacheXAverage[idx];
			return indicator.CacheIndicator(new TBXaverage() { Period = period, Input = input }, ref cacheXAverage);
		}

		public static TBXaverage TBXAverage(this Strategy stra, DataSeries input, int period)
		{
			return stra.indicator.TBXAverage(input, period);
		}
	}
}
