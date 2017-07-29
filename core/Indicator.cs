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
		/// 新K线
		/// </summary>
		public bool IsFirstTickOfBar = true;

		/// <summary>
		/// 
		/// </summary>
		public Indicator() { this.Init(); }

		/// <summary>
		/// 输入序列
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
					ipt.OnChanged += input_OnChanged; //每个输入序列变化都会被执行
				}
				//初始时调用
				//20170729 此时原始数据序列为空不可以调用 this.OnBarUpdate();
			}
		}

		private void input_OnChanged(int pType, object pNew, object pOld)
		{
			if (pType == 0 || pType == 1)
			{
				IsFirstTickOfBar = pType == 1; //1-添加; 0-更新; -1移除
				this.OnBarUpdate();
			}
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
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public double this[int index] { get { return Value[index]; } }


		/// <summary>
		/// 当前bar索引(0开始)
		/// </summary>
		protected int CurrentBar { get { return Math.Max(Input.Count - 1, 0); } }

		/// <summary>
		/// 输入序列数据点数量
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
		/// K线更新
		/// </summary>
		protected virtual void OnBarUpdate() { }
	}
}
