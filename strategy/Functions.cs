using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numeric = System.Decimal;

namespace HaiFeng
{
	static class Functions
	{
		static public Numeric Highest(DataSeries pSeries, Numeric pPeriod, int pPreBars = 0)
		{
			//_inds.GetOrAdd(string.Format("Highest_{0}_{1}", pSeries.SeriesName, pPeriod), new Highest(pSeries, (int) pPeriod));
			Numeric rtn = Numeric.MinValue;
			for (int i = 0; i < pPeriod; ++i)
			{
				if (pSeries[pPreBars + i] > rtn)
					rtn = pSeries[pPreBars + i];
			}
			return rtn;
		}
		static public Numeric Lowest(DataSeries pSeries, Numeric pPeriod, int pPreBars = 0)
		{
			Numeric rtn = Numeric.MaxValue;
			for (int i = 0; i < pPeriod; ++i)
			{
				if (pSeries[pPreBars + i] < rtn)
					rtn = pSeries[pPreBars + i];
			}
			return rtn;
		}
	}
}
