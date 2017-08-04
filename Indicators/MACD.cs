
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class MACD : Indicator
	{
		private DataSeries fastEma;
		private DataSeries slowEma;
		private double constant1;
		private double constant2;
		private double constant3;
		private double constant4;
		private double constant5;
		private double constant6;

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public DataSeries Avg
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public DataSeries Default
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public DataSeries Diff
		{
			get { return Values[2]; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Fast", "Parameters")]
		public int Fast = 12;

		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Slow", "Parameters")]
		public int Slow = 26;

		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Smooth", "Parameters")]
		public int Smooth = 9;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			constant1 = 2.0 / (1 + Fast);
			constant2 = (1 - (2.0 / (1 + Fast)));
			constant3 = 2.0 / (1 + Slow);
			constant4 = (1 - (2.0 / (1 + Slow)));
			constant5 = 2.0 / (1 + Smooth);
			constant6 = (1 - (2.0 / (1 + Smooth)));

			fastEma = new DataSeries(this.Input);
			slowEma = new DataSeries(this.Input);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			double input0 = Input[0];

			if (CurrentBar == 0)
			{
				fastEma[0] = input0;
				slowEma[0] = input0;
				Value[0] = 0;
				Avg[0] = 0;
				Diff[0] = 0;
			}
			else
			{
				double fastEma0 = constant1 * input0 + constant2 * fastEma[1];
				double slowEma0 = constant3 * input0 + constant4 * slowEma[1];
				double macd = fastEma0 - slowEma0;
				double macdAvg = constant5 * macd + constant6 * Avg[1];

				fastEma[0] = fastEma0;
				slowEma[0] = slowEma0;
				Value[0] = macd;
				Avg[0] = macdAvg;
				Diff[0] = macd - macdAvg;
			}
		}
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private MACD[] cacheMACD;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fast"></param>
		/// <param name="slow"></param>
		/// <param name="smooth"></param>
		/// <returns></returns>
		public MACD MACD(DataSeries input, int fast, int slow, int smooth)
		{
			if (cacheMACD != null)
				for (int idx = 0; idx < cacheMACD.Length; idx++)
					if (cacheMACD[idx] != null && cacheMACD[idx].Fast == fast && cacheMACD[idx].Slow == slow && cacheMACD[idx].Smooth == smooth && cacheMACD[idx].EqualsInput(input))
						return cacheMACD[idx];
			return CacheIndicator(new MACD() { Fast = fast, Slow = slow, Smooth = smooth, Input = input }, ref cacheMACD);
		}
	}
	public partial class Strategy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fast"></param>
		/// <param name="slow"></param>
		/// <param name="smooth"></param>
		/// <returns></returns>
		public MACD MACD(DataSeries input, int fast, int slow, int smooth)
		{
			return Indicator.MACD(input, fast, slow, smooth);
		}
	}

	#endregion
}
