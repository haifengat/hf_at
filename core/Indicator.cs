using System.Collections.Concurrent;

#region

using System.Collections.Generic;
using System.Reflection;
#endregion

using Numeric = System.Decimal;
namespace HaiFeng
{
	/// <summary>
	/// </summary>
	public abstract class Indicator
	{
		/// <summary>
		/// 自定义指标中声明的 DataSeries
		/// </summary>
		public ConcurrentDictionary<string, DataSeries> CustomSeries = new ConcurrentDictionary<string, DataSeries>();

		//K线数据:由Strategy赋值
		internal DataSeries IndD,
						   IndO,
						   IndH,
						   IndL,
						   IndC,
						   IndV,
						   IndI,
						   IndA;

		/// <summary>
		/// 	输入序列
		/// </summary>
		public DataSeries Input = null;


		/// <summary>
		/// 	参数数组
		/// </summary>
		public Numeric[] Periods = null;

		/// <summary>
		/// </summary>
		public DataSeries Value = new DataSeries();

		/// <summary>
		/// 每个tick只处理一次; 策略中被调用
		/// </summary>
		internal bool IsUpdated = false;

		/// <summary>
		/// </summary>
		/// <param name="input"> </param>
		/// <param name="periods"> </param>
		protected Indicator(DataSeries input, params Numeric[] periods)
		{
			this.Input = input;
			this.Periods = periods;
			foreach (var fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
			{
				if (fieldInfo.FieldType != typeof(DataSeries) || new List<string>(new[] { "d", "o", "h", "l", "c", "v", "i", "a", "input", "Input" }).IndexOf(fieldInfo.Name) >= 0)
				{
					continue;
				}
				DataSeries s = (DataSeries)fieldInfo.GetValue(this);
				if (s == null)
				{
					fieldInfo.SetValue(this, new DataSeries());
					s = (DataSeries)fieldInfo.GetValue(this);
				}
				this.CustomSeries[fieldInfo.Name] = s;
				s.Idc = this;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Numeric this[int index] { get { return Value[index]; } set { Value[index] = value; } }

		/// <summary>
		/// 	取得参数组中的首个参数
		/// </summary>
		protected int Period { get { return (int)this.Periods[0]; } }

		/// <summary>
		/// </summary>
		protected DataSeries Date { get { return this.IndD; } }

		/// <summary>
		/// </summary>
		protected DataSeries High { get { return this.IndH; } }

		/// <summary>
		/// </summary>
		protected DataSeries Low { get { return this.IndL; } }

		/// <summary>
		/// </summary>
		protected DataSeries Open { get { return this.IndO; } }

		/// <summary>
		/// </summary>
		protected DataSeries Close { get { return this.IndC; } }

		/// <summary>
		/// </summary>
		protected DataSeries Volume { get { return this.IndV; } }

		/// <summary>
		/// </summary>
		protected DataSeries OpenInterest { get { return this.IndI; } }

		/// <summary>
		/// </summary>
		protected DataSeries Average { get { return this.IndA; } }

		/// <summary>
		/// 	当前bar索引(0开始)
		/// </summary>
		protected int CurrentBar { get { return this.IndD.Count - 1; } }

		/// <summary>
		/// 	输入序列数据点数量
		/// </summary>
		protected int Count { get { return this.IndD.Count; } }

		/// <summary>
		/// 	K线更新
		/// </summary>
		public abstract void OnBarUpdate();

		/// <summary>
		/// 处理数据同步及执行(根据是否同步过,判断是否再次执行)
		/// </summary>
		internal void update()
		{
			//是否同步过?已经执行:未执行
			//2013.10.6 移至Strategy.indicator2False
			//if (this.Input.Count > Value.Count)
			//{
			//	foreach (var s in this.CustomSeries)
			//	{
			//		while (s.Count < Input.Count)
			//		{
			//			s.Add(Input[0]);
			//		}
			//	}
			//	//isUpdated = true;
			//	//OnBarUpdate();
			//}
			if (!this.IsUpdated)
			{
				this.IsUpdated = true;
				OnBarUpdate();
			}
		}
	}
}
