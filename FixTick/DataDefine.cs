using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCenter
{
	enum BarType
	{
		Min, Day, Real, Time, Product, TradeDate, InstrumentInfo, Instrumet888
	}


	class ReqPackage
	{
		public BarType Type { get; set; }
		public string Instrument { get; set; }
		public string Begin { get; set; }
		public string End { get; set; }
	}

	public class Min
	{
		/// <summary>
		/// yyyyMMdd HH:mm:ss
		/// </summary>
		public string _id;
		public string TradingDay { get; set; }
		public double Open { get; set; }
		public double High { get; set; }
		public double Low { get; set; }
		public double Close { get; set; }
		public int Volume { get; set; }
		public double OpenInterest { get; set; }
	}

	public class Day
	{
		public string _id { get; set; }
		public double Open { get; set; }
		public double High { get; set; }
		public double Low { get; set; }

		public double Close { get; set; }
		public int Volume { get; set; }
		public double OpenInterest { get; set; }
		public double UpperLimit { get; set; }
		public double LowerLimit { get; set; }
		public double Settlement { get; set; }
	}

	public class Product
	{
		public string _id { get; set; }
		public double PriceTick { get; set; }
		public int VolumeTuple { get; set; }
		public string ExchangeID { get; set; }
		public string ProductType { get; set; }
	}


	public class WorkingTime
	{
		public object _id { get; set; }
		/// <summary>
		/// 品种ID
		/// </summary>
		[DisplayName("品种")]
		public string GroupId { get; set; }
		/// <summary>
		/// 开始日期[此交易时间段开始的时间]
		/// </summary>
		[DisplayName("开始日期")]
		public string OpenDate { get; set; }
		/// <summary>
		/// 多个交易时间段
		/// </summary>
		[DisplayName("交易时间"), Browsable(false)]
		public List<TimeRange> WorkingTimes { get; set; } = new List<TimeRange>();
		/// <summary>
		/// 是否夜盘开始交易
		/// </summary>
		/// <returns></returns>
		[DisplayName("是否有夜盘")]
		public bool NightOpen() { return WorkingTimes == null ? false : WorkingTimes[0].IsNight; }
	}
	public class TimeRange
	{
		[DisplayName("开始时间")]
		public string Begin { get; set; }
		[DisplayName("结束时间")]
		public string End { get; set; }
		[DisplayName("夜盘小节")]
		public bool IsNight { get; set; }
		[DisplayName("收盘小节")]
		public bool IsClose { get; set; }
		[DisplayName("开盘小节")]
		public bool IsOpen { get; set; }

		public override string ToString()
		{
			return string.Concat(Begin, "-", End);
		}

		internal TimeRange Clone()
		{
			return (TimeRange)MemberwiseClone();
		}
	}
	/// <summary>
	/// 合约信息
	/// </summary>
	public class Instrument
	{
		public string _id;
		public string ProductID;
	}
	/// <summary>
	/// 主力合约信息
	/// </summary>
	public class Instrument888
	{
		/// <summary>
		/// xx888
		/// </summary>
		public string _id;
		public string value;
	}
}
