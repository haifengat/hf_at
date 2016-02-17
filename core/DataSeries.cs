#region
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

#endregion

using Numeric = System.Decimal;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class DataSeries : Collection<Numeric>
	{
		internal string SeriesName
		{
			get { return _name; }
		}

		/// <summary>
		/// 	指标中的DataSeries在被取值时this[n]运算指标
		/// </summary>
		internal Indicator Idc = null;

		private readonly string _name;

		/// <summary>
		/// 构建函数(参数勿填)
		/// </summary>
		/// <param name="pSeriesName"></param>
		public DataSeries([CallerMemberName] string pSeriesName = null)
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
		public new Numeric this [int index]
		{
			get
			{
				Numeric rtn = 0; //Numeric.ZeroNaN;
				if (this.Idc != null)// && !this.idc.IsOperated)
				{
					//在策略调用时处理:此处会导致循环调用
					//this.idc.isUpdated = false;
					this.Idc.update();// .OnBarUpdate();
				}
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

		public void New()
		{
			if (Count == 0)
				base.Add(0);
			else
				base.Add(this[0]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public new void Add(Numeric p)
		{
			base.Add(p);
		}
	}


	/// <summary>
	/// 自定义集合变化事件
	/// </summary>
	/// <param name="pType"></param>
	/// <param name="pNew"></param>
	/// <param name="pOld"></param>
	public delegate void CollectionChange(int pType, object pNew, object pOld);

	/// <summary>
	/// 	数据集合
	/// </summary>
	public class DataCollection : Collection<Data>
	{
		private CollectionChange _onChange;
		/// <summary>
		/// 数据变化:加1;减-1;更新0
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Data item)
		{
			base.InsertItem(index, item);
			if (_onChange != null)
			{
				_onChange(1, item, null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, Data item)
		{
			Data old = this[index];
			base.SetItem(index, item);
			if (_onChange != null)
			{
				_onChange(0, item, old);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			Data old = this[index];
			base.RemoveItem(index);
			if (_onChange != null)
			{
				_onChange(-1, old, old);
			}
		}
	}

	/// <summary>
	/// </summary>
	public class StrategyCollection : Collection<Strategy>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Strategy item)
		{
			base.InsertItem(index, item);
			if (_onChange != null)
			{
				_onChange(1, item, null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, Strategy item)
		{
			Strategy old = this[index];
			base.SetItem(index, item);
			if (_onChange != null)
			{
				_onChange(0, item, old);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			Strategy old = this[index];
			base.RemoveItem(index);
			if (_onChange != null)
			{
				_onChange(-1, old, old);
			}
		}
	}
}
