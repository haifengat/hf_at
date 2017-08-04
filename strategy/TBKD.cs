using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TBKD : Indicator
	{/*	HighestValue = HighestFC(High, Length);
	LowestValue = LowestFC(Low, Length); 
	SumHLValue = SummationFC(HighestValue-LowestValue,SlowLength);
	SumCLValue = SummationFC(Close - LowestValue,SlowLength);
	If(SumHLValue <> 0)
	{
		KValue = SumCLValue/SumHLValue*100;
	}Else
	{
		KValue = 0;
	}
	DValue = AverageFC(KValue,SmoothLength);*/

		internal DataSeries High, Low;
		DataSeries Close;
		DataSeries HL, CL;
		protected override void Init()
		{
			Close = Input;
			HL = new DataSeries(Close);
			CL = new DataSeries(Close);
		}

		protected override void OnBarUpdate()
		{
			var highest = High.Highest(0, Length);
			var lowest = Low.Lowest(0, Length);
			HL[0] = highest - lowest;
			CL[0] = Close[0] - lowest;
			var sumHL = this.TBSum(HL, SlowLength);
			var sumCL = this.TBSum(CL, SlowLength);
			if (sumHL[0] != 0)
				KValue[0] = sumCL[0] / sumHL[0] * 100;
			else
				KValue[0] = 0;
			DValue[0] = KValue.Average(0, SmoothLength);
		}

		#region
		public int Length { get; set; } = 14;
		public int SlowLength { get; set; } = 3;
		public int SmoothLength { get; set; } = 3;

		public DataSeries KValue { get { return Values[0]; } }
		public DataSeries DValue { get { return Values[1]; } }
		#endregion
	}


	public static partial class TBExtension
	{
		private static TBKD[] cacheTBKD;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public static TBKD TBKD(this Indicator indicator, DataSeries high, DataSeries low, DataSeries close, int length, int slow, int smooth)
		{
			if (cacheTBKD != null)
				for (int idx = 0; idx < cacheTBKD.Length; idx++)
					if (cacheTBKD[idx] != null && cacheTBKD[idx].Length == length && cacheTBKD[idx].SlowLength == slow && cacheTBKD[idx].SmoothLength == smooth && cacheTBKD[idx].High == high && cacheTBKD[idx].Low == low && cacheTBKD[idx].EqualsInput(close))
						return cacheTBKD[idx];
			return indicator.CacheIndicator(new TBKD() { Length = length, SlowLength = slow, SmoothLength = smooth, High = high, Low = low, Input = close }, ref cacheTBKD);
		}

		public static TBKD TBKD(this Strategy stra, int length, int slow, int smooth)
		{
			return stra.Indicator.TBKD(stra.H, stra.L, stra.C, length, slow, smooth);
		}
	}
}
