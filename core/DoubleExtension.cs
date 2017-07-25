using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public static class DoubleExtension
	{
		static double Epsilon = 0.000001;
		/// <summary>
		/// double 变量比较
		/// 用法：double a; if(a.ApproxCompare(b)==0) ...
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>Equel 0 Great 1 Less -1 </returns>
		public static int ApproxCompare(this double x, double y) { return Math.Abs(x - y) < Epsilon ? 0 : (x > y ? 1 : -1); }

		/// <summary>
		/// 相等
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool Equal(this double x, double y) { return x.ApproxCompare(y) == 0; }

		/// <summary>
		/// 大于
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool Greater(this double x, double y) { return x.ApproxCompare(y) > 0; }

		/// <summary>
		/// 小于
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool Less(this double x, double y) { return x.ApproxCompare(y) < 0; }

		/// <summary>
		/// 小于等于
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool LessEqual(this double x, double y) { return x.ApproxCompare(y) <= 0; }

		/// <summary>
		/// 大于等于
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool GreaterEqual(this double x, double y) { return x.ApproxCompare(y) >= 0; }
	}
}
