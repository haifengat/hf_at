
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HaiFeng
{
	/// <summary>
	/// 	策略
	/// </summary>
	[DefaultProperty("Name")]
	public abstract partial class Strategy : CustomTypeDescriptor
	{
		private Indicator indicator = new Indicator();

		private string _name = string.Empty;

		/// <summary>
		/// </summary>
		protected Strategy()
		{
			Datas = new List<Data>();

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
		/// 策略所用的数据序列
		/// </summary>
		[Browsable(false)]
		public List<Data> Datas { get; private set; }

		/// <summary>
		/// 	报单操作
		/// </summary>
		[Description("报单操作列表"), Category("交易")]
		[Browsable(false)]
		public List<OrderItem> Operations { get { return Datas.Count == 0 ? null : this.Datas[0].Operations; } }
		

		#region 策略状态:对data[0]的引用

		/// <summary>
		/// 	当前持仓手数:多
		/// </summary>
		[Description("当前持仓手数:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionLong { get { return this.Datas[0].PositionLong; } }

		/// <summary>
		/// 	当前持仓手数:空
		/// </summary>
		[Description("当前持仓手数:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionShort { get { return this.Datas[0].PositionShort; } }

		/// <summary>
		/// 	当前持仓手数:净
		/// </summary>
		[Description("当前持仓手数:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int PositionNet { get { return this.Datas[0].PositionNet; } }

		/// <summary>
		/// 	当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryDateLong { get { return this.Datas[0].EntryDateLong; } }

		/// <summary>
		/// 	当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓首个建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryDateShort { get { return this.Datas[0].EntryDateShort; } }

		/// <summary>
		/// 	当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryDateLong { get { return this.Datas[0].LastEntryDateLong; } }

		/// <summary>
		/// 	当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("当前持仓最后建仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryDateShort { get { return this.Datas[0].LastEntryDateShort; } }

		/// <summary>
		/// 	当前持仓首个建仓价格:多
		/// </summary>
		[Description("当前持仓首个建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryPriceLong { get { return this.Datas[0].EntryPriceLong; } }

		/// <summary>
		/// 	当前持仓首个建仓价格:空
		/// </summary>
		[Description("当前持仓首个建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double EntryPriceShort { get { return this.Datas[0].EntryPriceShort; } }

		/// <summary>
		/// 	当前持仓最后建仓价格:多
		/// </summary>
		[Description("当前持仓最后建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryPriceLong { get { return this.Datas[0].LastEntryPriceLong; } }

		/// <summary>
		/// 	当前持仓最后建仓价格:空
		/// </summary>
		[Description("当前持仓最后建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double LastEntryPriceShort { get { return this.Datas[0].LastEntryPriceShort; } }

		/// <summary>
		/// 	当前持仓平均建仓价格:多
		/// </summary>
		[Description("当前持仓平均建仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double AvgEntryPriceLong { get { return this.Datas[0].AvgEntryPriceLong; } }

		/// <summary>
		/// 	当前持仓平均建仓价格:空
		/// </summary>
		[Description("当前持仓平均建仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double AvgEntryPriceShort { get { return this.Datas[0].AvgEntryPriceShort; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceEntryLong { get { return this.Datas[0].BarsSinceEntryLong; } }

		/// <summary>
		/// 	当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓首个建仓到当前位置的Bar数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceEntryShort { get { return this.Datas[0].BarsSinceEntryShort; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceLastEntryLong { get { return this.Datas[0].BarsSinceLastEntryLong; } }

		/// <summary>
		/// 	当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("当前持仓的最后建仓到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceLastEntryShort { get { return this.Datas[0].BarsSinceLastEntryShort; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:多(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:多(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceExitLong { get { return this.Datas[0].BarsSinceExitLong; } }

		/// <summary>
		/// 	最近平仓位置到当前位置的Bar计数:空(从0开始计数)
		/// </summary>
		[Description("最近平仓位置到当前位置的Bar计数:空(从0开始计数)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public int BarsSinceExitShort { get { return this.Datas[0].BarsSinceExitShort; } }

		/// <summary>
		/// 	最近平仓时间:多(yyyyMMdd.HHmmss)
		/// </summary>
		[Description("平仓时间:多(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitDateLong { get { return this.Datas[0].ExitDateLong; } }

		///<summary>
		///	最近平仓时间:空(yyyyMMdd.HHmmss)
		///</summary>
		[Description("平仓时间:空(yyyyMMdd.HHmmss)"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitDateShort { get { return this.Datas[0].ExitDateShort; } }

		/// <summary>
		/// 	最近平仓价格:多
		/// </summary>
		[Description("平仓价格:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitPriceLong { get { return this.Datas[0].ExitPriceLong; } }

		/// <summary>
		/// 	最近平仓价格:空
		/// </summary>
		[Description("平仓价格:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double ExitPriceShort { get { return this.Datas[0].ExitPriceShort; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:多
		/// </summary>
		[Description("浮动盈亏:多"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double PositionProfitLong { get { return this.Datas[0].PositionProfitLong; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:空
		/// </summary>
		[Description("浮动盈亏:空"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double PositionProfitShort { get { return this.Datas[0].PositionProfitShort; } }

		/// <summary>
		/// 	当前持仓浮动盈亏:净
		/// </summary>
		[Description("浮动盈亏:净"), Category("状态"), ReadOnly(true), Browsable(false)]
		public double PositionProfit { get { return this.Datas[0].PositionProfit; } }
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
		public string InstrumentID { get { return Datas.Count == 0 ? null : this.Datas[0].Instrument; } }

		/// <summary>
		/// 	周期数
		/// </summary>
		[Description("周期数"), Category("设计")]
		[Browsable(false)]
		public int Interval { get { return Datas.Count == 0 ? 0 : this.Datas[0].Interval; } }

		/// <summary>
		/// 	周期类型
		/// </summary>
		[Description("周期类型"), Category("设计")]
		[Browsable(false)]
		public EnumIntervalType IntervalType { get { return Datas.Count == 0 ? EnumIntervalType.Min : this.Datas[0].IntervalType; } }

		/// <summary>
		/// 	合约信息
		/// </summary>
		public InstrumentInfo InstrumentInfo { get { return Datas.Count == 0 ? null : this.Datas[0].InstrumentInfo; } }

		/// <summary>
		/// 	当前K线索引(由左向右从0开始)
		/// </summary>
		[Description("当前K线索引"), Category("数据")]
		[Browsable(false)]
		public int CurrentBar { get { return Datas.Count == 0 ? -1 : this.Datas[0].CurrentBar; } }

		/// <summary>
		/// 	实际行情(无数据时为UpdateTime == null)
		/// </summary>
		[Description("分笔数据"), Category("数据")]
		[Browsable(false)]
		public Tick Tick { get { return Datas.Count == 0 ? null : this.Datas[0].Tick; } }

		/// <summary>
		/// 	时间(yyyyMMdd.HHmmss)
		/// </summary>
		[Browsable(false)]
		public DataSeries D { get { return Datas.Count == 0 ? null : this.Datas[0].D; } }

		/// <summary>
		/// 	最高价
		/// </summary>
		[Browsable(false)]
		public DataSeries H { get { return Datas.Count == 0 ? null : this.Datas[0].H; } }

		/// <summary>
		/// 	最低价
		/// </summary>
		[Browsable(false)]
		public DataSeries L { get { return Datas.Count == 0 ? null : this.Datas[0].L; } }

		/// <summary>
		/// 	开盘价
		/// </summary>
		[Browsable(false)]
		public DataSeries O { get { return Datas.Count == 0 ? null : this.Datas[0].O; } }

		/// <summary>
		/// 	收盘价
		/// </summary>
		[Browsable(false)]
		public DataSeries C { get { return Datas.Count == 0 ? null : this.Datas[0].C; } }

		/// <summary>
		/// 	成交量
		/// </summary>
		[Browsable(false)]
		public DataSeries V { get { return Datas.Count == 0 ? null : this.Datas[0].V; } }

		/// <summary>
		/// 	持仓量
		/// </summary>
		[Browsable(false)]
		public DataSeries I { get { return Datas.Count == 0 ? null : this.Datas[0].I; } }

		/// <summary>
		/// 	均价
		/// </summary>
		[Browsable(false)]
		public DataSeries A { get { return Datas.Count == 0 ? null : this.Datas[0].A; } }
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
			this.Datas.Clear();
			this.Datas.Clear();

			//此时数据为空
			foreach (Data data in pDatas)
				this.Datas.Add(data);

			this.Initialize(); //调用客户初始化函数


			//发单由plat层控制
			foreach (Data data in this.Datas)
				data.OnRtnOrder += (o, d) =>
				{
					_rtnOrder?.Invoke(o, d, this);
				};

			//#region 初始化自身数据
			////所有指标赋值
			//this._indicators.Clear();
			//foreach (var idx in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(n => n.FieldType.BaseType == typeof(Indicator)))
			//{
			//	Indicator i = (Indicator)idx.GetValue(this);
			//	if (i == null)
			//	{
			//		throw new Exception("指标未初始化!");
			//	}
			//	this._indicators.Add(i);
			//}
			//#endregion


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
		
		/// <summary>
		/// 支持多个合约数据序列
		/// </summary>
		/// <param name="barss"></param>
		public void LoadHistory(params Tuple<Data, List<Bar>>[] barss)
		{
			if (barss.Sum(n => n.Item2.Count) == 0) return;

			var barseries = new List<Bar>();
			foreach (var v in barss)
				barseries.AddRange(v.Item2);
			//按时间排序;对各Data序列赋值
			barseries = barseries.OrderBy(n => n.D).ToList();
			foreach (var bar in barseries)
			{
				var f = barss.First(n => n.Item2.IndexOf(bar) >= 0);
				f.Item1.Add(bar);
				Update();
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
		public void Buy(double pLots, double pPrice, string pRemark = "")
		{
			this.Datas[0].Buy((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平多仓：卖平
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void Sell(double pLots, double pPrice, string pRemark = "")
		{
			this.Datas[0].Sell((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	开空仓：卖开
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void SellShort(double pLots, double pPrice, string pRemark = "")
		{
			this.Datas[0].SellShort((int)pLots, pPrice, pRemark);
		}

		/// <summary>
		/// 	平空仓：买平
		/// </summary>
		/// <param name="pLots"> 手数 </param>
		/// <param name="pPrice"> 价格  (非PriceTick整数倍会报错,请先行处理.)</param>
		/// <param name="pRemark">注释</param>
		public void BuyToCover(double pLots, double pPrice, string pRemark = "")
		{
			this.Datas[0].BuyToCover((int)pLots, pPrice, pRemark);
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
		//	//就采用循环,对调用指标赋值
		//	foreach (var i in this._indicators) //.Where(i => !i.IsOperated))
		//	{
		//		this.indicator2False(i);
		//	}

		//	foreach (var i in this._indicators)//.Where(i => !i.IsOperated))
		//	{
		//		//base data series run the indicator before strategy
		//		if (new[] { i.IndA, i.IndC, i.IndD, i.IndH, i.IndI, i.IndL, i.IndO, i.IndV }.ToList().IndexOf(i.Input) >= 0)
		//			//i.isUpdated = false;
		//			i.update();// .OnBarUpdate();
		//	}

			this.OnBarUpdate();
		}

		private void indicator2False(Indicator pIdx)
		{
			pIdx.IsUpdated = false;
			//foreach (var s in pIdx.CustomSeries.Values)
			//{
			//	while (s.Count < pIdx.Input.Count)
			//	{
			//		s.Add(pIdx.Input[0]);
			//	}
			//}

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
