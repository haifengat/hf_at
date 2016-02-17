namespace HaiFeng
{
	internal class Bollinger : Indicator
	{
		private readonly decimal NumStdDev;
		private readonly SMA sma;
		private readonly StdDev stdDev;

		public DataSeries Lower;

		public DataSeries Middle;
		public DataSeries Upper;

		public Bollinger(DataSeries input, int period, decimal numStdDev) : base(input, period)
		{
			this.NumStdDev = numStdDev;
			this.sma = new SMA(input, period);
			this.stdDev = new StdDev(input, period);
		}

		public override void OnBarUpdate()
		{
			decimal smaValue = this.sma.Value[0];
			decimal stdDevValue = this.stdDev.Value[0];
			this.Upper[0] = (smaValue + this.NumStdDev*stdDevValue);
			this.Middle[0] = (smaValue);
			this.Lower[0] = (smaValue - this.NumStdDev*stdDevValue);
		}
	}
}
