using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
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
