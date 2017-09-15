using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	class UpMins : Strategy
	{
		public override void Initialize()
		{
			var up = UpperPeriod(5, EnumIntervalType.Min);
			Date5 = up.Date;
			Time5 = up.Time;
			Open5 = up.Open;
			High5 = up.High;
			Low5 = up.Low;
			Close5 = up.Close;
			Vol5 = up.Volume;
			OI5 = up.OpenInterest;
		}

		DataSeries Date5, Time5, Open5, High5, Low5, Close5, Vol5, OI5;

		public override void OnBarUpdate()
		{
			Console.WriteLine($"{Date5[0]} {Time5[0]}, {Open5[0]},{High5[0]},{Low5[0]},{Close5[0]},{Vol5[0]},{OI5[0]}");
			Console.WriteLine($"{Date[0]} {Time[0]}, {Open[0]},{High[0]},{Low[0]},{Close[0]},{Volume[0]},{OpenInterest[0]}");
		}
	}
}
