
// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// Calculates the range of a bar.
	/// </summary>
	public class Range : Indicator
	{
		DataSeries High, Low;

		protected override void Init()
		{
			High = Inputs[0];
			Low = Inputs[1];
		}

		protected override void OnBarUpdate()
		{
			Value[0] = High[0] - Low[0];
		}
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private Range[] cacheRange;

		public Range Range(DataSeries high, DataSeries low)
		{
			if (cacheRange != null)
				for (int idx = 0; idx < cacheRange.Length; idx++)
					if (cacheRange[idx] != null && cacheRange[idx].EqualsInput(high, low))
						return cacheRange[idx];
			return CacheIndicator<Range>(new Range { Inputs = new[] { high, low } }, ref cacheRange);
		}
	}

	public partial class Strategy
	{
		public Range Range()
		{
			return Range(Datas[0]);
		}
		public Range Range(Data data)
		{
			return indicator.Range(data.H, data.L);
		}
	}
}

#endregion
