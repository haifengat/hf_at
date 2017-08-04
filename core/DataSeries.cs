#region
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
#endregion


namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class DataSeries : Collection<double>
	{
		static ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries> _dicOperateAdd = new ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries>();
		static ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries> _dicOperateSub = new ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries>();
		static ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries> _dicOperateMul = new ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries>();
		static ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries> _dicOperateDiv = new ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries>();
		static ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries> _dicOperateMod = new ConcurrentDictionary<Tuple<DataSeries, DataSeries>, DataSeries>();

		private DataSeries _base = null;
		private CollectionChange _onChanging;
		private CollectionChange _onChanged;

		/// <summary>
		/// 构建函数(参数勿填)
		/// </summary>
		/// <param name="baseSeries">同步序列</param>
		public DataSeries(DataSeries baseSeries = null)
		{
			if (baseSeries == null) return;

			_base = baseSeries;
			//初始时同步
			foreach (var v in _base)
				this.Add(v);
			_base.OnChanging += _base_OnChanged;
		}

		/// <summary>
		/// 无效时返回Nan
		/// </summary>
		/// <param name="index">从0开始计数</param>
		/// <returns>无效时返回Nan</returns>
		public new double this[int index]
		{
			get
			{
				double rtn = double.NaN;
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

		#region 函数
		/// <summary>
		/// 取指定范围的最大值
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public double Highest(int begin, int length) { return this.Where((n, idx) => idx <= Count - 1 - begin && idx > Count - 1 - begin - length).Max(); }
		/// <summary>
		/// 取指定范围的最小值
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public double Lowest(int begin, int length) { return this.Where((n, idx) => idx <= Count - 1 - begin && idx > Count - 1 - begin - length).Min(); }
		/// <summary>
		/// 取指定范围的和
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public double Sum(int begin, int length) { return this.Where((n, idx) => idx <= Count - 1 - begin && idx > Count - 1 - begin - length).Sum(); }
		/// <summary>
		/// 取指定范围的均值
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public double Average(int begin, int length) { return this.Where((n, idx) => idx <= Count - 1 - begin && idx > Count - 1 - begin - length).Average(); }
		#endregion

		private void _base_OnChanged(int pType, object pNew, object pOld)
		{
			if (pType == 1) //增加
			{
				this.Add((double)pNew);
			}
		}

		/// <summary>
		/// 策略变化:加1;减-1;更新0
		/// </summary>
		public event CollectionChange OnChanging
		{
			add
			{
				_onChanging += value;
			}
			remove
			{
				_onChanging -= value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public event CollectionChange OnChanged
		{
			add
			{
				_onChanged += value;
			}
			remove
			{
				_onChanged -= value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, double item)
		{
			base.InsertItem(index, item);
			_onChanging?.Invoke(1, item, null);
			_onChanged?.Invoke(1, item, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, double item)
		{
			base.SetItem(index, item);
			_onChanging?.Invoke(0, item, base[index]);
			_onChanged?.Invoke(0, item, base[index]);
		}


		/// <summary>
		/// 两个序列中各值相加(两序列数目必须相等)
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static DataSeries operator +(DataSeries s1, DataSeries s2)
		{
			if (s1.Count != s2.Count) return null;
			DataSeries ds = null;
			foreach (var v in _dicOperateAdd)
			{
				if (v.Key.Item1 == s1 && v.Key.Item2 == s2)
				{
					ds = v.Value;
					break;
				}
			}
			if (ds == null)
			{
				ds = new DataSeries();
				_dicOperateAdd[new Tuple<DataSeries, DataSeries>(s1, s2)] = ds;
			}
			//更新
			if (ds.Count == s1.Count)
				ds[0] = s1[0] + s2[0];
			else//添加
				for (int i = ds.Count; i < s1.Count; i++)
					ds.Add(s1.Items[i] + s2.Items[i]);
			return ds;
		}

		/// <summary>
		/// 两个序列中各值相加(两序列数目必须相等)
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static DataSeries operator -(DataSeries s1, DataSeries s2)
		{
			if (s1.Count != s2.Count) return null;
			DataSeries ds = null;
			foreach (var v in _dicOperateAdd)
			{
				if (v.Key.Item1 == s1 && v.Key.Item2 == s2)
				{
					ds = v.Value;
					break;
				}
			}
			if (ds == null)
			{
				ds = new DataSeries();
				_dicOperateSub[new Tuple<DataSeries, DataSeries>(s1, s2)] = ds;
			}
			//更新
			if (ds.Count == s1.Count)
				ds[0] = s1[0] - s2[0];
			else//添加
				for (int i = ds.Count; i < s1.Count; i++)
					ds.Add(s1.Items[i] - s2.Items[i]);
			return ds;
		}

		/// <summary>
		/// 两个序列中各值相加(两序列数目必须相等)
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static DataSeries operator *(DataSeries s1, DataSeries s2)
		{
			if (s1.Count != s2.Count) return null;
			DataSeries ds = null;
			foreach (var v in _dicOperateAdd)
			{
				if (v.Key.Item1 == s1 && v.Key.Item2 == s2)
				{
					ds = v.Value;
					break;
				}
			}
			if (ds == null)
			{
				ds = new DataSeries();
				_dicOperateMul[new Tuple<DataSeries, DataSeries>(s1, s2)] = ds;
			}
			//更新
			if (ds.Count == s1.Count)
				ds[0] = s1[0] * s2[0];
			else//添加
				for (int i = ds.Count; i < s1.Count; i++)
					ds.Add(s1.Items[i] * s2.Items[i]);
			return ds;
		}
		/// <summary>
		/// 两个序列中各值相加(两序列数目必须相等)
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static DataSeries operator /(DataSeries s1, DataSeries s2)
		{
			if (s1.Count != s2.Count) return null;
			DataSeries ds = null;
			foreach (var v in _dicOperateAdd)
			{
				if (v.Key.Item1 == s1 && v.Key.Item2 == s2)
				{
					ds = v.Value;
					break;
				}
			}
			if (ds == null)
			{
				ds = new DataSeries();
				_dicOperateDiv[new Tuple<DataSeries, DataSeries>(s1, s2)] = ds;
			}
			//更新
			if (ds.Count == s1.Count)
				ds[0] = s1[0] / s2[0];
			else//添加
				for (int i = ds.Count; i < s1.Count; i++)
					ds.Add(s1.Items[i] / s2.Items[i]);
			return ds;
		}
		/// <summary>
		/// 两个序列中各值相加(两序列数目必须相等)
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static DataSeries operator %(DataSeries s1, DataSeries s2)
		{
			if (s1.Count != s2.Count) return null;
			DataSeries ds = null;
			foreach (var v in _dicOperateAdd)
			{
				if (v.Key.Item1 == s1 && v.Key.Item2 == s2)
				{
					ds = v.Value;
					break;
				}
			}
			if (ds == null)
			{
				ds = new DataSeries();
				_dicOperateMod[new Tuple<DataSeries, DataSeries>(s1, s2)] = ds;
			}
			//更新
			if (ds.Count == s1.Count)
				ds[0] = s1[0] % s2[0];
			else//添加
				for (int i = ds.Count; i < s1.Count; i++)
					ds.Add(s1.Items[i] % s2.Items[i]);
			return ds;
		}
	}

}
