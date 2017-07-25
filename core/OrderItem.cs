using System;
using System.ComponentModel;

namespace HaiFeng
{

	/// <summary>
	/// 	交易报单
	/// </summary>
	public class OrderItem
	{
		//在策略中被赋值
		internal int IndexEntryLong;
		internal int IndexLastEntryLong;
		internal int IndexExitShort;
		internal int IndexEntryShort;
		internal int IndexExitLong;
		internal int IndexLastEntryShort;

		/// <summary>
		/// 	时间
		/// </summary>
		[Description("时间:yyyyMMdd.HHmmss"), Category("字段"), ReadOnly(true)]
		public DateTime Date { get; set; }

		/// <summary>
		/// 	买卖
		/// </summary>
		[Description("买卖"), Category("字段"), ReadOnly(true)]
		public Direction Dir { get; set; }

		/// <summary>
		/// 	开平
		/// </summary>
		[Description("开平"), Category("字段"), ReadOnly(true)]
		public Offset Offset { get; set; }

		/// <summary>
		/// 	价格
		/// </summary>
		[Description("价格"), Category("字段"), ReadOnly(true)]
		public double Price { get; set; }

		/// <summary>
		/// 	手数
		/// </summary>
		[Description("手数"), Category("字段"), ReadOnly(true)]
		public int Lots { get; set; }

		/// <summary>
		/// 注释
		/// </summary>
		[Description("说明"), Category("字段"), ReadOnly(true)]
		public string Remark { get; set; }

		
		internal double AvgEntryPriceShort { get; set; }

		internal double AvgEntryPriceLong { get; set; }
		
		internal int PositionLong { get; set; }

		internal int PositionShort { get; set; }

		internal double EntryDateLong { get; set; }

		internal double EntryPriceLong { get; set; }

		internal double ExitDateShort { get; set; }

		internal double ExitPriceShort { get; set; }

		internal double EntryDateShort { get; set; }

		internal double EntryPriceShort { get; set; }

		internal double ExitDateLong { get; set; }

		internal double ExitPriceLong { get; set; }

		internal double LastEntryDateShort { get; set; }

		internal double LastEntryPriceShort { get; set; }

		internal double LastEntryDateLong { get; set; }

		internal double LastEntryPriceLong { get; set; }

	}
}