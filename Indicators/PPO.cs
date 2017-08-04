#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#endregion

//This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// 百分比价格振荡器
	/// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as 
	/// a percentage. The PPO is found by subtracting the longer MA from the shorter MA and 
	/// then dividing the difference by the longer MA.
	/// </summary>
	public class PPO : Indicator
	{
		private EMA emaFast;
		private EMA emaSlow;

		protected override void Init()
		{
			Fast = 12;
			Slow = 26;
			Smooth = 9;

			emaFast = EMA(Input, Fast);
			emaSlow = EMA(Input, Slow);
		}

		protected override void OnBarUpdate()
		{
			double emaSlow0 = emaSlow[0];
			Default[0] = 100 * ((emaFast[0] - emaSlow0) / emaSlow0);
			Smoothed[0] = EMA(Values[0], Smooth)[0];
		}

		#region Properties
		[Browsable(false)]
		public DataSeries Default
		{
			get { return Values[0]; }
		}

		[Range(1, int.MaxValue)]
		[Parameter("Fast")]
		public int Fast { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Slow")]
		public int Slow { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Smooth")]
		public int Smooth { get; set; }

		[Browsable(false)]
		public DataSeries Smoothed
		{
			get { return Values[1]; }
		}
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private PPO[] cachePPO;

		public PPO PPO(DataSeries input, int fast, int slow, int smooth)
		{
			if (cachePPO != null)
				for (int idx = 0; idx < cachePPO.Length; idx++)
					if (cachePPO[idx] != null && cachePPO[idx].Fast == fast && cachePPO[idx].Slow == slow && cachePPO[idx].Smooth == smooth && cachePPO[idx].EqualsInput(input))
						return cachePPO[idx];
			return CacheIndicator<PPO>(new PPO() { Fast = fast, Slow = slow, Smooth = smooth, Input = input }, ref cachePPO);
		}
	}

	public partial class Strategy
	{

		public PPO PPO(DataSeries input, int fast, int slow, int smooth)
		{
			return Indicator.PPO(input, fast, slow, smooth);
		}
	}
}

#endregion
