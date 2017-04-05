using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HaiFeng
{
	public class DataProcess
	{
		//交易日历
		List<string> _tradeDates = new List<string>();
		public ConcurrentDictionary<string, Product> ProductInfo = new ConcurrentDictionary<string, Product>();
		private ConcurrentDictionary<string, WorkingTime> _workingTimes = new ConcurrentDictionary<string, WorkingTime>();
		ConcurrentDictionary<string, MinList> _dicProcMinList = new ConcurrentDictionary<string, MinList>();
		GetData _getData = null;

		public DataProcess(string host = "58.247.171.146", int port = 5055)
		{
			_getData = new GetData(host, port);
			UpdateInfo();
		}

		public GetData GetData { get { return _getData; } }

		/// <summary>
		/// 20170405期权上市后不能再用“去尾数字法”取合约的productid
		/// </summary>
		/// <param name="tick"></param>
		/// <param name="tradingDay"></param>
		/// <param name="proc"></param>
		/// <returns></returns>
		public bool FixTick(MarketData tick, string tradingDay, string proc)
		{
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

		private void UpdateInfo()
		{
			foreach (var g in _getData.QueryTime().GroupBy(n => n.GroupId))
				_workingTimes[g.Key] = g.OrderByDescending(n => n.OpenDate).First();
			foreach (var v in _getData.QueryProduct())
				ProductInfo[v._id] = v;
			_tradeDates = _getData.QueryDate();

			foreach (var item in ProductInfo)
			{
				WorkingTime wt;
				if (!_workingTimes.TryGetValue(item.Key, out wt))
					wt = _workingTimes["default"];
				_dicProcMinList[item.Key] = new MinList(wt);
			}
			Console.WriteLine("数据更新完成.");
		}

		//品种对应的交易时段内的分钟序列
		class MinList
		{
			public MinList(WorkingTime wtg)
			{
				ListTrading = new List<string>();
				ListAction = new List<string>();
				ListBegin = new List<string>();
				ListEnd = new List<string>();
				SetWorkTimeGroup(wtg);
			}

			internal List<string> ListBegin;
			internal List<string> ListAction;
			internal List<string> ListEnd;
			internal List<string> ListTrading;

			void SetWorkTimeGroup(WorkingTime wtg)
			{
				foreach (var tr in wtg.WorkingTimes)
				{
					var begin = TimeSpan.Parse(tr.Begin);
					var end = TimeSpan.Parse(tr.End);
					ListBegin.Add(begin.Add(TimeSpan.FromMinutes(-1)).ToString(@"hh\:mm"));
					ListEnd.Add(tr.End.Substring(0, 5));

					if (tr.End.CompareTo(tr.Begin) < 0)
						end = end.Add(TimeSpan.FromDays(1));

					var listAdd = ListTrading; //实际要添加的list=>影响使用action还是tradingday
					if (tr.IsNight) //夜盘
					{
						listAdd = ListAction;
						if (tr.End.CompareTo(tr.Begin) < 0) //隔夜数据:结果加1天
							end = TimeSpan.Parse(tr.End).Add(TimeSpan.FromDays(1)); //结束时间加1天
					}

					while (begin < end)
					{
						listAdd.Add(begin.ToString(@"hh\:mm"));
						begin = begin.Add(TimeSpan.FromMinutes(1));
					}
				}
			}

			internal bool FixMin(string updateTime, int ms, string tradingDay, string actionDay, out DateTime dt)
			{
				dt = new DateTime();
				//时间过滤
				var t5 = updateTime.Substring(0, 5);
				if (ListBegin.IndexOf(t5) >= 0)
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
}
