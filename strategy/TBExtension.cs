using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public static partial class TBExtension
	{
		public static int TBBarStatus(this Strategy stra)
		{
			return TBBarStatus(stra.Datas[0]);
		}

		public static int TBBarStatus(this Data data)
		{
			return data.Count <= 1 ? 0 : string.IsNullOrEmpty(data.Tick.InstrumentID) ? 1 : 2;
		}

		public static double TBHighest(this Strategy stra, DataSeries input, int Begin, int Length) { return input.Highest(Begin, Length); }
		public static double TBLowest(this Strategy stra, DataSeries input, int Begin, int Length) { return input.Lowest(Begin, Length); }
		public static double TBAverage(this Strategy stra, DataSeries input, int Begin, int Length) { return input.Average(Begin, Length); }
	}


}
