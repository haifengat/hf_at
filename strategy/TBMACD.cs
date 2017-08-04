using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TBMACD : Indicator
	{/*
Params
	Numeric FastLength(12);
	Numeric SlowLength(26);
	Numeric MACDLength(9);
Vars
	NumericSeries MACDValue; 
	Numeric AvgMACD;
	Numeric MACDDiff;
Begin
End*/


		protected override void Init()
		{
		}

		protected override void OnBarUpdate()
		{
			/*
	MACDValue = XAverage( Close, FastLength ) - XAverage( Close, SlowLength ) ;	
	AvgMACD = XAverage(MACDValue,MACDLength);
	MACDDiff = MACDValue - AvgMACD;
	PlotNumeric("MACD",MACDValue);
	PlotNumeric("MACDAvg",AvgMACD);
	If (MACDDiff >= 0)	
		PlotNumeric("MACDDiff",MACDDiff,0,Red); 
	Else
		PlotNumeric("MACDDiff",MACDDiff,0,Green); 
	PlotNumeric("零线",0); 	 */

			var fast = this.TBXAverage(Input, FastLength);
			var slow = this.TBXAverage(Input, SlowLength);
			var avg = this.TBXAverage(MACDValue, MACDLength);

			MACDValue[0] = fast[0] - slow[0];
			AvgMACD[0] = avg[0];
			MACDDiff[0] = MACDValue[0] - AvgMACD[0];
		}

		#region
		public int FastLength { get; set; } = 12;
		public int SlowLength { get; set; } = 26;
		public int MACDLength { get; set; } = 9;
		public DataSeries MACDValue { get { return Values[0]; } }
		public DataSeries AvgMACD { get { return Values[1]; } }
		public DataSeries MACDDiff { get { return Values[2]; } }
		#endregion
	}

	public static partial class TBExtension
	{
		private static TBMACD[] cacheTBMACD;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public static TBMACD TBMACD(this Indicator indicator, DataSeries input, int fast, int slow, int length)
		{
			if (cacheTBMACD != null)
				for (int idx = 0; idx < cacheTBMACD.Length; idx++)
					if (cacheTBMACD[idx] != null && cacheTBMACD[idx].FastLength == fast && cacheTBMACD[idx].SlowLength == slow && cacheTBMACD[idx].MACDLength == length && cacheTBMACD[idx].EqualsInput(input))
						return cacheTBMACD[idx];
			return indicator.CacheIndicator(new TBMACD() { FastLength = fast, SlowLength = slow, MACDLength = length, Input = input }, ref cacheTBMACD);
		}

		public static TBMACD TBMACD(this Strategy stra, int fast, int slow, int length)
		{
			return stra.Indicator.TBMACD(stra.C, fast, slow, length);
		}
	}
}
