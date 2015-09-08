namespace HaiFeng
{
	internal class KDJ : Indicator
	{
		private readonly DataSeries C;
		private readonly int Length; //, SlowLength, SmoothLength;

		public DataSeries KValue, DValue, JValue;

		private DataSeries HH, LL;
		private DataSeries RSV;

		private Highest highest;
		private Lowest lowest;
		private SMA smaK, smaD;//导致程序报错

		public KDJ(int length, int slow, int smooth, DataSeries h, DataSeries l, DataSeries c)
			: base(null, length)
		{
			this.C = c;
			this.Length = length;
			highest = new Highest(h, length);
			lowest = new Lowest(l, length);
			HH = highest.Value;
			LL = lowest.Value;
			smaK = new SMA(RSV, slow);
			smaD = new SMA(RSV, smooth);
		}

		public override void OnBarUpdate()
		{
			//if (Math.Abs(this.HH[1] - this.LL[1]) > 1E-6)
			if(this.HH[1] != this.LL[1])
			{
				this.RSV[0] = (this.C[0] - this.LL[0]) / (this.HH[0] - this.LL[0]) * 100;
				KValue[0] = smaK.Value[0];
				DValue[0] = smaD.Value[0];
			}
			else
			{
				this.KValue[0] = 50;
				this.DValue[0] = 50;
			}

			this.JValue[0] = this.KValue[0] * 3 - this.DValue[0] * 2;
		}
	}
}
