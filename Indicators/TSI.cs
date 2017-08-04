#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. 
	/// Designed to determine both trend and overbought/oversold conditions, the TSI is 
	/// applicable to intraday time frames as well as long term trading.
	/// </summary>
	public class TSI : Indicator
	{
		private double constant1;
		private double constant2;
		private double constant3;
		private double constant4;
		private DataSeries fastEma;
		private DataSeries fastAbsEma;
		private DataSeries slowEma;
		private DataSeries slowAbsEma;

		protected override void Init()
		{
			Fast = 3;
			Slow = 14;

			constant1 = (2.0 / (1 + Slow));
			constant2 = (1 - (2.0 / (1 + Slow)));
			constant3 = (2.0 / (1 + Fast));
			constant4 = (1 - (2.0 / (1 + Fast)));

			fastAbsEma = new DataSeries(Input);
			fastEma = new DataSeries(Input);
			slowAbsEma = new DataSeries(Input);
			slowEma = new DataSeries(Input);
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				fastAbsEma[0] = 0;
				fastEma[0] = 0;
				slowAbsEma[0] = 0;
				slowEma[0] = 0;
				Value[0] = 0;
			}
			else
			{
				double momentum = Input[0] - Input[1];
				slowEma[0] = momentum * constant1 + constant2 * slowEma[1];
				fastEma[0] = slowEma[0] * constant3 + constant4 * fastEma[1];
				slowAbsEma[0] = Math.Abs(momentum) * constant1 + constant2 * slowAbsEma[1];
				fastAbsEma[0] = slowAbsEma[0] * constant3 + constant4 * fastAbsEma[1];
				Value[0] = fastAbsEma[0] == 0 ? 0 : 100 * fastEma[0] / fastAbsEma[0];
			}
		}

		#region Properties

		[Range(1, int.MaxValue)]
		[Parameter("Fast")]
		public int Fast { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Slow")]
		public int Slow { get; set; }
		#endregion
	}


	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private TSI[] cacheTSI;

		public TSI TSI(DataSeries input, int fast, int slow)
		{
			if (cacheTSI != null)
				for (int idx = 0; idx < cacheTSI.Length; idx++)
					if (cacheTSI[idx] != null && cacheTSI[idx].Fast == fast && cacheTSI[idx].Slow == slow && cacheTSI[idx].EqualsInput(input))
						return cacheTSI[idx];
			return CacheIndicator<TSI>(new TSI() { Fast = fast, Slow = slow, Input = input }, ref cacheTSI);
		}
	}

	public partial class Strategy
	{
		public TSI TSI(DataSeries input, int fast, int slow)
		{
			return Indicator.TSI(input, fast, slow);
		}
	}
}

#endregion
