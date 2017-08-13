#region
using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public class Lowest : Indicator
	{
		private int lastBar;
		private double lastMin;
		private int runningBar;
		private double runningMin;
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
				this.runningMin = Input[0];
				this.lastMin = Input[0];
				this.runningBar = 0;
				this.lastBar = 0;
				this.thisBar = 0;
				return;
			}
			if (CurrentBar - this.runningBar >= Period)
			{
				this.runningMin = double.MaxValue;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack > 0; barsBack--)
				{
					if (Input[barsBack].LessEqual(this.runningMin))
					{
						this.runningMin = Input[barsBack];
						this.runningBar = CurrentBar - barsBack;
					}
				}
			}
			if (this.thisBar != CurrentBar)
			{
				this.lastMin = this.runningMin;
				this.lastBar = this.runningBar;
				this.thisBar = CurrentBar;
			}
			if (Input[0].LessEqual(this.lastMin))
			{
				this.runningMin = Input[0];
				this.runningBar = CurrentBar;
			}
			else
			{
				this.runningMin = this.lastMin;
				this.runningBar = this.lastBar;
			}
			Value[0] = this.runningMin;
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
		private Lowest[] cacheLowest;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public Lowest Lowest(DataSeries input, int period)
		{
			if (cacheLowest != null)
				for (int idx = 0; idx < cacheLowest.Length; idx++)
					if (cacheLowest[idx] != null && cacheLowest[idx].Period == period && cacheLowest[idx].EqualsInput(input))
						return cacheLowest[idx];
			return CacheIndicator(new Lowest { Period = period, Input = input }, ref cacheLowest);
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
		public Lowest Lowest(DataSeries input, int period)
		{
			return Indicator.Lowest(input, period);
		}
	}
}
