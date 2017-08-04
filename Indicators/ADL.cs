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
		internal DataSeries High, Low, Volume;
		DataSeries Close;	//只在Close被修改时才会触发指标计算,以避免多个input造成指标计算多次的性能问题.

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			Close = Input;
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
			var cat = cacheADL;
			if (cacheADL != null)
				for (int idx = 0; idx < cacheADL.Length; idx++)
					if (cacheADL[idx] != null && cat[idx].High == high && cat[idx].Low == low&&cat[idx].Volume==volume && cacheADL[idx].EqualsInput(close))
						return cacheADL[idx];
			return CacheIndicator<ADL>(new ADL { High = high, Low = low, Volume = volume, Input = close }, ref cacheADL);
		}
	}
	public partial class Strategy
	{
		public ADL ADL()
		{
			return ADL(Datas[0]);
		}
		public ADL ADL(Data data)
		{
			return indicator.ADL(data.H, data.L, data.C, data.V);
		}
	}
}

#endregion
