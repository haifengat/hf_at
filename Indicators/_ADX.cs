#region
using System;

#endregion

namespace HaiFeng
{
	internal class ADX : Indicator
	{
		private DataSeries dmMinus;

		private DataSeries dmPlus;

		private DataSeries sumDmMinus;

		private DataSeries sumDmPlus;

		private DataSeries sumTr,
		                  tr;

		public ADX(int period) : base(null, period)
		{
		}

		public override void OnBarUpdate()
		{
			decimal trueRange = High[0] - Low[0];
			if (CurrentBar == 0)
			{
				this.tr[0] = (trueRange);
				this.dmPlus[0] = (0);
				this.dmMinus[0] = (0);
				this.sumTr[0] = (this.tr[0]);
				this.sumDmPlus[0] = (this.dmPlus[0]);
				this.sumDmMinus[0] = (this.dmMinus[0]);
				Value[0] = (50);
			}
			else
			{
				this.tr[0] = (Math.Max(Math.Abs(Low[0] - Close[1]), Math.Max(trueRange, Math.Abs(High[0] - Close[1]))));
				this.dmPlus[0] = (High[0] - High[1] > Low[1] - Low[0] ? Math.Max(High[0] - High[1], 0) : 0);
				this.dmMinus[0] = (Low[1] - Low[0] > High[0] - High[1] ? Math.Max(Low[1] - Low[0], 0) : 0);
				if (CurrentBar < Period)
				{
					this.sumTr[0] = (this.sumTr[1] + this.tr[0]);
					this.sumDmPlus[0] = (this.sumDmPlus[1] + this.dmPlus[0]);
					this.sumDmMinus[0] = (this.sumDmMinus[1] + this.dmMinus[0]);
				}
				else
				{
					this.sumTr[0] = (this.sumTr[1] - this.sumTr[1]/Period + this.tr[0]);
					this.sumDmPlus[0] = (this.sumDmPlus[1] - this.sumDmPlus[1]/Period + this.dmPlus[0]);
					this.sumDmMinus[0] = (this.sumDmMinus[1] - this.sumDmMinus[1]/Period + this.dmMinus[0]);
				}
				decimal diPlus = 100*(this.sumTr[0] == 0 ? 0 : this.sumDmPlus[0]/this.sumTr[0]);
				decimal diMinus = 100*(this.sumTr[0] == 0 ? 0 : this.sumDmMinus[0]/this.sumTr[0]);
				decimal diff = Math.Abs(diPlus - diMinus);
				decimal sum = diPlus + diMinus;
				Value[0] = (sum == 0 ? 50 : ((Period - 1)*Value[1] + 100*diff/sum)/Period);
			}
		}
	}
}
