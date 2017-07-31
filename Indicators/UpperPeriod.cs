using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HaiFeng
{
	public class UpperPeriod : Indicator
	{
		DataSeries close;
		Bar bar;
		private bool newbar = false;

		protected override void Init()
		{
			close = Inputs[0];
		}

		protected override void OnBarUpdate()
		{
			DateTime dtBegin = DateTime.MaxValue;
			switch (IntervalType)
			{
				case EnumIntervalType.Sec:
					dtBegin = dtBegin.Date.AddHours(MinBar.D.Hour).AddMinutes(MinBar.D.Minute).AddSeconds(MinBar.D.Second / Interval * Interval);
					break;
				case EnumIntervalType.Min:
					dtBegin = dtBegin.Date.AddHours(MinBar.D.Hour).AddMinutes(MinBar.D.Minute / Interval * Interval);
					break;
				case EnumIntervalType.Hour:
					dtBegin = dtBegin.Date.AddHours(MinBar.D.Hour / Interval * Interval);
					break;
				case EnumIntervalType.Day:
					dtBegin = DateTime.ParseExact(MinBar.TradingDay.ToString(), "yyyyMMdd", null);
					break;
				case EnumIntervalType.Week:
					dtBegin = DateTime.ParseExact(MinBar.TradingDay.ToString(), "yyyyMMdd", null);
					dtBegin = dtBegin.Date.AddDays(1 - (byte)dtBegin.DayOfWeek);
					break;
				case EnumIntervalType.Month:
					dtBegin = DateTime.ParseExact(MinBar.TradingDay.ToString(), "yyyyMMdd", null);
					dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, 1);
					break;
				case EnumIntervalType.Year:
					dtBegin = DateTime.ParseExact(MinBar.TradingDay.ToString(), "yyyyMMdd", null);
					dtBegin = new DateTime(dtBegin.Year, 1, 1);
					break;
				default:
					throw new Exception("参数错误");
			}
			if (bar == null) //首次调用
			{
				bar = new Bar
				{
					D = dtBegin,
					I = MinBar.I,
					V = MinBar.V, // kOld.preVol == 0 ? 0 : _tick.Volume - kOld.preVol;
				};
				bar.H = MinBar.H;
				bar.L = MinBar.L;
				bar.O = MinBar.O;
				bar.C = MinBar.C;
				newbar = true;
			}
			else
			{
				if (bar.D == dtBegin) //在当前K线范围内
				{
					newbar = false;
					bar.H = Math.Max(bar.H, MinBar.H);
					bar.L = Math.Min(bar.L, MinBar.L);
					bar.C = MinBar.C;
					bar.V = bar.V + MinBar.V;
					bar.I = MinBar.I;
				}
				else if (dtBegin > bar.D)
				{
					newbar = true;
					bar.D = dtBegin;
					bar.V = MinBar.V;
					bar.I = MinBar.I;
					bar.O = MinBar.O;
					bar.H = MinBar.H;
					bar.L = MinBar.L;
					bar.C = MinBar.C;
				}
			}

			if (newbar)
			{
				Date.Add(double.Parse(bar.D.ToString("yyyyMMdd")));
				Time.Add(double.Parse(bar.D.ToString("0.HHmmss")));
				Open.Add(bar.O);
				High.Add(bar.H);
				Low.Add(bar.L);
				Close.Add(bar.C);
				Volume.Add(bar.V);
				OpenInterest.Add(bar.I);
			}
			else
			{
				High[0] = bar.H;
				Low[0] = bar.L;
				Close[0] = bar.C;
				Volume[0] = bar.V;
				OpenInterest[0] = bar.I;
			}
		}


		public DataSeries Date { get; set; } = new DataSeries();
		public DataSeries Time { get; set; } = new DataSeries();
		public DataSeries Open { get; set; } = new DataSeries();
		public DataSeries High { get; set; } = new DataSeries();
		public DataSeries Low { get; set; } = new DataSeries();
		public DataSeries Close { get; set; } = new DataSeries();
		public DataSeries Volume { get; set; } = new DataSeries();
		public DataSeries OpenInterest { get; set; } = new DataSeries();


		#region Properties
		public Bar MinBar { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Interval")]
		public int Interval { get; set; } = 5;

		[Range(1, int.MaxValue)]
		[Parameter("IntervalType")]
		public EnumIntervalType IntervalType { get; set; } = EnumIntervalType.Min;
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private UpperPeriod[] cacheUpperPeriod;

		public UpperPeriod UpperPeriod(DataSeries close, Bar curMinBar, int interval, EnumIntervalType type)
		{
			if (cacheUpperPeriod != null)
				for (int idx = 0; idx < cacheUpperPeriod.Length; idx++)
					if (cacheUpperPeriod[idx] != null && cacheUpperPeriod[idx].Interval == interval && cacheUpperPeriod[idx].IntervalType == type && cacheUpperPeriod[idx].EqualsInput(close))
						return cacheUpperPeriod[idx];
			return CacheIndicator<UpperPeriod>(new UpperPeriod() { Interval = interval, IntervalType = type, MinBar = curMinBar, Inputs = new[] { close } }, ref cacheUpperPeriod);
		}
	}
	#endregion

	public partial class Strategy
	{
		public UpperPeriod UpperPeriod(int inteval, EnumIntervalType type)
		{
			return UpperPeriod(Datas[0], inteval, type);
		}

		public UpperPeriod UpperPeriod(Data data, int inteval, EnumIntervalType type)
		{
			return indicator.UpperPeriod(data.C, data.CurrentMinBar, inteval, type);
		}
	}
}
