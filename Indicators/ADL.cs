#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or 
	/// out of an instrument by identifying the position of the close of the period in relation to that period's high/low range.
	/// </summary>
	public class ADL : Indicator
	{
		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			High = Inputs[0];
			Low = Inputs[1];
			Close = Inputs[2];
			Volume = Inputs[3];
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void OnBarUpdate()
		{
			double high0 = High[0];
			double low0 = Low[0];
			double close0 = Close[0];

			AD[0] = ((CurrentBar == 0 ? 0 : AD[1]) + (high0.ApproxCompare(low0) != 0 ? (((close0 - low0) - (high0 - close0)) / (high0 - low0)) * Volume[0] : 0));
		}

		DataSeries High, Low, Close, Volume;

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries AD { get { return Values[0]; } }
		#endregion
	}

	#region generated code. Neither change nor remove.

	public partial class Indicator
	{
		private ADL[] cacheADL;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close"></param>
		/// <param name="volume"></param>
		/// <returns></returns>
		public ADL ADL(DataSeries high, DataSeries low, DataSeries close, DataSeries volume)
		{
			if (cacheADL != null)
				for (int idx = 0; idx < cacheADL.Length; idx++)
					if (cacheADL[idx] != null && cacheADL[idx].EqualsInput(high, low, close, volume))
						return cacheADL[idx];
			return CacheIndicator<ADL>(new ADL { Inputs = new[] { high, low, close, volume } }, ref cacheADL);
		}
	}
	public partial class Strategy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ADL ADL()
		{
			return indicator.ADL(H, L, C, V);
		}
	}
}

#endregion
