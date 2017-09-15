using System;
using System.Runtime.InteropServices;

namespace HaiFeng
{
	/// <summary>
	/// 合约信息
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class InstrumentInfo
	{
		/// <summary>
		/// 合约代码
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string InstrumentID;

		/// <summary>
		/// 产品代码
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string ProductID;

		/// <summary>
		/// 合约数量乘数
		/// </summary>
		public int VolumeMultiple;

		/// <summary>
		/// 最小变动价位
		/// </summary>
		public double PriceTick;
	}
}
