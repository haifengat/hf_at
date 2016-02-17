using System;
using System.ComponentModel;
using Numeric = System.Decimal;

namespace HaiFeng
{

	/// <summary>
	/// </summary>
	public class Bar
	{
		/// <summary>
		/// 	时间
		/// </summary>
		[Category("字段"), ReadOnly(true)]
		public DateTime D { get; set; }

		/// <summary>
		/// 	开盘价
		/// </summary>
		[Description("开盘价"), Category("字段"), ReadOnly(true)]
		public Numeric O { get; set; }

		/// <summary>
		/// 	最高价
		/// </summary>
		[Description("最高价"), Category("字段"), ReadOnly(true)]
		public Numeric H { get; set; }

		/// <summary>
		/// 	最低价
		/// </summary>
		[Description("最低价"), Category("字段"), ReadOnly(true)]
		public Numeric L { get; set; }

		/// <summary>
		/// 	收盘价
		/// </summary>
		[Description("收盘价"), Category("字段"), ReadOnly(true)]
		public Numeric C { get; set; }

		/// <summary>
		/// 	成交量
		/// </summary>
		[Description("成交量"), Category("字段"), ReadOnly(true)]
		public Numeric V { get; set; }

		/// <summary>
		/// 	持仓量
		/// </summary>
		[Description("持仓量"), Category("字段"), ReadOnly(true)]
		public Numeric I { get; set; }

		/// <summary>
		/// 	均价
		/// </summary>
		[Description("均价"), Category("字段"), ReadOnly(true)]
		public Numeric A { get; set; }

		/// <summary>
		/// 	前Bar的成交量:只用于中间计算
		/// </summary>
		internal Numeric PreVol { get; set; }
	}
}
