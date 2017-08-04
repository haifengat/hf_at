
// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// Calculates the range of a bar.
	/// </summary>
	public class Range : Indicator
	{
		internal DataSeries High, Low;
		//DataSeries Close;   //只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		protected override void Init()
		{
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

		public Range Range(DataSeries high, DataSeries low, DataSeries close)
		{
			var cat = cacheRange;
			if (cacheRange != null)
				for (int idx = 0; idx < cacheRange.Length; idx++)
					if (cacheRange[idx] != null && cat[idx].High == high && cat[idx].Low == low && cacheRange[idx].EqualsInput(close))
						return cacheRange[idx];
			return CacheIndicator<Range>(new Range { High = high, Low = low, Input = close }, ref cacheRange);
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
			return Indicator.Range(data.H, data.L, data.C);
		}
	}
}

#endregion
