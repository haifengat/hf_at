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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>Equel 0 Great 1 Less -1 </returns>
		public static int ApproxCompare(this double x, double y)
		{
			return x - y < double.Epsilon ? 0 : (x > y ? 1 : -1);
		}

		/// <summary>
		/// 判断两个double变量相等
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool Equal(this double x, double y)
		{
			return x - y < Math.Abs(double.Epsilon);
		}

		/// <summary>
		/// 判断double变量x>y
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool Great(this double x, double y)
		{
			return x - y > double.Epsilon;
		}

		/// <summary>
		/// 判断double变量<!--x<y-->
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool Less(this double x, double y)
		{
			return x - y < double.Epsilon;
		}
	}
}
