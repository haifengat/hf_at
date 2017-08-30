using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public static partial class A_
	{
		public static TradeExt Trade { get; internal set; }

		/// <summary>
		/// 返回当前公式应用绑定账户的交易帐户ID
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static string A_AccountID(this Strategy stra)
		{
			return Trade?.Investor;
		}

		/// <summary>
		/// 返回当前公式应用绑定账户对应的交易商ID
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static string A_BrokerID(this Strategy stra)
		{
			return Trade?.Broker;
		}

		/// <summary>
		/// 返回当前公式应用绑定账户下当前商品的买入持仓
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static PositionField A_BuyPosition(this Strategy stra)
		{
			return Trade != null & Trade.DicPositionField.TryGetValue($"{stra.InstrumentID}_Buy", out PositionField posi) ? posi : null;
		}

		/// <summary>
		/// 返回当前公式应用绑定账户下当前商品的卖出持仓
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static PositionField A_SellPosition(this Strategy stra)
		{
			return Trade != null & Trade.DicPositionField.TryGetValue($"{stra.InstrumentID}_Sell", out PositionField posi) ? posi : null;
		}

		/// <summary>
		/// 返回当前公式应用绑定帐户的权益
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static TradingAccount A_TradingAccount(this Strategy stra)
		{
			return Trade?.TradingAccount;
		}


		/// <summary>
		/// 返回当前公式应用绑定帐户下当前策略的委托单列表
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static OrderField[] A_GetOrders(this Strategy stra)
		{
			return Trade?.DicOrderField.Values.Where(n => int.TryParse(stra.Name, out int id) && n.Custom == id * 100).ToArray();
		}

		/// <summary>
		/// 返回当前公式应用的帐户下当前策略的最后一个当日委托单
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static OrderField A_GetLastOrderIndex(this Strategy stra)
		{
			return Trade?.DicOrderField.Values.Where(n => int.TryParse(stra.Name, out int id) && n.Custom == id * 100).LastOrDefault();
		}

		/// <summary>
		/// 返回当前公式应用绑定帐户下当前策略的未成交委托单
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static OrderField[] A_GetNotFillOrderCount(this Strategy stra)
		{
			return Trade?.DicOrderField.Values.Where(n => int.TryParse(stra.Name, out int id) && n.Custom == id * 100 && (n.Status == OrderStatus.Normal || n.Status == OrderStatus.Partial)).ToArray();
		}

		/// <summary>
		/// 返回当前公式应用绑定帐户下当前策略的某个委托单
		/// 买 0;卖 1; 未找到:-1
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static OrderField A_GetOrder(this Strategy stra, string orderid)
		{
			if (Trade == null) return null;
			return Trade.DicOrderField.TryGetValue(orderid, out OrderField order) ? order : null;
		}

		/// <summary>
		///  针对当前公式应用的绑定帐户、商品发送委托单
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static int A_SendOrder(this Strategy stra, DirectionType dire, OffsetType offset, double price, int lots)
		{
			if (Trade == null) return -1;
			if (offset == OffsetType.Close && Trade.DicInstrumentField[stra.InstrumentID].ExchangeID == Exchange.SHFE)
				return Trade.ClosePosi(stra.InstrumentID, dire == DirectionType.Buy ? DirectionType.Sell : DirectionType.Buy, price, lots);
			return Trade.ReqOrderInsert(stra.InstrumentID, dire, offset, price, lots, int.Parse(stra.Name) * 100);
		}

		/// <summary>
		/// 针对当前公式应用的绑定帐户、商品发送撤单指令
		/// </summary>
		/// <param name="stra"></param>
		/// <returns></returns>
		public static int A_DeleteOrder(this Strategy stra, string orderid)
		{
			if (Trade == null) return -1;
			return Trade.ReqOrderAction(orderid);
		}
	}
}
