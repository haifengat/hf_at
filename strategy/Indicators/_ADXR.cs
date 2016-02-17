namespace HaiFeng
{
	internal class ADXR : Indicator
	{
		private readonly int Interval;
		private readonly ADX adx;

		public ADXR(DataSeries input, int period, int interval) : base(input, period, interval)
		{
			this.Interval = interval;
			this.adx = new ADX(period);
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar < this.Interval)
			{
				Value[0] = ((this.adx.Value[0] + this.adx.Value[CurrentBar])/2);
			}
			else
			{
				Value[0] = ((this.adx.Value[0] + this.adx.Value[this.Interval])/2);
			}
		}
	}
}
