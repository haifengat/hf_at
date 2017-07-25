using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{

	/// <summary>
	/// 自定义集合变化事件
	/// </summary>
	/// <param name="pType">策略变化:加1;减-1;更新0</param>
	/// <param name="pNew"></param>
	/// <param name="pOld"></param>
	public delegate void CollectionChange(int pType, object pNew, object pOld);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="pOrderItem"></param>
	/// <param name="pData"></param>
	public delegate void RtnOrder(OrderItem pOrderItem, Data pData);
}
