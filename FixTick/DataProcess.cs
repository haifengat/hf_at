using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace DataCenter
{
	public class DataProcess
	{
		//交易日历
		public List<string> TradeDates = new List<string>();
		public ConcurrentDictionary<string, Product> ProductInfo = new ConcurrentDictionary<string, Product>();
		public ConcurrentDictionary<string, Instrument> InstrumentInfo = new ConcurrentDictionary<string, Instrument>();
		public ConcurrentDictionary<string, string> Instrument888 = new ConcurrentDictionary<string, string>();
		public ConcurrentDictionary<string, double> Rate000 = new ConcurrentDictionary<string, double>();

		private ConcurrentDictionary<string, WorkingTime> _workingTimes = new ConcurrentDictionary<string, WorkingTime>();
		private ConcurrentDictionary<string, MinList> _dicProcMinList = new ConcurrentDictionary<string, MinList>();
		string _host;
		int _port;

		public DataProcess(string host = "58.247.171.146", int port = 5055)
		{
			_host = host;
			_port = port;
			UpdateInfo();
		}


		/// <summary>
		/// 20170405期权上市后不能再用“去尾数字法”取合约的productid
		/// </summary>
		/// <param name="tick"></param>
		/// <param name="tradingDay"></param>
		/// <param name="proc"></param>
		/// <returns></returns>
		public bool FixTick(HaiFeng.Tick tick, string tradingDay, string proc)
		{
			var action = TradeDates[TradeDates.IndexOf(tradingDay) - 1];
			DateTime dtTick;
			if (_dicProcMinList[proc].FixMin(tick.UpdateTime, tick.UpdateMillisec, tradingDay, action, out dtTick))
			{
				tick.UpdateTime = dtTick.ToString("yyyyMMdd HH:mm:ss");
				tick.UpdateMillisec = dtTick.Millisecond;
				return true;
			}
			return false;
		}

		public void UpdateInfo()
		{
			TradeDates = QueryDate();
			Rate000 = QueryRate000();//合约在000中的占比
			foreach (var g in QueryTime().GroupBy(n => n.GroupId))
				_workingTimes[g.Key] = g.OrderByDescending(n => n.OpenDate).First();

			foreach (var v in QueryProduct())
				ProductInfo[v._id] = v;

			foreach (var v in QueryInstrumentInfo())
				InstrumentInfo[v._id] = v;

			foreach (var v in QueryInstrument888())
				Instrument888[v._id.TrimEnd('8')] = v.value;

			//最后再处理minlist
			foreach (var item in ProductInfo)
			{
				WorkingTime wt;
				if (!_workingTimes.TryGetValue(item.Key, out wt))
					wt = _workingTimes["default"];
				_dicProcMinList[item.Key] = new MinList(wt);
			}
			Console.WriteLine("数据更新完成.");
		}

		public List<Day> QueryDay(string pInstrument, string pBegin, string pEnd)
		{
			return (List<Day>)Request(false, pInstrument, pBegin, pEnd);
		}

		public List<Min> QueryMin(string pInstrument, string pBegin, string pEnd)
		{
			return (List<Min>)Request(true, pInstrument, pBegin, pEnd);
		}

		public List<Min> QueryReal(string pInstrument)
		{
			return (List<Min>)Request(true, pInstrument, null, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pMin"></param>
		/// <param name="pInstrument"></param>
		/// <param name="pBegin">yyyyMMdd</param>
		/// <param name="pEnd">yyyyMMdd不包含</param>
		private IList Request(bool pMin, string pInstrument, string pBegin, string pEnd)
		{
			string msg = SendAndReceive(new ReqPackage
			{
				Type = pMin ? (pBegin == null ? BarType.Real : BarType.Min) : BarType.Day,
				Instrument = pInstrument,
				Begin = pBegin,
				End = pEnd
			});

			IList bars;
			if (pMin)
				bars = JsonConvert.DeserializeObject<List<Min>>(msg);
			else
				bars = JsonConvert.DeserializeObject<List<Day>>(msg);
			return bars;
		}

		private string SendAndReceive(ReqPackage r)
		{
			string msg = string.Empty;
			using (var req = new RequestSocket($">tcp://{_host}:{_port}"))
			{
				req.SendFrame(JsonConvert.SerializeObject(r));
				byte[] bs;
				if (!req.TryReceiveFrameBytes(TimeSpan.FromSeconds(10), out bs))
				{
					throw new Exception($"服务端未开启 {_host}:{_port}");
				}

				using (MemoryStream ms = new MemoryStream(bs))
				{
					using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Decompress))
					{
						byte[] bytes = new byte[1024];
						var len = 0;
						MemoryStream oms = new MemoryStream();
						ms.Position = 0;
						while ((len = zipStream.Read(bytes, 0, bytes.Length)) > 0)
							oms.Write(bytes, 0, len);
						zipStream.Close();
						msg = Encoding.UTF8.GetString(oms.ToArray());
					}
				}
			}
			return msg;
		}

		/// <summary>
		/// 获取品种信息(最小变动/合约乘数)
		/// </summary>
		/// <returns></returns>
		List<Product> QueryProduct()
		{
			string msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.Product
			});
			return JsonConvert.DeserializeObject<List<Product>>(msg);
		}

		/// <summary>
		/// 查合约信息
		/// </summary>
		/// <returns></returns>
		List<Instrument> QueryInstrumentInfo()
		{
			var msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.InstrumentInfo,
			});
			return JsonConvert.DeserializeObject<List<Instrument>>(msg);
		}

		/// <summary>
		/// 获取各品种交易时间
		/// </summary>
		/// <returns></returns>
		List<WorkingTime> QueryTime()
		{
			string msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.Time
			});
			return JsonConvert.DeserializeObject<List<WorkingTime>>(msg);
		}

		/// <summary>
		/// 获取交易日历
		/// </summary>
		/// <returns></returns>
		List<string> QueryDate()
		{
			var msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.TradeDate
			});
			var list = JsonConvert.DeserializeObject<List<string>>(msg);
			list.Sort();
			return list;
		}

		List<Instrument888> QueryInstrument888()
		{
			var msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.Instrumet888,
			});
			var list = JsonConvert.DeserializeObject<List<Instrument888>>(msg);
			return list;
		}

		ConcurrentDictionary<string, double> QueryRate000()
		{
			var msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.Rate000
			});
			return JsonConvert.DeserializeObject<ConcurrentDictionary<string, double>>(msg);
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
