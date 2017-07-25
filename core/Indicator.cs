using System;
using System.Collections.Generic;

namespace HaiFeng
{
	/// <summary>
	/// </summary>
	public partial class Indicator
	{
		DataSeries[] _inputs = new DataSeries[255];
		DataSeries[] _values = new DataSeries[255];
		int[] _periods = new int[255];

		/// <summary>
		/// 
		/// </summary>
		public Indicator() { this.Init(); }

		/// <summary>
		/// 	输入序列
		/// </summary>
		public DataSeries Input
		{
			get
			{
				return _inputs[0];
			}
			set
			{
				_inputs[0] = value;
				for (int i = 0; i < _values.Length; i++)
					_values[i] = new DataSeries(value);     //所有输出序列与values[0]同步,如需更改可在指标中用dataseries(xxx)
				foreach (var ipt in _inputs)//输入序列有变化:执行OnBarUpdate
				{
					if (ipt == null) break;
					ipt.OnChanged += input_OnChanged;
				}
				//初始时调用
				this.OnBarUpdate();
			}
		}

		private void input_OnChanged(int pType, object pNew, object pOld)
		{
			if (pType == 0 || pType == 1)
				this.OnBarUpdate();
		}

		/// <summary>
		/// 输入多个序列
		/// </summary>
		public DataSeries[] Inputs
		{
			get
			{
				return _inputs;
			}
			set
			{
				_inputs = value;
				Input = _inputs[0];//调用Input.set
			}
		}

		/// <summary>
		/// 输出序列
		/// </summary>
		public DataSeries Value
		{
			get { return _values[0]; }
			set { _values[0] = value; }
		}

		/// <summary>
		/// 输出序列
		/// </summary>
		public DataSeries[] Values
		{
			get { return _values; }
			set { _values = value; }
		}

		/// <summary>
		/// 每个tick只处理一次; 策略中被调用
		/// </summary>
		internal bool IsUpdated = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public double this[int index] { get { return Value[index]; } }


		/// <summary>
		/// 	当前bar索引(0开始)
		/// </summary>
		protected int CurrentBar { get { return Math.Max(Input.Count - 1, 0); } }

		/// <summary>
		/// 	输入序列数据点数量
		/// </summary>
		protected int Count { get { return Input.Count; } }

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="Indicator"></typeparam>
		/// <param name="idx"></param>
		/// <param name="catchIdx"></param>
		/// <returns></returns>
		protected Indicator CacheIndicator<Indicator>(Indicator idx, ref Indicator[] catchIdx)
		{
			if (catchIdx == null)
				catchIdx = new Indicator[] { idx };
			else
			{
				var list = new List<Indicator>(catchIdx);
				list.Add(idx);
				catchIdx = list.ToArray();
			}
			return idx;
		}

		/// <summary>
		/// 比较输入序列是否相同
		/// </summary>
		/// <param name="inputs"></param>
		/// <returns></returns>
		protected bool EqualsInput(params DataSeries[] inputs)
		{
			for (int i = 0; i < inputs.Length; i++)
				if (inputs[i].Equals(Inputs[i]))
					return false;
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Init() { }

		/// <summary>
		/// 	K线更新
		/// </summary>
		protected virtual void OnBarUpdate() { }

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
