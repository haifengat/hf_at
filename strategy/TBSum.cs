using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TBSum : Indicator
	{/*	If(CurrentBar < Length)
	{
		SumValue = 0;
		for i = 0 to Length - 1
		{
			SumValue = SumValue + Price[i];
		}
	}Else
	{
		SumValue = SumValue[1] + Price - Price[Length];
	}
	Return SumValue;*/

		protected override void Init()
		{
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < Length)
			{
				Value[0] = 0;
				for (int i = 0; i < Math.Min(Input.Count, Length - 1); i++)
				{
					Value[0] += Input[i];
				}
			}
			else
			{
				Value[0] = Value[1] + Input[0] - Input[Length];
			}
		}

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Parameter("Length", "Parameters")]
		public int Length { get; set; } = 14;
		#endregion
	}


	public static partial class TBExtension
	{
		private static TBSum[] cacheTBSum;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public static TBSum TBSum(this Indicator indicator, DataSeries input, int period)
		{
			if (cacheTBSum != null)
				for (int idx = 0; idx < cacheTBSum.Length; idx++)
					if (cacheTBSum[idx] != null && cacheTBSum[idx].Length == period && cacheTBSum[idx].EqualsInput(input))
						return cacheTBSum[idx];
			return indicator.CacheIndicator(new TBSum() { Length = period, Input = input }, ref cacheTBSum);
		}

		public static TBSum TBSum(this Strategy stra, DataSeries input, int period)
		{
			return stra.indicator.TBSum(input, period);
		}
	}
}
