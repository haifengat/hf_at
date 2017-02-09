
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
	public abstract partial class Strategy : CustomTypeDescriptor
	{
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
					fi.SetValue(this, p.Value.Value);   //配置字段的值
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

			bool _real = false;
			//正式数据
			foreach (Data data in pDatas)
			{
				this.Datas.Add(data);
				StrategyData sd = new StrategyData(data);
				sd.OnRtnOrder += (o, d) =>
				{
					if (_rtnOrder != null && _real)
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
			_real = true;   //控制实盘发单

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
		/// <param name="pPrice"> 价格 (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void Buy(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			this.StrategyDatas[0].Buy((int)pLots, pPrice, pRemark);
		}
		
		/// <summary>
		/// 	平多仓：卖平
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void Sell(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			this.StrategyDatas[0].Sell((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	开空仓：卖开
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void SellShort(Numeric pLots, Numeric pPrice, string pRemark = "")
		{
			this.StrategyDatas[0].SellShort((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平空仓：买平
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
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

			TBKey();    //TB数据更新
			periodUpper(); //跨周期数据更新
			foreach (var i in this._indicators)//.Where(i => !i.IsOperated))
			{
				//base data series run the indicator before strategy
				if(new[] { i.IndA,i.IndC,i.IndD,i.IndH,i.IndI,i.IndL,i.IndO,i.IndV}.ToList().IndexOf(i.Input) >= 0)
					//i.isUpdated = false;
					i.update();// .OnBarUpdate();
			}

			this.OnBarUpdate();
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


}
