
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


using Numeric = System.Decimal;

namespace HaiFeng
{
	/// <summary>
	/// 	数据
	/// </summary>
	public class Data : Collection<Bar> //: CustomTypeDescriptor
	{
		private CollectionChange _onChange;
		/// <summary>
		/// 策略变化:加1;减-1;更新0
		/// </summary>
		public event CollectionChange OnChanged
		{
			add
			{
				_onChange += value;
			}
			remove
			{
				_onChange -= value;
			}
		}

		#region 数据序列

		/// <summary>
		/// 	时间(yyyyMMdd.HHmmss)
		/// </summary>
		public DataSeries D = new DataSeries();

		/// <summary>
		/// 	开盘价
		/// </summary>
		public DataSeries O = new DataSeries();

		/// <summary>
		/// 	最高价
		/// </summary>
		public DataSeries H = new DataSeries();

		/// <summary>
		/// 	最低价
		/// </summary>
		public DataSeries L = new DataSeries();

		/// <summary>
		/// 	收盘价
		/// </summary>
		public DataSeries C = new DataSeries();

		/// <summary>
		/// 	成交量
		/// </summary>
		public DataSeries V = new DataSeries();

		/// <summary>
		/// 	持仓量
		/// </summary>
		public DataSeries I = new DataSeries();

		/// <summary>
		/// 	均价
		/// </summary>
		public DataSeries A = new DataSeries();
		#endregion
		
		/// <summary>
		/// 	构造函数
		/// </summary>
		public Data()
		{
			this.Tick = new Tick();
			//this.InstrumentInfo = new InstrumentField();
		}

		#region Properties
		/// <summary>
		/// 	实际行情(无数据时为Instrument== null)
		/// </summary>
		[Description("分笔数据"), Category("数据"), Browsable(false)]
		public Tick Tick { get; set; }

		/// <summary>
		/// 	合约信息
		/// </summary>
		[Description("合约信息"), Category("数据"), Browsable(false)]
		public InstrumentInfo InstrumentInfo { get; set; }

		/// <summary>
		/// 	合约
		/// </summary>
		[Description("合约"), Category("配置")]
		public string Instrument { get; set; } = string.Empty;

		/// <summary>
		/// 	委托合约
		/// </summary>
		[Description("合约"), Category("配置")]
		public string InstrumentOrder { get; set; } = string.Empty;

		/// <summary>
		/// 	周期类型
		/// </summary>
		[Description("周期类型"), Category("配置")]
		public EnumIntervalType IntervalType { get; set; } = EnumIntervalType.Min;

		/// <summary>
		/// 	周期数
		/// </summary>
		[Description("周期数"), Category("配置")]
		public int Interval { get; set; } = 5;

		/// <summary>
		/// 	当前K线索引(由左向右从0开始)
		/// </summary>
		[Description("当前K线索引"), Category("设计"), Browsable(false)]
		public int CurrentBar { get { return Count == 0 ? 0 : (Count - 1); } }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public string Name { get { return this.Instrument + "_" + this.Interval + "_" + this.IntervalType; } }

		/// <summary>
		/// 被tick行情调用
		/// </summary>
		/// <param name="f"></param>
		/// <exception cref="Exception"></exception>
		public void OnTick(Tick f)
		{
			if (this.Instrument != f.InstrumentID)
				return;
			#region 生成or更新K线
			DateTime dt = DateTime.ParseExact(f.UpdateTime, "yyyyMMdd HH:mm:ss", null);
			DateTime dtBegin = dt.Date;
			//foreach (var data in this.Datas.Where(n => n.Instrument == f.InstrumentID))
			{
				switch (IntervalType)
				{
					case EnumIntervalType.Sec:
						dtBegin = dtBegin.Date.AddHours(dt.Hour).AddMinutes(dt.Minute).AddSeconds(dt.Second / Interval * Interval);
						break;
					case EnumIntervalType.Min:
						dtBegin = dtBegin.Date.AddHours(dt.Hour).AddMinutes(dt.Minute / Interval * Interval);
						break;
					case EnumIntervalType.Hour:
						dtBegin = dtBegin.Date.AddHours(dt.Hour / Interval * Interval);
						break;
					case EnumIntervalType.Day:
						dtBegin = dtBegin.Date;
						break;
					case EnumIntervalType.Week:
						dtBegin = dtBegin.Date.AddDays(1 - (byte)dtBegin.DayOfWeek);
						break;
					case EnumIntervalType.Month:
						dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, 1);
						break;
					case EnumIntervalType.Year:
						dtBegin = new DateTime(dtBegin.Year, 1, 1);
						break;
					default:
						throw new Exception("参数错误");
				}
				if (Count == 0) //无数据
				{
					Bar bar = new Bar
					{
						D = dtBegin,
						PreVol = f.Volume,
						I = f.OpenInterest,
						A = f.AveragePrice,
						V = 0 // kOld.preVol == 0 ? 0 : _tick.Volume - kOld.preVol;
					};
					bar.H = bar.L = bar.O = bar.C = f.LastPrice;
					Add(bar);
				}
				else
				{
					Bar bar = this[CurrentBar];
					//if (bar == null)	//特殊处理,未找到为null的原因
					//{
					//	RemoveLast();
					//}
					//bar = this[CurrentBar];
					if (bar.D == dtBegin) //在当前K线范围内
					{
						bar.H = Math.Max(bar.H, f.LastPrice);
						bar.L = Math.Min(bar.L, f.LastPrice);
						bar.C = f.LastPrice;
						bar.V = bar.V + f.Volume - bar.PreVol; //此处可能有问题
						bar.PreVol = f.Volume;
						bar.I = f.OpenInterest;
						bar.A = f.AveragePrice;

						this[CurrentBar] = bar;	//更新
					}
					else if (dtBegin > bar.D)
					{
						Bar di = new Bar
						{
							D = dtBegin,
							//V = Math.Abs(bar.PreVol - 0) < 1E-06 ? 0 : f.Volume - bar.PreVol,
							V = f.Volume - bar.PreVol,
							PreVol = f.Volume,
							I = f.OpenInterest,
							A = f.AveragePrice,
							O = f.LastPrice,
							H = f.LastPrice,
							L = f.LastPrice,
							C = f.LastPrice
						};
						Add(di);
					}
				}
				Tick = f; //更新最后的tick
			}
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Bar item)
		{
			D.Add(Numeric.Parse(item.D.ToString("yyyyMMdd.HHmmss")));
			O.Add(item.O);
			H.Add(item.H);
			L.Add(item.L);
			C.Add(item.C);
			V.Add(item.V);
			I.Add(item.I);
			A.Add(item.A);
			base.InsertItem(index, item);
			if (_onChange != null)
			{
				_onChange(1, item, item);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, Bar item)
		{
			Bar old = this[index];
			H[CurrentBar - index] = item.H;
			L[CurrentBar - index] = item.L;
			C[CurrentBar - index] = item.C;
			V[CurrentBar - index] = item.V;
			I[CurrentBar - index] = item.I;
			A[CurrentBar - index] = item.A;
			base.SetItem(index, item);
			if (_onChange != null)
			{
				_onChange(0, old, item);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			Bar old = this[index];
			if (D.Count == Count)
			{
				D.RemoveAt(index);
				O.RemoveAt(index);
				H.RemoveAt(index);
				L.RemoveAt(index);
				C.RemoveAt(index);
				V.RemoveAt(index);
				I.RemoveAt(index);
				A.RemoveAt(index);
			}
			base.RemoveItem(index);
			if (_onChange != null)
			{
				_onChange(-1, old, old);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		protected override void ClearItems()
		{
			D.Clear();
			O.Clear();
			H.Clear();
			L.Clear();
			C.Clear();
			V.Clear();
			I.Clear();
			A.Clear();
			base.ClearItems();
		}
	}
}
