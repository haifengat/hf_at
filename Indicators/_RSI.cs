#region
using System;

#endregion

namespace HaiFeng
{
	internal class RSI : Indicator
	{
		private readonly int Smooth;
		private readonly SMA smaDown;
		private readonly SMA smaUp;
		public DataSeries Avg;
		private DataSeries avgDown;
		private DataSeries avgUp;
		private DataSeries down;
		private DataSeries up;

		public RSI(DataSeries input, int period, int smooth) : base(input, period, smooth)
		{
			this.Smooth = smooth;
			this.smaUp = new SMA(this.up, Period);
			this.smaDown = new SMA(this.down, Period);
		}

		public override void OnBarUpdate()
		{
			Value[0] = this.Avg[0] = 50; //初值
			if (CurrentBar == 0)
			{
				this.down[0] = 0;
				this.up[0] = 0;
				if (Period < 3)
				{
					this.Avg[0] = 50;
				}
				return;
			}
			this.down[0] = Math.Max(Input[1] - Input[0], 0);
			this.up[0] = Math.Max(Input[0] - Input[1], 0);
			if ((CurrentBar + 1) < Period)
			{
				if ((CurrentBar + 1) == (Period - 1))
				{
					this.Avg[0] = 50;
				}
				return;
			}
			if ((CurrentBar + 1) == Period)
			{
				// First averages 
				this.avgDown[0] = this.smaDown.Value[0];
				this.avgUp[0] = this.smaUp.Value[0];
			}
			else
			{
				// Rest of averages are smoothed
				this.avgDown[0] = (this.avgDown[1]*(Period - 1) + this.down[0])/Period;
				this.avgUp[0] = (this.avgUp[1]*(Period - 1) + this.up[0])/Period;
			}
			decimal rsi = this.avgDown[0] == 0 ? 100 : 100 - 100/(1 + this.avgUp[0]/this.avgDown[0]);
			decimal rsiAvg = (2/(1 + this.Smooth))*rsi + (1 - (2/(1 + this.Smooth)))*this.Avg[1];
			this.Avg[0] = rsiAvg;
			Value[0] = rsi;
		}
	}
}
