using System;
using System.ComponentModel;
using Numeric = System.Decimal;

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
		public Numeric Price { get; set; }

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

		
		internal Numeric AvgEntryPriceShort { get; set; }

		internal Numeric AvgEntryPriceLong { get; set; }
		
		internal int PositionLong { get; set; }

		internal int PositionShort { get; set; }

		internal Numeric EntryDateLong { get; set; }

		internal Numeric EntryPriceLong { get; set; }

		internal Numeric ExitDateShort { get; set; }

		internal Numeric ExitPriceShort { get; set; }

		internal Numeric EntryDateShort { get; set; }

		internal Numeric EntryPriceShort { get; set; }

		internal Numeric ExitDateLong { get; set; }

		internal Numeric ExitPriceLong { get; set; }

		internal Numeric LastEntryDateShort { get; set; }

		internal Numeric LastEntryPriceShort { get; set; }

		internal Numeric LastEntryDateLong { get; set; }

		internal Numeric LastEntryPriceLong { get; set; }

	}
}