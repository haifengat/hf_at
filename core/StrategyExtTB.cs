using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Numeric = System.Decimal;

namespace HaiFeng
{
	/// <summary>
	/// TB适应性封装
	/// </summary>
	public abstract partial class Strategy : CustomTypeDescriptor
	{
		//Open = O; High = H; Low = L; Close = C; Vol = V; OpenInt = I;
		public DataSeries Open { get { return O; } }
		public DataSeries High { get { return H; } }
		public DataSeries Low { get { return L; } }
		public DataSeries Close { get { return C; } }
		public DataSeries Vol { get { return V; } }
		public DataSeries OpenInt { get { return I; } }

		public Numeric Highest(DataSeries pSeries, Numeric pPeriod, int pPreBars = 0)
		{
			//_inds.GetOrAdd(string.Format("Highest_{0}_{1}", pSeries.SeriesName, pPeriod), new Highest(pSeries, (int) pPeriod));
			Numeric rtn = Numeric.MinValue;
			for (int i = pPreBars; i < pPeriod; ++i)
			{
				if (pSeries[i] > rtn)
					rtn = pSeries[i];
			}
			return rtn;
		}
		public Numeric Lowest(DataSeries pSeries, Numeric pPeriod, int pPreBars = 0)
		{
			Numeric rtn = Numeric.MaxValue;
			for (int i = pPreBars; i < pPeriod; ++i)
			{
				if (pSeries[i] < rtn)
					rtn = pSeries[i];
			}
			return rtn;
		}

		private readonly Numeric[] _global = new Numeric[256];

		public void SetGlobalVar(int p, Numeric Time)
		{
			_global[p] = Time;
		}

		public void SetGlobalVar(int p, int p1)
		{
			_global[p] = p1;
		}

		public decimal GetGlobalVar(int p)
		{
			return _global[p];
		}

		public void Commentary(string msg)
		{ }

		public string Text(Numeric num)
		{
			return num.ToString();
		}

		#region 数学函数
		public Numeric Min(Numeric x, Numeric y) { return Math.Min(x, y); }
		public Numeric Max(Numeric x, Numeric y) { return Math.Max(x, y); }
		public Numeric Abs(Numeric number) { return Math.Abs(number); }// 返回参数的绝对值。

		public Numeric Acos(Numeric number) { return (Numeric)Math.Acos((double)number); }// 返回参数的反余弦值。

		public Numeric Acosh(Numeric number) { return (Numeric)Math.Cosh((double)number); }// 返回参数的反双曲余弦值。

		public Numeric Asin(Numeric number) { return (Numeric)Math.Asin((double)number); }// 返回参数的反正弦值。

		public Numeric Asinh(Numeric number) { return (Numeric)Math.Sinh((double)number); }// 返回参数的反双曲正弦值。

		public Numeric Atan(Numeric number) { return (Numeric)Math.Atan((double)number); }// 返回参数的反正切值。

		public Numeric Atan2(Numeric x_number, Numeric y_number) { return (Numeric)Math.Atan2((double)x_number, (double)y_number); }// 返回给定的X及Y坐标值的反正切值。

		public Numeric Atanh(Numeric number) { return (Numeric)Math.Tanh((double)number); }// 返回参数的反双曲正切值。

		public Numeric Ceiling(Numeric number) { return Math.Ceiling(number); }// 将参数 Number 沿绝对值增大的方向，舍入为最接近的整数或基数Significance的最小倍数。

		//public Numeric Combin(Numeric number){ return Math.Combin(number); }// 计算从给定数目的对象集合中提取若干对象的组合数。

		public Numeric Cos(Numeric number) { return (Numeric)Math.Cos((double)number); }// 返回给定角度的余弦值。

		public Numeric Cosh(Numeric number) { return (Numeric)Math.Cosh((double)number); }// 返回参数的双曲余弦值。

		public Numeric Ctan(Numeric number) { return (Numeric)Math.Tan((double)number); }// 返回给定角度的余切值。

		//public Numeric Even(Numeric number){ return Math.Even(number); }// 返回沿绝对值增大方向取整后最接近的偶数。

		public Numeric Exp(Numeric number) { return (Numeric)Math.Exp((double)number); }// 返回e的Number次幂。

		public Numeric Fact(Numeric number) { return (Numeric)Math.Log10((double)number); }// 返回数的阶乘。

		public Numeric Floor(Numeric number) { return (Numeric)Math.Floor((double)number); }// 将参数 Number 沿绝对值减小的方向去尾舍入，使其等于最接近的 Significance 的倍数。

		//public Numeric FracPart(Numeric number){ return Math.FracPart(number); }// 返回实数舍入后的小数值。

		public Numeric IntPart(Numeric number) { return (Numeric)Math.Truncate((double)number); }// 返回实数舍入后的整数值。

		public Numeric Ln(Numeric number) { return (Numeric)Math.Log((double)number, Math.E); }// 返回一个数的自然对数。

		public Numeric Log(Numeric number, double Base) { return (Numeric)Math.Log((double)number, (double)Base); }// 按所指定的底数，返回一个数的对数。

		public Numeric Mod(Numeric number, int Divisor) { return (Numeric)Math.IEEERemainder((double)number, Divisor); }// 返回两数相除的余数。

		public Numeric Neg(Numeric number) { return -Math.Abs((Numeric)number); }// 返回参数的负绝对值。

		//public Numeric Odd(Numeric number){ return double Math.Odd(number); }// 返回对指定数值进行舍入后的奇数。

		public Numeric Pi(Numeric number) { return (Numeric)Math.PI; }// 返回数字3.1415926535898。

		public Numeric Power(Numeric number, Numeric Power) { return (Numeric)Math.Pow((double)number, (double)Power); }// 返回给定数字的乘幂。

		public Numeric Rand(Numeric number) { return (Numeric)new Random((int)number).NextDouble(); }// 返回位于两个指定数之间的一个随机数。

		public Numeric Round(Numeric number) { return (Numeric)Math.Round((double)number); }// 返回某个数字按指定位数舍入后的数字。

		public Numeric RoundDown(Numeric number) { return (Numeric)Math.Ceiling((double)number); }// 靠近零值，向下（绝对值减小的方向）舍入数字。

		public Numeric RoundUp(Numeric number) { return (Numeric)Math.Floor((Numeric)number); }// 远离零值，向上（绝对值增大的方向）舍入数字。

		public Numeric Sign(Numeric number) { return (Numeric)Math.Sign((Numeric)number); }// 返回数字的符号。

		public Numeric Sin(Numeric number) { return (Numeric)Math.Sin((double)number); }// 返回给定角度的正弦值。

		public Numeric Sinh(Numeric number) { return (Numeric)Math.Sinh((double)number); }// 返回某一数字的双曲正弦值。

		public Numeric Sqr(Numeric number) { return (Numeric)number * (Numeric)number; }// 返回参数的平方。

		public Numeric Sqrt(Numeric number) { return (Numeric)Math.Sqrt((double)number); }// 返回参数的正平方根。

		public Numeric Tan(Numeric number) { return (Numeric)Math.Tan((double)number); }// 返回给定角度的正切值。
		public Numeric Tanh(Numeric number) { return (Numeric)Math.Tanh((double)number); }// 返回某一数字的双曲正切值。
		#endregion

		private DataSeries _OpenD = new DataSeries();
		private DataSeries _HighD = new DataSeries();
		private DataSeries _LowD = new DataSeries();
		private DataSeries _CloseD = new DataSeries();

		public Numeric OpenD(Numeric pPre)
		{
			return _OpenD[(int)pPre];
		}
		public Numeric HighD(Numeric pPre)
		{
			return _HighD[(int)pPre];
		}
		public Numeric LowD(Numeric pPre)
		{
			return _LowD[(int)pPre];
		}
		public Numeric CloseD(Numeric pPre)
		{
			return _CloseD[(int)pPre];
		}
		public class DataSeriesDate : Collection<DateTime>
		{
			internal string SeriesName
			{
				get { return _name; }
			}

			/// <summary>
			/// 	指标中的DataSeries在被取值时this[n]运算指标
			/// </summary>
			//internal Indicator Idc = null;

			private readonly string _name;

			/// <summary>
			/// 构建函数(参数勿填)
			/// </summary>
			/// <param name="pSeriesName"></param>
			public DataSeriesDate([CallerMemberName] string pSeriesName = null)
			{
				//根据变量名，赋值member
				//this.SeriesName = new StackTrace(true).GetFrame(1).GetMethod().Name; // pSeriesName; .Net4下结果不正确(.cto)
				_name = pSeriesName;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public new DateTime this[int index]
			{
				get
				{
					DateTime rtn = DateTime.MinValue;
					//if (this.Idc != null)// && !this.idc.IsOperated)
					//{
					//	//在策略调用时处理:此处会导致循环调用
					//	//this.idc.isUpdated = false;
					//	this.Idc.update();// .OnBarUpdate();
					//}
					if (Count - 1 - index >= 0)
					{
						rtn = base[Count - 1 - index];
					}
					return rtn;
				}
				set
				{
					if (Count - 1 - index < 0)
					{
						return;
					}
					base[Count - 1 - index] = value;
				}
			}
		}

		#region 跨周期
		public ConcurrentDictionary<Tuple<PriceType, int, PeriodType>, DataSeries> DicPeriodValue = new ConcurrentDictionary<Tuple<PriceType, int, PeriodType>, DataSeries>();
		public ConcurrentDictionary<Tuple<PriceType, int, PeriodType>, DataSeriesDate> DicPeriodTime = new ConcurrentDictionary<Tuple<PriceType, int, PeriodType>, DataSeriesDate>();

		//internal ConcurrentDictionary<Tuple<PriceType, int, PeriodType, DateTime>, Numeric> periodLog = new ConcurrentDictionary<Tuple<PriceType, int, PeriodType, DateTime>, Numeric>();

		/// <summary>
		/// 跨周期引用
		/// </summary>
		/// <param name="priceType"></param>
		/// <param name="interval">周期数</param>
		/// <param name="type">周期类型:周/月/年时interval无效</param>
		/// <returns></returns>
		public DataSeries PeriodUpper(PriceType priceType, int interval = 1, PeriodType type = PeriodType.Day)
		{
			DataSeries rtn = new DataSeries();
			DicPeriodValue.TryAdd(new Tuple<PriceType, int, PeriodType>(priceType, interval, type), rtn);
			DicPeriodTime.TryAdd(new Tuple<PriceType, int, PeriodType>(priceType, interval, type), new DataSeriesDate());
			return rtn;
		}

		void periodUpper()
		{
			//Parallel.ForEach(this.dicPeriodValue, item =>
			foreach (var item in this.DicPeriodValue)
			{
				DataSeries tmp = null;
				switch (item.Key.Item1)
				{
					case PriceType.H:
						tmp = this.H;
						break;
					case PriceType.L:
						tmp = this.L;
						break;
					case PriceType.O:
						tmp = this.O;
						break;
					case PriceType.C:
						tmp = this.C;
						break;
					case PriceType.V:
						tmp = this.V;
						break;
					case PriceType.I:
						tmp = this.I;
						break;
				}
				DateTime dt0 = DateTime.ParseExact(this.D[0].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);
				DateTime dtID;
				switch (item.Key.Item3)
				{
					case PeriodType.Minute:
						dtID = new DateTime(dt0.Year, dt0.Month, dt0.Day, dt0.Hour, dt0.Minute, 0);
						if (this.DicPeriodTime[item.Key].Count == 0 || (dt0.Minute % item.Key.Item2 == 0 && this.DicPeriodTime[item.Key][0] != dtID))
						{
							this.DicPeriodTime[item.Key].Add(dtID);
							item.Value.Add(tmp[0]);
						}
						else
							updatePeriodUpper(item.Key.Item1, item.Value);
						break;
					case PeriodType.Hour:
						dtID = new DateTime(dt0.Year, dt0.Month, dt0.Day, dt0.Hour, 0, 0);

						if (this.DicPeriodTime[item.Key].Count == 0 || this.DicPeriodTime[item.Key][0] != dtID)
						{
							this.DicPeriodTime[item.Key].Add(dtID);
							item.Value.Add(tmp[0]);
						}
						else
							updatePeriodUpper(item.Key.Item1, item.Value);
						break;
					case PeriodType.Day:
						dtID = new DateTime(dt0.Year, dt0.Month, dt0.Day);

						if (this.DicPeriodTime[item.Key].Count == 0 || this.DicPeriodTime[item.Key][0] != dtID)
						{
							this.DicPeriodTime[item.Key].Add(dtID);
							item.Value.Add(tmp[0]);
						}
						else
							updatePeriodUpper(item.Key.Item1, item.Value);
						break;
					case PeriodType.Week:
						dtID = dt0.AddDays(1 - (int)dt0.DayOfWeek);
						dtID = new DateTime(dtID.Year, dtID.Month, dtID.Day);

						if (this.DicPeriodTime[item.Key].Count == 0 || this.DicPeriodTime[item.Key][0] != dtID)
						{
							this.DicPeriodTime[item.Key].Add(dtID);
							item.Value.Add(tmp[0]);
						}
						else
						{
							updatePeriodUpper(item.Key.Item1, item.Value);
						}
						break;
					case PeriodType.Month:
						dtID = new DateTime(dt0.Year, dt0.Month, 1);

						if (this.DicPeriodTime[item.Key].Count == 0 || this.DicPeriodTime[item.Key][0] != dtID)
						{
							this.DicPeriodTime[item.Key].Add(dtID);
							item.Value.Add(tmp[0]);
						}
						else
						{
							updatePeriodUpper(item.Key.Item1, item.Value);
						}
						break;
					case PeriodType.Year:
						dtID = new DateTime(dt0.Year, 1, 1);

						if (this.DicPeriodTime[item.Key].Count == 0 || this.DicPeriodTime[item.Key][0] != dtID)
						{
							this.DicPeriodTime[item.Key].Add(dtID);
							item.Value.Add(tmp[0]);
							break;      //不执行update
						}
						else
						{
							updatePeriodUpper(item.Key.Item1, item.Value);
						}
						break;
				}
			}//);
		}
		void updatePeriodUpper(PriceType priceType, DataSeries input)
		{
			switch (priceType)
			{
				case PriceType.H:
					input[0] = Math.Max(input[0], this.H[0]);
					break;
				case PriceType.L:
					input[0] = Math.Min(input[0], this.L[0]);
					break;
				case PriceType.O:
					//tmp = this.O;
					break;
				case PriceType.C:
					input[0] = this.C[0];
					break;
				case PriceType.V:
					input[0] += this.V[0];
					break;
				case PriceType.I:
					input[0] = this.I[0];
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public enum PeriodType
		{
			/// <summary>
			/// 
			/// </summary>
			Tick,
			/// <summary>
			/// 
			/// </summary>
			Second,
			/// <summary>
			/// 
			/// </summary>
			Minute,
			/// <summary>
			/// 
			/// </summary>
			Hour,
			/// <summary>
			/// 
			/// </summary>
			Day,
			/// <summary>
			/// 
			/// </summary>
			Week,
			/// <summary>
			/// 
			/// </summary>
			Month,
			/// <summary>
			/// 
			/// </summary>
			Year
		}
		/// <summary>
		/// 
		/// </summary>
		public enum PriceType
		{
			/// <summary>
			/// 
			/// </summary>
			H,
			/// <summary>
			/// 
			/// </summary>
			L,
			/// <summary>
			/// 
			/// </summary>
			O,
			/// <summary>
			/// 
			/// </summary>
			C,
			/// <summary>
			/// 
			/// </summary>
			V,
			/// <summary>
			/// 
			/// </summary>
			I
		}
		#endregion


		public Numeric MinMove = 1, PriceScale = 1, CurrentTime;
		//public DataSeries Open, High, Low, Close, Vol, OpenInt, Date;
		public DataSeries Date, Time;
		public Numeric BarInterval, MarketPosition, BarsSinceLastEntry, AvgEntryPrice, BarsSinceEntry, EntryPrice;

		public int BarStatus;
		public string FormulaName = string.Empty;

		public decimal A_FreeMargin;
		//TB关键字转换:实时
		void TBKey()
		{
			//QA函数
			A_FreeMargin = 1000000;


			BarInterval = this.Interval;
			MarketPosition = this.PositionNet == 0 ? 0 : (this.PositionNet > 0 ? 1 : -1);
			if (PositionNet > 0)
			{
				EntryPrice = this.EntryPriceLong;
				BarsSinceLastEntry = this.BarsSinceLastEntryLong;
				AvgEntryPrice = this.AvgEntryPriceLong;
				BarsSinceEntry = this.BarsSinceEntryLong;
			}
			else
			{
				EntryPrice = this.EntryPriceShort;
				BarsSinceLastEntry = this.BarsSinceLastEntryShort;
				AvgEntryPrice = this.AvgEntryPriceShort;
				BarsSinceEntry = this.BarsSinceEntryShort;
			}
			Date[0] = Math.Truncate(this.D[0]);
			Time[0] = this.D[0] - Date[0];
			CurrentTime = Numeric.Parse(DateTime.Now.ToString("0.HHmmss"));
			if (!string.IsNullOrEmpty(this.Tick.InstrumentID))
				BarStatus = 2;
			else
				BarStatus = CurrentBar == 0 ? 0 : 1;
		}

		//TB引用转换:初始化
		void TBInit()
		{
			FormulaName = this.Name;
			_OpenD = PeriodUpper(PriceType.O, 1, PeriodType.Day);
			_HighD = PeriodUpper(PriceType.H, 1, PeriodType.Day);
			_LowD = PeriodUpper(PriceType.L, 1, PeriodType.Day);
			_CloseD = PeriodUpper(PriceType.C, 1, PeriodType.Day);


			if (InstrumentInfo.PriceTick >= 1)
				MinMove = (int)InstrumentInfo.PriceTick;
			else
			{
				PriceScale = (Numeric)Math.Pow(10, -Math.Round(InstrumentInfo.PriceTick, 6).ToString().TrimEnd('0').Split('.')[1].Length);
				MinMove = (int)(InstrumentInfo.PriceTick / PriceScale);
			}
			//Date = D;
		}
	}
}
