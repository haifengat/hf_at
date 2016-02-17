namespace HaiFeng
{
	public class MACD : Indicator
	{
		private readonly int _fast;
		private readonly int _slow;
		private readonly int _smooth;
		private readonly DataSeries fastEma = new DataSeries();
		private readonly DataSeries slowEma = new DataSeries();

		//输出序列
		public DataSeries Fast,
		                 Slow;

		public MACD(DataSeries input, int fast, int slow, int smooth) : base(input, fast, slow, smooth)
		{
			this._fast = fast;
			this._slow = slow;
			this._smooth = smooth;
		}

		public override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				this.fastEma[0] = Input[0];
				this.slowEma[0] = Input[0];
				Value[0] = 0;
				this.Fast[0] = 0;
				this.Slow[0] = 0;
			}
			else
			{
				this.fastEma[0] = (2/(1 + this._fast))*Input[0] + (1 - (2/(1 + this._fast)))*this.fastEma[1];
				this.slowEma[0] = (2/(1 + this._slow))*Input[0] + (1 - (2/(1 + this._slow)))*this.slowEma[1];
				decimal macd = this.fastEma[0] - this.slowEma[0];
				decimal macdAvg = (2/(1 + this._smooth))*macd + (1 - (2/(1 + this._smooth)))*this.Fast[1];
				this.Fast[0] = macd;
				this.Slow[0] = macdAvg;
				Value[0] = macd - macdAvg;
			}
		}
	}
}
