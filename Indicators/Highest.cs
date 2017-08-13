
#region

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class Highest : Indicator
	{
		private int lastBar;
		private double lastMax;
		private int runningBar;
		private double runningMax;
		private int thisBar;

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				this.runningMax = Input[0];
				this.lastMax = Input[0];
				this.runningBar = 0;
				this.lastBar = 0;
				this.thisBar = 0;
				return;
			}
			if (CurrentBar - this.runningBar >= Period)
			{
				this.runningMax = double.MinValue;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack > 0; barsBack--)
				{
					if (Input[barsBack].GreaterEqual(this.runningMax))
					{
						this.runningMax = Input[barsBack];
						this.runningBar = CurrentBar - barsBack;
					}
				}
			}
			if (this.thisBar != CurrentBar)
			{
				this.lastMax = this.runningMax;
				this.lastBar = this.runningBar;
				this.thisBar = CurrentBar;
			}
			if (Input[0].Greater(this.lastMax))
			{
				this.runningMax = Input[0];
				this.runningBar = CurrentBar;
			}
			else
			{
				this.runningMax = this.lastMax;
				this.runningBar = this.lastBar;
			}
			Value[0] = (this.runningMax);
		}

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Range(1, int.MaxValue)]
		[Parameter("Period", "Parameters")]
		public int Period { get; set; }
		#endregion
	}

	public partial class Indicator
	{
		private Highest[] cacheHighest;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Highest Highest(DataSeries input, int period)
		{
			if (cacheHighest != null)
				for (int idx = 0; idx < cacheHighest.Length; idx++)
					if (cacheHighest[idx] != null && cacheHighest[idx].Period == period && cacheHighest[idx].EqualsInput(input))
						return cacheHighest[idx];
			return CacheIndicator(new Highest { Period = period, Input = input }, ref cacheHighest);
		}
	}

	public partial class Strategy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Highest Highest(DataSeries input, int period)
		{
			return Indicator.Highest(input, period);
		}
	}
}
