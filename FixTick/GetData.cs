using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace HaiFeng
{
	public class GetData
	{
		private int _port = 5555;
		private string _host = "localhost";// "58.247.171.146";

		public GetData(string host = "58.247.171.146", int port = 5055) { _host = host; _port = port; }

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
		public List<Product> QueryProduct()
		{
			string msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.Product
			});
			return JsonConvert.DeserializeObject<List<Product>>(msg);
		}

		/// <summary>
		/// 获取各品种交易时间
		/// </summary>
		/// <returns></returns>
		public List<WorkingTime> QueryTime()
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
		public List<string> QueryDate()
		{
			var msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.TradeDate
			});
			var list = JsonConvert.DeserializeObject<List<string>>(msg);
			list.Sort();
			return list;
		}

		/// <summary>
		/// 查合约
		/// </summary>
		/// <returns></returns>
		public List<string> QueryInstrument()
		{
			var msg = SendAndReceive(new ReqPackage
			{
				Type = BarType.Instrument,
			});
			var list = JsonConvert.DeserializeObject<List<string>>(msg);
			list.Sort();
			return list;
		}
	}
}
