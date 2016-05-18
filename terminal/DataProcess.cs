using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HaiFeng
{
	public class DataProcess
	{
		public ConcurrentDictionary<string, ProductInfo> ProductInfo = new ConcurrentDictionary<string, ProductInfo>();

		private ConcurrentDictionary<string, WorkingTimeGroup> _workingTimes = new ConcurrentDictionary<string, WorkingTimeGroup>();
		private List<string> _tradeDates = new List<string>();
		ConcurrentDictionary<string, MinList> _dicProcMinList = new ConcurrentDictionary<string, MinList>();

		public DataProcess()
		{
		}

		public bool FixTick(MarketData tick, string tradingDay)
		{
			var proc = new string(tick.InstrumentID.Where(c => !char.IsDigit(c)).ToArray());
			var action = _tradeDates[_tradeDates.IndexOf(tradingDay) - 1];
			DateTime dtTick;
			if (_dicProcMinList[proc].FixMin(tick.UpdateTime, tick.UpdateMillisec, tradingDay, action, out dtTick))
			{
				tick.UpdateTime = dtTick.ToString("yyyyMMdd HH:mm:ss");
				tick.UpdateMillisec = dtTick.Millisecond;
				return true;
			}
			return false;
		}

		private MongoServer GetMongoServer(string mongoUrl, string user, string pwd)
		{
			//var strconn = "mongodb://reader:reader2015@192.168.105.203:27017/?authenticationDatabase=admin";
			var serverAddress = mongoUrl.IndexOf(':') >= 0 ? new MongoServerAddress(mongoUrl.Split(':')[0], short.Parse(mongoUrl.Split(':')[1])) : new MongoServerAddress(mongoUrl);

			var sett = new MongoServerSettings
			{
				Server = serverAddress,
			};
			if (!string.IsNullOrEmpty(user))
				sett.Credentials = new[] { MongoCredential.CreateCredential("admin", user, pwd) };
			return new MongoServer(sett);
		}

		public void UpdateInfo(string mongoUrl, string user = null, string pwd = null)
		{
			var db = GetMongoServer(mongoUrl, user, pwd).GetDatabase("future_config");
			//tradingdate
			foreach (var v in db.GetCollection("TradingDate").FindAll().SetSortOrder("_id"))
				if (v["IsTrading"].AsBoolean)
					_tradeDates.Add(v["_id"].ToUniversalTime().ToLocalTime().ToString("yyyyMMdd"));
			//WorkingTime
			foreach (var v in db.GetCollection("WorkingTime").FindAllAs<WorkingTimeGroup>())
				_workingTimes[v._id] = v;
			//productinfo
			foreach (var v in db.GetCollection("ProductInfo").FindAllAs<ProductInfo>())
				ProductInfo[v._id] = v;
			foreach (var item in ProductInfo)
			{
				WorkingTimeGroup wt;
				if (!_workingTimes.TryGetValue(item.Key, out wt))
					wt = _workingTimes["default"];
				_dicProcMinList[item.Key] = new MinList(wt);
			}
			Console.WriteLine("数据更新完成.");
		}
		
		//品种对应的交易时段内的分钟序列
		public class MinList
		{
			public MinList(WorkingTimeGroup wtg)
			{
				ListTrading = new List<string>();
				ListAction = new List<string>();
				Begin = string.Empty;
				ListEnd = new List<string>();
				SetWorkTimeGroup(wtg);
			}

			internal string Begin;
			internal List<string> ListAction;
			internal List<string> ListEnd;
			internal List<string> ListTrading;

			void SetWorkTimeGroup(WorkingTimeGroup wtg)
			{
				foreach (var tr in wtg.WorkingTimes)
				{
					TimeSpan ts = tr.Beign;
					if (tr.IsOpen)
						Begin = ts.Add(TimeSpan.FromMinutes(-1)).ToString(@"hh\:mm"); //开盘前1分钟
					var end = tr.End;
					var listAdd = ListTrading; //实际要添加的list=>影响使用action还是tradingday
					if (tr.IsNight) //夜盘
					{
						listAdd = ListAction;
						end = tr.End.Add(TimeSpan.FromDays(1)); //结束时间加1天
					}
					do
					{
						listAdd.Add(ts.ToString(@"hh\:mm"));
						ts = ts.Add(TimeSpan.FromMinutes(1));
					} while (ts < end); //夜盘时间未加入 ....=> 增加547行
					ListEnd.Add(ts.ToString(@"hh\:mm"));
				}
			}

			internal bool FixMin(string updateTime, int ms, string tradingDay, string actionDay, out DateTime dt)
			{
				dt = new DateTime();
				//时间过滤
				var t5 = updateTime.Substring(0, 5);
				if (Begin == t5)
				{
					updateTime = TimeSpan.ParseExact(t5, @"hh\:mm", null).Add(TimeSpan.FromMinutes(1)).ToString(@"hh\:mm\:ss");
					ms = 0;
				}
				else if (ListEnd.IndexOf(t5) >= 0)
				{
					updateTime = TimeSpan.ParseExact(ListEnd[ListEnd.IndexOf(t5)], @"hh\:mm", null).Add(TimeSpan.FromMinutes(-1)).ToString(@"hh\:mm\:ss");
					ms = 999;
				}
				t5 = updateTime.Substring(0, 5);//更新

				string day = string.Empty;
				if (ListAction.IndexOf(t5) >= 0)
				{//actionday--第2天(不一定是tradingDay)
					if (t5.CompareTo(ListAction[0]) < 0) //<21:00:00表示已经是第2天
						day = DateTime.ParseExact(actionDay, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
					else
						day = actionDay;
				}
				else if (ListTrading.IndexOf(t5) >= 0)
				{
					day = tradingDay;
				}
				else
					return false;
				dt = DateTime.ParseExact(day, "yyyyMMdd", null).Add(TimeSpan.Parse(updateTime)).AddMilliseconds(ms);
				return true;
			}
		}
	}
	#region 配置相关类


	public class ProductInfo
	{
		[DisplayName("品种标识")]
		public string _id { get; set; }
		[DisplayName("最小变动")]
		public double PriceTick { get; set; }
		[DisplayName("合约乘数")]
		public double VolumeTuple { get; set; }
		[DisplayName("交易所")]
		public string ExchangeID { get; set; }
		[DisplayName("品种类型")]
		public string ProductType { get; set; }
	}

	public class WorkingTimeGroup
	{
		/// <summary>
		/// 品种ID
		/// </summary>
		[DisplayName("品种")]
		public string _id { get; set; }
		/// <summary>
		/// 开始日期[此交易时间段开始的时间]
		/// </summary>
		[DisplayName("开始日期")]
		public DateTime OpenDate { get; set; } = new DateTime(1900, 1, 1);
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
		public TimeRange()
		{
		}
		[DisplayName("开始时间")]
		public TimeSpan Beign { get; set; } = new TimeSpan(9, 0, 0);
		[DisplayName("结束时间")]
		public TimeSpan End { get; set; } = new TimeSpan(11, 30, 0);
		[DisplayName("夜盘小节")]
		public bool IsNight { get; set; }
		[DisplayName("收盘小节")]
		public bool IsClose { get; set; }
		[DisplayName("开盘小节")]
		public bool IsOpen { get; set; }

		public override string ToString()
		{
			return string.Concat(Beign, "-", End);
		}

		internal TimeRange Clone()
		{
			return (TimeRange)MemberwiseClone();
		}
	}
	#endregion
}
