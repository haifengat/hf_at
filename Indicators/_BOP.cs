namespace HaiFeng
{
	/// <summary>
	/// 	The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
	/// </summary>
	internal class BOP : Indicator
	{
		private readonly SMA sma;
		private DataSeries bop;

		public BOP(int period) : base(null, period)
		{
			this.sma = new SMA(this.bop, period);
		}

		public override void OnBarUpdate()
		{
			if ((High[0] - Low[0]) == 0)
			{
				this.bop[0] = (0);
			}
			else
			{
				this.bop[0] = ((Close[0] - Open[0])/(High[0] - Low[0]));
			}
			Value[0] = (this.sma.Value[0]);
		}
	}
}
