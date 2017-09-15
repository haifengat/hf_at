using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public static class TBExtension
	{
		public static int GetBarStatus(this Data data)
		{
			return data.Count <= 1 ? 0 : string.IsNullOrEmpty(data.Tick.InstrumentID) ? 1 : 2;
		}
	}
}
