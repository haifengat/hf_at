namespace HaiFeng
{
	internal class SUM : Indicator
	{
		public SUM(DataSeries input, int period) : base(input, period)
		{
		}

		public override void OnBarUpdate()
		{
			Value[0] = (Input[0] + (CurrentBar > 0 ? Value[1] : 0) - (CurrentBar >= Period ? Input[Period] : 0));
		}
	}
}
