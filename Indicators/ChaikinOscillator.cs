#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace HaiFeng
{
	/// <summary>
	/// Chaikin Oscillator.
	/// http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:chaikin_oscillatorusing
	/// 佳庆指标CHAIK1N （Chaikin Oscillator），是由Marc Chaikin所发展的一种新成交量指标。他汲取JosephGranville和Larry Williams两位教授的理论精华，将A/D VOLUME指标加以改良，衍生出佳庆指标。MarcChaikin 本人对佳庆指标的设计原理，做了以下简要的叙述：
	///    为了将市场的内在动能，真实地表现在分析图表上。现有的技术指标，不管应用在大盘或者个股，都必须将成交量列入考虑的范围。在价格的波动趋势中，成交量分析有助于掌握股价本质上的强弱度。成交量与股价的背离现象，经常是确认反转讯号的唯一线索。JosephGranville和Larry Williams两位教授，直到60年代后期，才开始注意成交量与股价的关系。他们发现，必须在成交量总额中，筛选出较具意义的部分成交量，才能创造出更具代表性的指标。多年来，大部分的分析师，将上涨股的成交量全部视为正值，将下跌股的成交量全部视为负值。但是，这种论调存在着很大的缺点，必须加以改良，才足以反应股价的真实本质。
	///    以OBV累积能量线为例子。如果当日股价上涨，则当日所有的成交量总额，一律视为多头动能。如果当日股价下跌，则当日所有成交量总额，一律视为空头动能。这种论点太过于简化，而且不符合实际的现状。一段完整的趋势行情，会发生很多次重要的短、中期头部和底部，然而，OBV指标主要是针对极端的行情起作用。也就是说，只有在成交量极度萎缩或极度扩张的状况下，OBV指标才能发挥作用。
	///    Larry Williams将OBV加以改良，用来决定当日的成交量，属于多方或空方力道。OBV以当日的收盘价和前一日的收盘价比较。然而，Williams却以当日收盘价和当日开盘价比较，并且设计了一条累积能量线。如果收盘价高于开盘价，则开盘价距收盘价之间的上涨讽度，以正值百分比表示，并乘以当日成交量。如果收盘价低于开盘价，则开盘价距收盘价之间的下奠幅度，以负值百分比表示，再乘以当日成交量。经过这样的改良之后，其侦测量价背离的功能，显然更具有参考价值。
	///在使用本指标之前，必须注意下列三大要点：

	///以中间价为豁准，如果收盘价高于当日中间价，则当日成交量视为
	///正值。收盘价越接近当日最高价，其多头力道越强。如果收盘价低于当日中间价，则当日成交量视为负值。收盘价越接近当日最低价，其空头力道越强。

	///一波健全的上升趋势，必须包含强劲的多头力道。多头力道就像火箭升空，须要消耗的燃料一般。如果多头力道虚弱，则视为燃料不足，没有推升股价的条件。相反的，下降趋势经常伴随着较低的成交量。但是，波段下降趋势即将成熟前，经常会出现恐慌性抛压。这些卖盘，有部分来自于法人机构的大额结帐抛售。这一点，相当值得注意！股价不断的创新低点，成交量也相对呈现缓步的缩减。在这量缩低迷的期间，注意突然暴出的大量，这个现象发生时，经常是底部完成的讯号。

	///我们必须承认，没有任何一个指标是绝对完美的，建议搭配其他指标辅助，可以避免假讯号的发生。一般将21天的Envelope指标、超买超卖指标、佳庆指标组成一个指标群。运用在短、中期的交易上，成效相当良好。

	///2．	运用原则

	///佳庆指标与股价产生背离时，可视为反转讯号。（特别是其他搭配运用的指标群，正处于超买或超卖水平时）。

	///佳庆指标本身具有超买超卖的作用，但是，其超买和超卖的界限位置，随着个股不同而不同，须自行认定。建议至少须观察一年以上的走势图，从中搜寻其经常性的超买和超卖界限，才能界定出一个标准。

	///佳庆指标由负值向上穿越0轴时，为买进讯号。（注意！股价必须位于90天移动平均线之上，才可视为有效）。

	///佳庆指标由正值向下穿越0轴时，为卖出讯号。（注意！股价必须位于90天移动平卷线之下，才可视为有效）。 
	/// </summary>
	public class ChaikinOscillator : Indicator
	{
		private DataSeries cummulative;
		private EMA emaFast;
		private EMA emaSlow;
		private DataSeries moneyFlow, High, Low, Close, Volume;

		protected override void Init()
		{
			Fast = 3;
			Slow = 10;

			High = Inputs[0];
			Low = Inputs[1];
			Close = Inputs[2];
			Volume = Inputs[3];

			cummulative = new DataSeries(this.Input);
			moneyFlow = new DataSeries(this.Input);
			emaFast = EMA(cummulative, Fast);
			emaSlow = EMA(cummulative, Slow);
		}

		protected override void OnBarUpdate()
		{
			double close0 = Close[0];
			double low0 = Low[0];
			double high0 = High[0];

			moneyFlow[0] = Volume[0] * ((close0 - low0) - (high0 - close0)) / ((high0 - low0).ApproxCompare(0) == 0 ? 1 : (high0 - low0));
			cummulative[0] = moneyFlow[0] + (CurrentBar == 0 ? 0 : cummulative[1]);
			Value[0] = emaFast[0] - emaSlow[0];
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[Parameter("Fast", "Parameters")]
		public int Fast { get; set; }

		[Range(1, int.MaxValue)]
		[Parameter("Slow", "Parameters")]
		public int Slow { get; set; }
		#endregion
	}

	#region generated code. Neither change nor remove.
	public partial class Indicator
	{
		private ChaikinOscillator[] cacheChaikinOscillator;

		public ChaikinOscillator ChaikinOscillator(DataSeries high, DataSeries low, DataSeries close, DataSeries volume, int fast, int slow)
		{
			if (cacheChaikinOscillator != null)
				for (int idx = 0; idx < cacheChaikinOscillator.Length; idx++)
					if (cacheChaikinOscillator[idx] != null && cacheChaikinOscillator[idx].Fast == fast && cacheChaikinOscillator[idx].Slow == slow && cacheChaikinOscillator[idx].EqualsInput(high, low, close, volume))
						return cacheChaikinOscillator[idx];
			return CacheIndicator<ChaikinOscillator>(new ChaikinOscillator() { Fast = fast, Slow = slow, Inputs = new[] { high, low, close, volume } }, ref cacheChaikinOscillator);
		}
	}

	public partial class Strategy
	{
		public ChaikinOscillator ChaikinOscillator(int fast, int slow)
		{
			return ChaikinOscillator(Datas[0], fast, slow);
		}
		public ChaikinOscillator ChaikinOscillator(Data data,int fast, int slow)
		{
			return indicator.ChaikinOscillator(data.H, data.L, data.C, data.V, fast, slow);
		}
	}
}

#endregion
