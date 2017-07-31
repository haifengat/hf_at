
// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into 
	/// or out of a security. When the security closes higher than the previous close, all 
	/// of the day's volume is considered up-volume. When the security closes lower than the 
	/// previous close, all of the day's volume is considered down-volume.
	/// </summary>
	public class OBV : Indicator
	{
		DataSeries Close, Volume;
		protected override void Init()
		{
			Close = Inputs[0];
			Volume = Inputs[1];
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value[0] = 0;
			else
			{
				double close0 = Close[0];
				double close1 = Close[1];

				if (close0 > close1)
					Value[0] = Value[1] + Volume[0];
				else if (close0 < close1)
					Value[0] = Value[1] - Volume[0];
				else
					Value[0] = Value[1];
			}
		}
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private OBV[] cacheOBV;

		public OBV OBV(DataSeries close, DataSeries volume)
		{
			if (cacheOBV != null)
				for (int idx = 0; idx < cacheOBV.Length; idx++)
					if (cacheOBV[idx] != null && cacheOBV[idx].EqualsInput(close, volume))
						return cacheOBV[idx];
			return CacheIndicator<OBV>(new OBV { Inputs = new[] { close, volume } }, ref cacheOBV);
		}
	}

	public partial class Strategy
	{
		public OBV OBV()
		{
			return OBV(Datas[0]);
		}

		public OBV OBV(Data data)
		{
			return indicator.OBV(data.C, data.V);
		}
	}
}

#endregion
