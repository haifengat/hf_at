

namespace HaiFeng
{
	internal class ADL : Indicator
	{
		public ADL() : base(null, 0)
		{
		}

		public override void OnBarUpdate()
		{
			Value[0] = (CurrentBar == 0 ? 0 : Value[1]) + (High[0] != Low[0] ? (((Close[0] - Low[0]) - (High[0] - Close[0]))/(High[0] - Low[0]))*Volume[0] : 0);
		}
	}
}
