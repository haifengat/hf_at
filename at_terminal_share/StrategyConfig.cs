using System;
using System.Collections.Generic;
using System.Text;

namespace HaiFeng
{
    class StrategyConfig
    {
		public string Name { get; set; }
		public Type Type { get; set; }
		public string Instrument { get; set; }
		public string InstrumentOrder { get; set; }
		public string Interval { get; set; }
		public DateTime BeginDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Params { get; set; }
	}
}
