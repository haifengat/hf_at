namespace HaiFeng
{
	internal class EMA : Indicator
	{
		public EMA(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			Value[0] = CurrentBar == 0 ? Input[0] : Input[0]*(2/(1 + Period)) + (1 - (2/(1 + Period)))*Value[1];
		}
	}
}
