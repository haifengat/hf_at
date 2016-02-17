
#region

using System;

#endregion

/*
 * 
 * 作者:吴俊海		时间:2012/3/17 16:13:06
 * 
 * 文件名:  ZigZag
 * 
 * CLR版本: 4.0.30319.239
 * 
 */

namespace HaiFeng
{
	internal class ZigZag : Indicator
	{
		#region Variables
		private readonly decimal PriceTick = 10M;
		private decimal currentZigZagHigh;
		private decimal currentZigZagLow;
		private DeviationType deviationType = DeviationType.Points;
		private decimal deviationValue = 0.5M;
		private int lastSwingIdx = -1;
		private decimal lastSwingPrice;
		private int trendDir; // 1 = trend up, -1 = trend down, init = 0
		private bool useHighLow = false;
		private DataSeries zigZagHighSeries;
		private DataSeries zigZagHighZigZags;
		private DataSeries zigZagLowSeries;
		private DataSeries zigZagLowZigZags;
		#endregion

		public ZigZag(DataSeries input, int period, decimal priceTick)
			: base(input, period)
		{
			this.PriceTick = priceTick;
		}

		/// <summary>
		/// 	Returns the number of bars ago a zig zag low occurred. Returns a value of -1 if a zig zag low is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"> </param>
		/// <param name="instance"> </param>
		/// <param name="lookBackPeriod"> </param>
		/// <returns> </returns>
		public int LowBar(int barsAgo, int instance, int lookBackPeriod)
		{
			if (instance < 1)
			{
				throw new Exception(GetType().Name + ".LowBar: instance must be greater/equal 1 but was " + instance);
			}
			else if (barsAgo < 0)
			{
				throw new Exception(GetType().Name + ".LowBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			}
			else if (barsAgo >= Count)
			{
				throw new Exception(GetType().Name + ".LowBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");
			}
			for (int idx = CurrentBar - barsAgo - 1; idx >= CurrentBar - barsAgo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
				{
					return -1;
				}
				if (idx >= Count) // zigZagLowZigZags.Count)
				{
					continue;
				}
				if (this.zigZagLowZigZags[idx].Equals(0.0))
				{
					continue;
				}
				if (instance == 1) // 1-based, < to be save
				{
					return CurrentBar - idx;
				}
				instance--;
			}
			return -1;
		}

		/// <summary>
		/// 	Returns the number of bars ago a zig zag high occurred. Returns a value of -1 if a zig zag high is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"> </param>
		/// <param name="instance"> </param>
		/// <param name="lookBackPeriod"> </param>
		/// <returns> </returns>
		public int HighBar(int barsAgo, int instance, int lookBackPeriod)
		{
			if (instance < 1)
			{
				throw new Exception(GetType().Name + ".HighBar: instance must be greater/equal 1 but was " + instance);
			}
			else if (barsAgo < 0)
			{
				throw new Exception(GetType().Name + ".HighBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			}
			else if (barsAgo >= Count)
			{
				throw new Exception(GetType().Name + ".HighBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");
			}
			for (int idx = CurrentBar - barsAgo - 1; idx >= CurrentBar - barsAgo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
				{
					return -1;
				}
				if (idx >= Count) // zigZagHighZigZags.Count)
				{
					continue;
				}
				if (this.zigZagHighZigZags[idx].Equals(0.0))
				{
					continue;
				}
				if (instance <= 1) // 1-based, < to be save
				{
					return CurrentBar - idx;
				}
				instance--;
			}
			return -1;
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar < 2) // need 3 bars to calculate Low/High
			{
				this.zigZagHighSeries[0] = (0);
				this.zigZagHighZigZags[0] = (0);
				this.zigZagLowSeries[0] = (0);
				this.zigZagLowZigZags[0] = (0);
				return;
			}
			// Initialization
			if (this.lastSwingPrice == 0)
			{
				this.lastSwingPrice = Input[0];
			}
			DataSeries highSeries = High;
			DataSeries lowSeries = Low;
			if (!this.useHighLow)
			{
				highSeries = Input;
				lowSeries = Input;
			}
			// Calculation always for 1-bar ago !
			bool isSwingHigh = highSeries[1] >= highSeries[0] && highSeries[1] >= highSeries[2];
			bool isSwingLow = lowSeries[1] <= lowSeries[0] && lowSeries[1] <= lowSeries[2];
			bool isOverHighDeviation = (this.deviationType == DeviationType.Percent && this.IsPriceGreater(highSeries[1], (this.lastSwingPrice * (1 + this.deviationValue * 0.01M)))) || (this.deviationType == DeviationType.Points && this.IsPriceGreater(highSeries[1], this.lastSwingPrice + this.deviationValue));
			bool isOverLowDeviation = (this.deviationType == DeviationType.Percent && this.IsPriceGreater(this.lastSwingPrice * (1 - this.deviationValue * 0.01M), lowSeries[1])) || (this.deviationType == DeviationType.Points && this.IsPriceGreater(this.lastSwingPrice - this.deviationValue, lowSeries[1]));
			decimal saveValue = 0;
			bool addHigh = false;
			bool addLow = false;
			bool updateHigh = false;
			bool updateLow = false;
			this.zigZagHighZigZags[0] = (0);
			this.zigZagLowZigZags[0] = (0);
			if (!isSwingHigh && !isSwingLow)
			{
				this.zigZagHighSeries[0] = (this.currentZigZagHigh);
				this.zigZagLowSeries[0] = (this.currentZigZagLow);
				return;
			}
			if (this.trendDir <= 0 && isSwingHigh && isOverHighDeviation)
			{
				saveValue = highSeries[1];
				addHigh = true;
				this.trendDir = 1;
			}
			else if (this.trendDir >= 0 && isSwingLow && isOverLowDeviation)
			{
				saveValue = lowSeries[1];
				addLow = true;
				this.trendDir = -1;
			}
			else if (this.trendDir == 1 && isSwingHigh && this.IsPriceGreater(highSeries[1], this.lastSwingPrice))
			{
				saveValue = highSeries[1];
				updateHigh = true;
			}
			else if (this.trendDir == -1 && isSwingLow && this.IsPriceGreater(this.lastSwingPrice, lowSeries[1]))
			{
				saveValue = lowSeries[1];
				updateLow = true;
			}
			if (addHigh || addLow || updateHigh || updateLow)
			{
				if (updateHigh && this.lastSwingIdx >= 0)
				{
					this.zigZagHighZigZags[0] = CurrentBar - this.lastSwingIdx;
					Value[0] = (CurrentBar - this.lastSwingIdx);
				}
				else if (updateLow && this.lastSwingIdx >= 0)
				{
					this.zigZagLowZigZags[CurrentBar - this.lastSwingIdx] = 0;
					Value[0] = (CurrentBar - this.lastSwingIdx); //此处用了reset,可能与[0]不一致
				}
				if (addHigh || updateHigh)
				{
					this.zigZagHighZigZags[1] = saveValue;
					this.zigZagHighZigZags[0] = 0;
					this.currentZigZagHigh = saveValue;
					this.zigZagHighSeries[1] = this.currentZigZagHigh;
					Value[1] = this.currentZigZagHigh;
				}
				else if (addLow || updateLow)
				{
					this.zigZagLowZigZags[1] = saveValue;
					this.zigZagLowZigZags[0] = 0;
					this.currentZigZagLow = saveValue;
					this.zigZagLowSeries[1] = this.currentZigZagLow;
					Value[1] = this.currentZigZagLow;
				}
				this.lastSwingIdx = CurrentBar - 1;
				this.lastSwingPrice = saveValue;
			}
			this.zigZagHighSeries[0] = (this.currentZigZagHigh);
			this.zigZagLowSeries[0] = (this.currentZigZagLow);
		}

		private bool IsPriceGreater(decimal a, decimal b)
		{
			if (a > b && a - b > this.PriceTick / 2)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	internal enum DeviationType
	{
		Percent,
		Points
	}
}
