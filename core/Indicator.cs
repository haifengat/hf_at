using System;
using System.Collections.Generic;

namespace HaiFeng
{
	/// <summary>
	/// </summary>
	public partial class Indicator
	{
		DataSeries _input;
		DataSeries[] _values = new DataSeries[8];
		int[] _periods = new int[255];

		/// <summary>
		/// 新K线
		/// </summary>
		public bool IsFirstTickOfBar = true;

		/// <summary>
		/// 
		/// </summary>
		public Indicator() { }

		/// <summary>
		/// 输入序列
		/// </summary>
		public DataSeries Input
		{
			get
			{
				return _input;
			}
			set
			{
				_input = value;
				for (int i = 0; i < _values.Length; i++)
					_values[i] = new DataSeries(value);     //所有输出序列与values[0]同步,如需更改可在指标中用dataseries(xxx)
						
				//输入序列有变化:执行OnBarUpdate
				_input.OnChanged += input_OnChanged; //每个输入序列变化都会被执行

				this.Init(); //在生成后再调用，否则会导致Input为null

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
		public int CurrentBar { get { return Math.Max(Input.Count - 1, 0); } }

		/// <summary>
		/// 输入序列数据点数量
		/// </summary>
		public int Count { get { return Input.Count; } }

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="Indicator"></typeparam>
		/// <param name="idx"></param>
		/// <param name="catchIdx"></param>
		/// <returns></returns>
		public Indicator CacheIndicator<Indicator>(Indicator idx, ref Indicator[] catchIdx)
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
		/// <param name="input"></param>
		/// <returns></returns>
		public bool EqualsInput(DataSeries input)
		{
			return Input.Equals(input);
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
