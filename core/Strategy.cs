
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Numeric = System.Decimal;

namespace HaiFeng
{
	/// <summary>
	/// 	策略
	/// </summary>
	[DefaultProperty("Name")]
	public abstract class Strategy : CustomTypeDescriptor
	{
		#region TB封装
		//Open = O; High = H; Low = L; Close = C; Vol = V; OpenInt = I;
		public DataSeries Open { get { return O; } }
		public DataSeries High { get { return H; } }
		public DataSeries Low { get { return L; } }
		public DataSeries Close { get { return C; } }
		public DataSeries Vol { get { return V; } }
		public DataSeries OpenInt { get { return I; } }
		
		public Numeric Highest(DataSeries pSeries, Numeric pPeriod)
		{
			//_inds.GetOrAdd(string.Format("Highest_{0}_{1}", pSeries.SeriesName, pPeriod), new Highest(pSeries, (int) pPeriod));
			Numeric rtn = Numeric.MinValue;
			for (int i = 0; i < pPeriod; ++i)
			{
				if (pSeries[i] > rtn)
					rtn = pSeries[i];
			}
			return rtn;
		}
		public Numeric Lowest(DataSeries pSeries, Numeric pPeriod)
		{
			Numeric rtn = Numeric.MaxValue;
			for (int i = 0; i < pPeriod; ++i)
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
							break;		//不执行update
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
			MarketPosition = this.PositionNet;
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
				PriceScale = 10 ^ -int.Parse(InstrumentInfo.PriceTick.ToString("F6").Split('.')[1]);
				MinMove = (int)(InstrumentInfo.PriceTick / PriceScale);
			}
			//Date = D;
		}

		#endregion

		/// <summary>
		/// 	用户自定义序列
		/// </summary>
		private readonly List<DataSeries> _costomSeries = new List<DataSeries>();

		/// <summary>
		/// 指标
		/// </summary>
		private readonly List<Indicator> _indicators = new List<Indicator>();

		private string _name = string.Empty;

		/// <summary>
		/// </summary>
		protected Strategy()
		{
			Datas = new List<Data>();
			this.StrategyDatas = new List<StrategyData>();

			//处理参数
			DicProperties.Clear();
			foreach (var v in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
			{
				object[] ps = v.GetCustomAttributes(typeof(ParameterAttribute), false);
				if (ps.Length <= 0)
				{
					continue;
				}
				ParameterAttribute pa = (ParameterAttribute)ps[0];
				Property p = new Property(v.Name, v.GetValue(this)) { Category = pa.Category, Description = pa.Description };
				Add(p);
			}
		}

		/// <summary>
		/// 	策略数据集合
		/// </summary>
		[Browsable(false)]
		internal List<StrategyData> StrategyDatas { get; set; }

		/// <summary>
		/// 策略所用的数据序列
		/// </summary>
		[Browsable(false)]
		public List<Data> Datas { get; private set; }

		/// <summary>
		/// 	报单操作
		/// </summary>
		[Description("报单操作列表"), Category("交易")]
		[Browsable(false)]
		public List<OrderItem> Operations { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Operations; } }

		public List<Indicator> Indicators { get { return _indicators; } }

		#region 策略状态:对data[0]的引用

		/// <summary>
		/// 	当前持仓手数:多
		/// </summary>
		[Description("当前持仓手数:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionLong { get { return this.StrategyDatas[0].PositionLong; } }

		/// <summary>
		/// 	当前持仓手数:空
		/// </summary>
		[Description("当前持仓手数:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionShort { get { return this.StrategyDatas[0].PositionShort; } }

		/// <summary>
		/// 	当前持仓手数:净
		/// </summary>
		[Description("当前持仓手数:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionNet { get { return this.StrategyDatas[0].PositionNet; } }

		/// <summary>
		/// 	当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryDateLong { get { return this.StrategyDatas[0].EntryDateLong; } }

		/// <summary>
		/// 	当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryDateShort { get { return this.StrategyDatas[0].EntryDateShort; } }

		/// <summary>
		/// 	当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryDateLong { get { return this.StrategyDatas[0].LastEntryDateLong; } }

		/// <summary>
		/// 	当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryDateShort { get { return this.StrategyDatas[0].LastEntryDateShort; } }

		/// <summary>
		/// 	当前持仓首个建仓价格:多
		/// </summary>
		[Description("当前持仓首个建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryPriceLong { get { return this.StrategyDatas[0].EntryPriceLong; } }

		/// <summary>
		/// 	当前持仓首个建仓价格:空
		/// </summary>
		[Description("当前持仓首个建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric EntryPriceShort { get { return this.StrategyDatas[0].EntryPriceShort; } }

		/// <summary>
		/// 	当前持仓最后建仓价格:多
		/// </summary>
		[Description("当前持仓最后建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryPriceLong { get { return this.StrategyDatas[0].LastEntryPriceLong; } }

		/// <summary>
		/// 	当前持仓最后建仓价格:空
		/// </summary>
		[Description("当前持仓最后建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric LastEntryPriceShort { get { return this.StrategyDatas[0].LastEntryPriceShort; } }

		/// <summary>
		/// 	当前持仓平均建仓价格:多
		/// </summary>
		[Description("当前持仓平均建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric AvgEntryPriceLong { get { return this.StrategyDatas[0].AvgEntryPriceLong; } }

		/// <summary>
		/// 	当前持仓平均建仓价格:空
		/// </summary>
		[Description("当前持仓平均建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric AvgEntryPriceShort { get { return this.StrategyDatas[0].AvgEntryPriceShort; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceEntryLong { get { return this.StrategyDatas[0].BarsSinceEntryLong; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceEntryShort { get { return this.StrategyDatas[0].BarsSinceEntryShort; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceLastEntryLong { get { return this.StrategyDatas[0].BarsSinceLastEntryLong; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceLastEntryShort { get { return this.StrategyDatas[0].BarsSinceLastEntryShort; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceExitLong { get { return this.StrategyDatas[0].BarsSinceExitLong; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric BarsSinceExitShort { get { return this.StrategyDatas[0].BarsSinceExitShort; } }

		/// <summary>
		/// 	最近平仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("平仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitDateLong { get { return this.StrategyDatas[0].ExitDateLong; } }

		///<summary>
		///	最近平仓时间:空(yyyyMMdd.HHmmss)
		///</summary>
		[Description("平仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitDateShort { get { return this.StrategyDatas[0].ExitDateShort; } }

		/// <summary>
		/// 	最近平仓价格:多
		/// </summary>
		[Description("平仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitPriceLong { get { return this.StrategyDatas[0].ExitPriceLong; } }

		/// <summary>
		/// 	最近平仓价格:空
		/// </summary>
		[Description("平仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric ExitPriceShort { get { return this.StrategyDatas[0].ExitPriceShort; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:多
		/// </summary>
		[Description("浮动盈亏:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfitLong { get { return this.StrategyDatas[0].PositionProfitLong; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:空
		/// </summary>
		[Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfitShort { get { return this.StrategyDatas[0].PositionProfitShort; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:净
		/// </summary>
		[Description("浮动盈亏:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public Numeric PositionProfit { get { return this.StrategyDatas[0].PositionProfit; } }
		#endregion

		#region ============== 属性 ==================

		/// <summary>
		/// 	策略名称
		/// </summary>
		[Description("名称"), Category("设计"), Browsable(false)]
		public string Name
		{
			get
			{
				if (_name == string.Empty)
				{
					string nameLast = DicProperties.Values.Select(p => GetType().GetField(p.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)).Where(fieldInfo => fieldInfo != null).Aggregate(string.Empty, (current, fieldInfo) => current + (fieldInfo.GetValue(this) + ","));
					nameLast = nameLast.TrimEnd(',');
					this._name = GetType().FullName + (string.IsNullOrEmpty(nameLast) ? "" : ("(" + nameLast + ")"));
				}
				return this._name;
			}
			set { this._name = value; }
		}
		
		/// <summary>
		/// 	合约
		/// </summary>
		[Description("合约"), Category("设计")]
		[Browsable(false)]
		public string InstrumentID { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].InstrumentID; } }

		/// <summary>
		/// 	周期数
		/// </summary>
		[Description("周期数"), Category("设计")]
		[Browsable(false)]
		public int Interval { get { return StrategyDatas.Count == 0 ? 0 : this.StrategyDatas[0].Interval; } }

		/// <summary>
		/// 	周期类型
		/// </summary>
		[Description("周期类型"), Category("设计")]
		[Browsable(false)]
		public EnumIntervalType IntervalType { get { return StrategyDatas.Count == 0 ? EnumIntervalType.Min : this.StrategyDatas[0].IntervalType; } }

		/// <summary>
		/// 	合约信息
		/// </summary>
		public InstrumentInfo InstrumentInfo { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].InstrumentInfo; } }

		/// <summary>
		/// 	当前K线索引(由左向右从0开始)
		/// </summary>
		[Description("当前K线索引"), Category("数据")]
		[Browsable(false)]
		public int CurrentBar { get { return StrategyDatas.Count == 0 ? -1 : this.StrategyDatas[0].CurrentBar; } }

		/// <summary>
		/// 	实际行情(无数据时为UpdateTime == null)
		/// </summary>
		[Description("分笔数据"), Category("数据")]
		[Browsable(false)]
		public Tick Tick { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].Tick; } }

		/// <summary>
		/// 	时间(yyyyMMdd.HHmmss)
		/// </summary>
		[Browsable(false)]
		public DataSeries D { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].D; } }

		/// <summary>
		/// 	最高价
		/// </summary>
		[Browsable(false)]
		public DataSeries H { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].H; } }

		/// <summary>
		/// 	最低价
		/// </summary>
		[Browsable(false)]
		public DataSeries L { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].L; } }

		/// <summary>
		/// 	开盘价
		/// </summary>
		[Browsable(false)]
		public DataSeries O { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].O; } }

		/// <summary>
		/// 	收盘价
		/// </summary>
		[Browsable(false)]
		public DataSeries C { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].C; } }

		/// <summary>
		/// 	成交量
		/// </summary>
		[Browsable(false)]
		public DataSeries V { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].V; } }

		/// <summary>
		/// 	持仓量
		/// </summary>
		[Browsable(false)]
		public DataSeries I { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].I; } }

		/// <summary>
		/// 	均价
		/// </summary>
		[Browsable(false)]
		public DataSeries A { get { return StrategyDatas.Count == 0 ? null : this.StrategyDatas[0].A; } }
		#endregion

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		public override string ToString()
		{
			string nameLast = DicProperties.Values.Aggregate(string.Empty, (current, p) => current + (p.Value + ",")).TrimEnd(',');
			return GetType().FullName + (string.IsNullOrEmpty(nameLast) ? "" : ("(" + nameLast + ")"));
		}

		/// <summary>
		/// 返回策略的参数列表(name:value),以','分隔
		/// </summary>
		/// <returns></returns>
		public string GetParams()
		{
			return "(" + DicProperties.Values.Aggregate(string.Empty, (current, p) => current + (p.Name + ":" + p.Value + ",")).TrimEnd(',') + ")";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pParameters"></param>
		public void FromString(string pParameters)
		{
			if (string.IsNullOrEmpty(pParameters))
				return;
			for (int i = 0; i < pParameters.Split(',').Length; i++)
			{
				var para = pParameters.Split(',')[i];
				var p = DicProperties.ElementAt(i);
				p.Value.Value = Convert.ChangeType(para, p.Value.Value.GetType());

				var fi = this.GetType().GetField(p.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				if (fi != null)
					fi.SetValue(this, p.Value.Value);	//配置字段的值
			}
		}

		/// <summary>
		/// 	StrategyCollection.Add时调用：初始化及测试
		/// </summary>
		/// <param name="pDatas">数据</param>
		public void Init(params Data[] pDatas)
		{
			//处理Bar相关
			this.StrategyDatas.Clear();
			this.Datas.Clear();

			//正式数据
			foreach (Data data in pDatas)
			{
				this.Datas.Add(data);
				StrategyData sd = new StrategyData(data);
				sd.OnRtnOrder += (o, d) =>
				{
					if (_rtnOrder != null)
						_rtnOrder(o, d, this);
				};
				this.StrategyDatas.Add(sd);
			}

			TBInit(); //初始化TB相关数据

			this.Initialize(); //调用客户初始化函数

			#region 初始化自身数据
			//所有指标赋值
			this._indicators.Clear();
			foreach (var idx in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(n => n.FieldType.BaseType == typeof(Indicator)))
			{
				Indicator i = (Indicator)idx.GetValue(this);
				if (i == null)
				{
					throw new Exception("指标未初始化!");
				}
				this._indicators.Add(i);
			}
			//所有用户自定义序列
			this._costomSeries.Clear();
			foreach (var v in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(n => n.FieldType == typeof(DataSeries)))
			{
				//非K线数据:存入
				if (new List<string>(new[] { "H", "L", "O", "C", "V", "I", "A", "D" }).IndexOf((string)v.GetType().GetProperty("Name").GetValue(v, null)) >= 0)
				{
					continue;
				}
				DataSeries s = (DataSeries)v.GetValue(this);
				if (s == null)
				{
					v.SetValue(this, new DataSeries());
				}
				this._costomSeries.Add((DataSeries)v.GetValue(this)); //setvalue后要重新getvalue
			}
			//处理参数
			//dicProperties.Clear();
			//foreach (var v in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
			//{
			//	ParameterAttribute attr = (ParameterAttribute)v.GetCustomAttribute(typeof(ParameterAttribute), false);
			//	if (attr != null)
			//	{
			//		ParameterAttribute pa = (ParameterAttribute)attr;
			//		Property p = new Property(v.Name, v.GetValue(this));
			//		p.Category = pa.Category;
			//		p.Description = pa.Description;
			//		this.Add(p);
			//	}
			//}
			#endregion

			this.Test();

			//this.Initialize(); //再次调用客户初始化函数: 首次调用时,数据源不正确
		}

		/// <summary>
		/// 策略报单
		/// </summary>
		/// <param name="pOrderItem">策略信号</param>
		/// <param name="pData">数据序列</param>
		/// <param name="pStrategy">策略</param>
		public delegate void RtnOrder(OrderItem pOrderItem, Data pData, Strategy pStrategy);

		private RtnOrder _rtnOrder;

		/// <summary>
		/// 
		/// </summary>
		public event RtnOrder OnRtnOrder
		{
			add
			{
				_rtnOrder += value;
			}
			remove
			{
				_rtnOrder -= value;
			}
		}

		//重置指标中的dataseries
		private void ResetInput(Indicator pIdx, bool pClearValue)
		{
			pIdx.IndD = this.D;
			pIdx.IndO = this.O;
			pIdx.IndH = this.H;
			pIdx.IndL = this.L;
			pIdx.IndC = this.C;
			pIdx.IndV = this.V;
			pIdx.IndI = this.I;
			pIdx.IndA = this.A;
			if (pIdx.Input == null)
			{
				pIdx.Input = this.C;
			}
			else
			{
				MemberInfo mi = GetType().GetMember(pIdx.Input.SeriesName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)[0];
				switch (mi.MemberType)
				{
					case MemberTypes.Field:
						pIdx.Input = (DataSeries)((FieldInfo)mi).GetValue(this);
						break;
					case MemberTypes.Property:
						pIdx.Input = (DataSeries)((PropertyInfo)mi).GetValue(this, null);
						break;
				}
			}
			//i.Input.bars.Clear();		//会清掉K线数据
			if (pClearValue)
				pIdx.Value.Clear();

			foreach (var fi in pIdx.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(n => n.FieldType.BaseType == typeof(Indicator)))
			{
				ResetInput((Indicator)fi.GetValue(pIdx), pClearValue);
			}
		}

		private void Test()
		{
			//测试(有数据时）
			if (this.StrategyDatas.Max(n => n.Data.Count) > 0)
			{
				//保存工作区数据
				List<Data> refDatas = this.StrategyDatas.Select(v => v.Data).ToList();

				//构造测试用的Data，替代真实Data
				for (int i = 0; i < this.StrategyDatas.Count; ++i)
				{
					this.StrategyDatas[i].Data = new Data
												  {
													  Instrument = refDatas[i].Instrument,
													  Interval = refDatas[i].Interval,
													  IntervalType = refDatas[i].IntervalType
												  };
				}
				//重置所有指标的输入,指向新的new 的 strategyData
				foreach (var i in this._indicators)
				{
					ResetInput(i, true);
				}

				//以_ws.Datas[0]为标准，填充customSeries,其它Data亦向其看齐
				Data testData = this.StrategyDatas[0].Data;
				Data refData = refDatas[0];
				while (testData.Count < refData.Count) //数据全部测试完成
				{
					testData.Add(refData[testData.Count]);
					//用户序列补全:向_ws.Datas[0]看齐
					foreach (var s in this._costomSeries)
					{
						while (s.Count < testData.Count)
						{
							//s.Add(Numeric.NaN);
							s.New();
						}
					}
					// 多商品同步
					for (int i = 1; i < refDatas.Count; ++i)
					{
						Data iData = refDatas[i];
						Data dataStra = this.StrategyDatas[i].Data;
						if (dataStra.Count == 0)
						{
							dataStra.Add(iData[0]);
						}
						else
						{
							//添加数据,直至下一个Bar的时间在主K时间之后
							while (dataStra.Count < iData.Count && iData[dataStra.Count].D < testData[testData.CurrentBar].D)
							{
								dataStra.Add(iData[dataStra.Count]);
							}
						}
					}
					//更新指标,调用策略
					this.Update();
				}
				//恢复为真实Data
				for (int i = 0; i < this.StrategyDatas.Count; ++i)
				{
					this.StrategyDatas[i].Data = refDatas[i];
				}
			}
			//放在测试过程之后:否则Value与Input序列无法在测试过程中同步
			foreach (var i in this._indicators)
			{
				ResetInput(i, false);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="pParamName"> </param>
		/// <param name="pValue"> </param>
		public void SetParameterValue(string pParamName, object pValue)
		{
			FieldInfo fi = GetType().GetField(pParamName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			if (fi == null)
			{
				return;
			}
			fi.SetValue(this, Convert.ChangeType(pValue, fi.FieldType));
			DicProperties[pParamName].Value = Convert.ChangeType(pValue, fi.FieldType);
		}

		/// <summary>
		/// 	开多仓：买开
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格 </param>
		/// <param name="pRemark">注释</param>
		public void Buy(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			if (PositionShort > 0)
				BuyToCover(PositionShort, pPrice, pRemark);
			this.StrategyDatas[0].Buy((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平多仓：卖平
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格 </param>
		/// <param name="pRemark">注释</param>
		public void Sell(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			this.StrategyDatas[0].Sell((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	开空仓：卖开
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格 </param>
		/// <param name="pRemark">注释</param>
		public void SellShort(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			if (PositionLong > 0)
				Sell(PositionLong, pPrice, pRemark);
			this.StrategyDatas[0].SellShort((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平空仓：买平
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格 </param>
		/// <param name="pRemark">注释</param>
		public void BuyToCover(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			this.StrategyDatas[0].BuyToCover((int)pLots, pPrice, pRemark);
		}

		//调用指标前置false
		//private void Indicator2False(Indicator pIdx)
		//{
		//	pIdx.IsOperated = false;
		//	//数据补全:此处赋值待测试
		//	foreach (var s in pIdx.CustomSeries)
		//	{
		//		while (s.Count < pIdx.Input.Count)
		//		{
		//			s.Add(pIdx.Input[0]);
		//		}
		//	}

		//	foreach (var id in pIdx.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(n => n.FieldType.BaseType == typeof(Indicator)))
		//	{
		//		Indicator2False((Indicator)id.GetValue(pIdx));
		//	}
		//}

		/// <summary>
		/// 数据更新时调用:先更新指标数据,再调用通过继承实现的OnbarUpdate函数
		/// </summary>
		public void Update()
		{
			//foreach (var i in this.indicators)
			//{
			//	Indicator2False(i);
			//}
			//DataSeries变量与主数据对齐
			//if (_costomSeries.Count > 0)// && _costomSeries[0].Count < Datas[0].refWSData.Count)
			foreach (DataSeries t in this._costomSeries)
			{
				var s = t ?? new DataSeries();
				while (s.Count < this.StrategyDatas[0].Data.Count)
					//s.Add(Numeric.NaN);
					s.New();//.Add(0);
			}
			//就采用循环,对调用指标赋值
			foreach (var i in this._indicators) //.Where(i => !i.IsOperated))
			{
				this.indicator2False(i);
			}

			TBKey();	//TB数据更新
			periodUpper(); //跨周期数据更新

			this.OnBarUpdate();
			//foreach (var i in this._indicators)//.Where(i => !i.IsOperated))
			//{
			//	i.isUpdated = false;
			//	i.update();// .OnBarUpdate();
			//}
		}

		private void indicator2False(Indicator pIdx)
		{
			pIdx.IsUpdated = false;
			foreach (var s in pIdx.CustomSeries.Values)
			{
				while (s.Count < pIdx.Input.Count)
				{
					s.Add(pIdx.Input[0]);
				}
			}

			foreach (var id in pIdx.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(n => n.FieldType.BaseType == typeof(Indicator)))
			{
				indicator2False((Indicator)id.GetValue(pIdx));
			}
		}

		//internal bool modified()
		//{
		//	bool rtn = this.Datas.Count == 0;
		//	foreach (Property p in dicProperties.Values)
		//	{
		//		FieldInfo fi = this.GetType().GetField(p.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		//		if (fi == null)
		//		{
		//			throw new Exception("未发现变量名为" + p.Name + "的参数!!!");
		//		}
		//		if (!p.Value.Equals(fi.GetValue(this)))
		//		{
		//			rtn = true;
		//			fi.SetValue(this, p.Value);
		//		}
		//	}
		//	return rtn;
		//}

		/// <summary>
		/// 	初始化
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// 	继承类需完成的策略主体函数
		/// </summary>
		public abstract void OnBarUpdate();
	}


	#region 自定义属性:类中需继承CustomTypeDescriptor,如需直接改写属性,重新定义继承CustomPropertyDescriptor的类并重写Setvalue
	/// <summary>
	/// </summary>
	public class CustomTypeDescriptor : ICustomTypeDescriptor
	{
		internal ConcurrentDictionary<string, Property> DicProperties = new ConcurrentDictionary<string, Property>();

		internal void Add(Property value)
		{
			if (value != null)
			{
				this.DicProperties.TryAdd(value.Name, value);
			}
		}

		#region ICustomTypeDescriptor 成员
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptor[] newProps = new PropertyDescriptor[this.DicProperties.Count];
			//for (int i = 0; i < this.dicProperties.Count; i++)
			int i = 0;
			foreach (var v in this.DicProperties)
			{
				//Property prop = (Property)this[i];
				Property prop = v.Value;
				newProps[i++] = new CustomPropertyDescriptor(ref prop, attributes);
			}
			return new PropertyDescriptorCollection(newProps);
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return TypeDescriptor.GetProperties(this, true);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
		#endregion

		/*
		private void Remove(Property value)
		{
			Property t;
			DicProperties.TryRemove(value.Name, out t);
		}
*/
	}

	/// <summary>
	/// 	自定义属性类
	/// </summary>
	internal class Property
	{
		private string _category = string.Empty;
		private string _displayname = string.Empty;
		private string _name = string.Empty;
		private bool _visible = true;

		public Property(string sName, object sValue)
		{
			this._name = sName;
			this.Value = sValue;
		}

		public Property(string sName, object sValue, bool sReadonly, bool sVisible)
		{
			this._name = sName;
			this.Value = sValue;
			this.ReadOnly = sReadonly;
			this._visible = sVisible;
		}

		public string Name //获得属性名   
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public string Description //属性显示名称   
		{
			get { return this._displayname; }
			set { this._displayname = value; }
		}

		public TypeConverter Converter //类型转换器，我们在制作下拉列表时需要用到   
		{ get; set; }

		public string Category //属性所属类别   
		{
			get { return this._category; }
			set { this._category = value; }
		}

		public object Value //属性值   
		{ get; set; }

		public bool ReadOnly //是否为只读属性   
		{ get; set; }

		public bool Visible //是否可见   
		{
			get { return this._visible; }
			set { this._visible = value; }
		}

		public virtual object Editor //属性编辑器   
		{ get; set; }
	}

	/// <summary>
	/// 	自定义属性描述
	/// </summary>
	internal class CustomPropertyDescriptor : PropertyDescriptor
	{
		protected Property Property;

		public CustomPropertyDescriptor(ref Property myProperty, Attribute[] attrs)
			: base(myProperty.Name, attrs)
		{
			this.Property = myProperty;
		}

		#region PropertyDescriptor 重写方法
		public override Type ComponentType { get { return null; } }

		public override string Description
		{
			get
			{
				//return m_Property.Name;
				return this.Property.Description != "" ? this.Property.Description : this.Property.Name;
			}
		}

		public override string Category { get { return this.Property.Category; } }

		public override string DisplayName
		{
			get
			{
				return this.Property.Name;
				//return m_Property.DisplayName != "" ? m_Property.DisplayName : m_Property.Name;
			}
		}

		public override bool IsReadOnly { get { return this.Property.ReadOnly; } }

		public override TypeConverter Converter { get { return this.Property.Converter; } }

		public override Type PropertyType { get { return this.Property.Value.GetType(); } }

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return this.Property.Value;
		}

		public override void ResetValue(object component)
		{
			//Have to implement   
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void SetValue(object component, object value)
		{
			if (component.GetType().BaseType == typeof(Strategy)) // .FullName == "GFCTP.Strategy")
			{
				FieldInfo fieldInfo = component.GetType().GetField(this.Property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(component, value);
				}
			}
			this.Property.Value = value;
		}

		public override object GetEditor(Type editorBaseType)
		{
			return this.Property.Editor ?? base.GetEditor(editorBaseType);
		}
		#endregion
	}
	#endregion

}
