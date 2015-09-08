
using Numeric = System.Decimal;
using NumericSeries = HaiFeng.DataSeries;

namespace HaiFeng
{
	public class Test : Strategy
	{
		public override void Initialize()
		{
		}
		#region Params
		[Parameter("开盘30分钟前不交易")]
		Numeric waitPeriodMins = 30M;
		[Parameter("停止开仓时间")]
		Numeric initTradesEndTime = 14.30M;
		[Parameter("上午收盘时间")]
		Numeric liqRevEndTime = 11.25M;
		[Parameter("收盘平仓时间")]
		Numeric ExitOnCloseMins = 15.10M;
		[Parameter("突破开仓幅度1")]
		Numeric thrustPrcnt1 = 0.30M;
		[Parameter("突破开仓幅度2")]
		Numeric thrustPrcnt2 = 0.60M;
		[Parameter("breakOutPrcnt")]
		Numeric breakOutPrcnt = 0.25M;
		[Parameter("failedBreakOutPrcnt")]
		Numeric failedBreakOutPrcnt = 0.25M;
		[Parameter("protStopPrcnt1")]
		Numeric protStopPrcnt1 = 0.25M;
		[Parameter("protStopPrcnt2")]
		Numeric protStopPrcnt2 = 0.15M;
		[Parameter("突破幅度边界")]
		Numeric protStopAmt = 3.00M;
		[Parameter("breakEvenPrcnt")]
		Numeric breakEvenPrcnt = 0.50M;
		[Parameter("10天震幅计算周期")]
		Numeric avgRngLength = 10M;
		[Parameter("10天实盘震幅计算周期")]
		Numeric avgOCLength = 10M;

		[Parameter("下单手数")]
		Numeric Lots = 1M;
		[Parameter("委托滑点设置")]
		Numeric OffSet = 2M;

		#endregion
		#region Vars
		NumericSeries averageRange;  //过去10天平均日内震幅
		NumericSeries averageOCRange;  //过去10天平均实体震幅
		NumericSeries mycanTrade;  //是否允许交易标志
		bool buyEasierDay;  //好买日标志
		bool sellEasierDay;  //好卖日标志
		NumericSeries buyBOPoint;  //突破开多价格
		NumericSeries sellBOPoint;  //突破开空价格
		Numeric longBreakPt = 0M;// 
		Numeric shortBreakPt = 0M;// 
		Numeric longFBOPoint = 0M;// 
		Numeric shortFBOPoint = 0M;// 
		NumericSeries mybarCount;  //
		NumericSeries intraHigh;  //
		NumericSeries intraLow;  //
		NumericSeries buysToday;  //
		NumericSeries sellsToday;  //
		NumericSeries currTrdType;  //止损反手开仓类型标志，多翻空为
		Numeric longLiqPoint = 0M;// 
		Numeric shortLiqPoint = 0M;// 
		Numeric yesterdayOCRRange = 0M;// 
		NumericSeries intraTradeHigh;  //
		NumericSeries intraTradeLow;  //
		NumericSeries tsMarketPosition1;  //

		Numeric DayOpen;// 
		Numeric PreDayHigh;// 
		Numeric PreDayLow;// 
		Numeric PreDayOpen;// 
		Numeric PreDayClose;// 
		Numeric i;// 
		Numeric MyPrice;// 
		Numeric MinPoint;// 

		#endregion

		//Begin
		public override void OnBarUpdate()
		{
			MinPoint = MinMove * PriceScale;
			DayOpen = OpenD(0);
			PreDayOpen = OpenD(1);
			PreDayHigh = HighD(1);
			PreDayLow = LowD(1);
			PreDayClose = CloseD(1);
			//求过去10天平均日内震幅averageRange
			averageRange[0] = PreDayHigh - PreDayLow;
			for (i = 2; i < avgRngLength; ++i)
			{
				averageRange[0] = averageRange[0] + HighD(i) - LowD(i);
			}
			averageRange[0] = averageRange[0] / avgRngLength;
			//求昨天实体幅度yesterdayOCRRange
			yesterdayOCRRange = Abs(PreDayOpen - PreDayClose);
			//求过去10天平均实体震幅averageOCRange
			averageOCRange[0] = Abs(PreDayOpen - PreDayClose);
			for (i = 2; i < avgOCLength; ++i)
			{
				averageOCRange[0] = averageOCRange[0] + Abs(OpenD(i) - CloseD(i));
			}
			averageOCRange[0] = averageOCRange[0] / avgOCLength;
			//
			mycanTrade[0] = 0;
			if (yesterdayOCRRange < 0.85M * averageOCRange[0]) 
				mycanTrade[0] = 1;
			Commentary("mycanTrade=" + Text(mycanTrade[0]));
			//
			//buyEasierDay[0]= false;
			//sellEasierDay[0]= false;
			//if (PreDayClose<=CloseD(2))buyEasierDay[0]= true;
			//if (PreDayClose>CloseD(2))sellEasierDay[0]= true;
			buyEasierDay = PreDayClose <= CloseD(2);
			sellEasierDay = PreDayClose > CloseD(2);

			//分别计算好买日buyEasierDay、好卖日sellEasierDay的突破开仓点
			if (buyEasierDay)
			{
				buyBOPoint[0] = DayOpen + thrustPrcnt1 * averageRange[0];
				sellBOPoint[0] = DayOpen - thrustPrcnt2 * averageRange[0];
			}
			if (sellEasierDay)
			{
				sellBOPoint[0] = DayOpen - thrustPrcnt1 * averageRange[0];
				buyBOPoint[0] = DayOpen + thrustPrcnt2 * averageRange[0];
			}
			//
			longBreakPt = PreDayHigh + breakOutPrcnt * averageRange[0];
			shortBreakPt = PreDayLow - breakOutPrcnt * averageRange[0];
			shortFBOPoint = PreDayHigh - failedBreakOutPrcnt * averageRange[0];
			longFBOPoint = PreDayLow + failedBreakOutPrcnt * averageRange[0];

			if (Date[0] != Date[1])
			{
				mybarCount[0] = 1;//当日第1支K线
				intraHigh[0] = High[0];//记录日内高点
				intraLow[0] = Low[0];//记录日内低点
				buysToday[0] = 0;//多头开仓次数
				sellsToday[0] = 0;//空头开仓次数
				currTrdType[0] = 0;//止损反手开仓类型标志，多翻空为+2，空翻多为-2
				tsMarketPosition1[0] = 0;
			}
			else
			{
				intraHigh[0] = Max(intraHigh[1], High[1]);
				intraLow[0] = Min(intraLow[1], Low[1]);
				mybarCount[0] = mybarCount[1] + 1;
			}

			if (mybarCount[0] <= (waitPeriodMins / BarInterval) || mycanTrade[0] == 0) return;//开盘30分钟前不交易

			//if (mybarCount[0]>waitPeriodMins/BarInterval && mycanTrade[0]=1)
			//{
			//{havewewaitedlongenough—waitPeriodMinisaninputvariable && BarIntervalissetbyTradeStation.WaitPeriodMins=30 && BarInterval=5,so30/5=6}
			if (MarketPosition == 1 && BarsSinceLastEntry >= 1)
			{
				intraTradeHigh[0] = Max(intraTradeHigh[1], High[1]);//记录多头开仓后的最高点
				buysToday[0] = 1;//多头开仓标记
			}
			if (MarketPosition == -1 && BarsSinceLastEntry >= 1)
			{
				intraTradeLow[0] = Min(intraTradeLow[1], Low[1]);//记录空头开仓后的最低点
				sellsToday[0] = 1;//空头开仓标记
			}
			//if (buysToday[0]=0 && Time<initTradesEndTime/100)Buy((int)"LBreakOut")nextbaratbuyBOPoint[0]stop;
			//突破开多
			if (MarketPosition != 1 && buysToday[0] == 0 && Time[0] < initTradesEndTime / 100 && High[0] >= buyBOPoint[0])
			{
				MyPrice = buyBOPoint[0];
				if (Open[0] > MyPrice) MyPrice = Open[0];
				MyPrice = MyPrice + OffSet * MinPoint;
				Buy((int)Lots, MyPrice);
				intraTradeHigh[0] = AvgEntryPrice;
				buysToday[0] = 1;//多头开仓标记
				return;
			}
			//if (sellsToday[0]=0 && Time<initTradesEndTime/100)thenSellShort((int)"SBreakout")nextbaratsellBOPoint[0]stop;
			//突破开空
			if (MarketPosition != -1 && sellsToday[0] == 0 && Time[0] < initTradesEndTime / 100 && Low[0] <= sellBOPoint[0])
			{
				MyPrice = sellBOPoint[0];
				if (Open[0] < MyPrice) MyPrice = Open[0];
				MyPrice = MyPrice - OffSet * MinPoint;
				SellShort((int)Lots, MyPrice);
				intraTradeLow[0] = AvgEntryPrice;
				sellsToday[0] = 1;
				return;
			}
			//if (intraHigh[0]>longBreakPt && sellsToday[0]=0 && Time<initTradesEndTime/100)thenSellShort((int)"SfailedBO")nextbaratshortFBOPointstop;
			//假突破开空
			if (MarketPosition != -1 && intraHigh[0] > longBreakPt && sellsToday[0] == 0 && Time[0] < initTradesEndTime / 100 && Low[0] <= shortFBOPoint)
			{
				MyPrice = shortFBOPoint;
				if (Open[0] < MyPrice) MyPrice = Open[0];
				MyPrice = MyPrice - OffSet * MinPoint;
				SellShort((int)Lots, MyPrice);
				intraTradeLow[0] = AvgEntryPrice;
				sellsToday[0] = 1;
				return;
			}
			//if (intraLow[0]<shortBreakPt && buysToday[0]=0 && Time<initTradesEndTime/100)thenBuy((int)"BfailedBO")nextbaratlongFBOPointstop;
			//假突破开多
			if (MarketPosition != 1 && intraLow[0] < shortBreakPt && buysToday[0] == 0 && Time[0] < initTradesEndTime / 100 && High[0] >= longFBOPoint)
			{
				MyPrice = longFBOPoint;
				if (Open[0] > MyPrice) MyPrice = Open[0];
				MyPrice = MyPrice + OffSet * MinPoint;
				Buy((int)Lots, MyPrice);
				intraTradeHigh[0] = AvgEntryPrice;
				buysToday[0] = 1;//多头开仓标记
				return;
			}
			//{Thenextmodulekeepstrackofpositions && placesprotectivestops}以下是初始止损、保本止损、跟踪止损模块

			if (MarketPosition == 1)
			{
				longLiqPoint = EntryPrice - protStopPrcnt1 * averageRange[0];
				longLiqPoint = Min(longLiqPoint, EntryPrice - protStopAmt);
				if (tsMarketPosition1[0] == -1 && BarsSinceEntry == 1 && High[1] >= shortLiqPoint && shortLiqPoint < shortFBOPoint)
				{
					currTrdType[0] = -2;//{wejustgotlongfromashortliqreversal}上一根bar空头止损反手开多
				}
				if (currTrdType[0] == -2)
				{
					longLiqPoint = EntryPrice - protStopPrcnt2 * averageRange[0];
					longLiqPoint = Min(longLiqPoint, EntryPrice - protStopAmt);
				}
				if (intraTradeHigh[0] >= EntryPrice + breakEvenPrcnt * averageRange[0]) longLiqPoint = EntryPrice;//{BreakEventrade}
				if (Time[0] >= initTradesEndTime / 100) longLiqPoint = Max(longLiqPoint, Lowest(Low, 3));//{Trailingstop}
				if (Time[0] < liqRevEndTime / 100 && sellsToday[0] == 0 && longLiqPoint != EntryPrice && BarsSinceEntry >= 4 && Low[0] <= longLiqPoint)
				{
					MyPrice = longLiqPoint;
					if (Open[0] < MyPrice) MyPrice = Open[0];
					MyPrice = MyPrice - OffSet * MinPoint;
					SellShort((int)Lots, MyPrice);
					intraTradeLow[0] = AvgEntryPrice;
					sellsToday[0] = 1;
					tsMarketPosition1[0] = 1;
					return;
				}
				else if (Low[0] <= longLiqPoint)
				{
					MyPrice = longLiqPoint;
					if (Open[0] < MyPrice) MyPrice = Open[0];
					MyPrice = MyPrice - OffSet * MinPoint;
					Sell((int)0, MyPrice);
					return;
				}
			}
			if (MarketPosition == -1)
			//begin
			{
				shortLiqPoint = EntryPrice + protStopPrcnt1 * averageRange[0];
				shortLiqPoint = Max(shortLiqPoint, EntryPrice + protStopAmt);
				if (tsMarketPosition1[0] == 1 && BarsSinceEntry == 1 && Low[1] <= longLiqPoint && longLiqPoint > longFBOPoint)
				{
					currTrdType[0] = 2;//{wejustgotlongfromashortliqreversal}
				}
				if (currTrdType[0] == 2)
				{
					//begin
					shortLiqPoint = EntryPrice + protStopPrcnt2 * averageRange[0];
					shortLiqPoint = Max(shortLiqPoint, EntryPrice + protStopAmt);
				}//end;

				if (intraTradeLow[0] <= EntryPrice - breakEvenPrcnt * averageRange[0]) shortLiqPoint = EntryPrice;//{BreakEventrade}
				if (Time[0] >= initTradesEndTime / 100) shortLiqPoint = Min(shortLiqPoint, Highest(High, 3));//{Trailingstop}
				if (Time[0] < liqRevEndTime / 100 && buysToday[0] == 0 && shortLiqPoint != EntryPrice && BarsSinceEntry >= 4 && High[0] >= shortLiqPoint)//then
				{//begin
					MyPrice = shortLiqPoint;
					if (Open[0] > MyPrice) MyPrice = Open[0];
					MyPrice = MyPrice + OffSet * MinPoint;
					Buy((int)Lots, MyPrice);
					intraTradeHigh[0] = AvgEntryPrice;
					buysToday[0] = 1;//多头开仓标记
					tsMarketPosition1[0] = -1;
					return;
				}//end
				else if (High[0] >= shortLiqPoint)//begin
				{
					MyPrice = shortLiqPoint;
					if (Open[0] > MyPrice) MyPrice = Open[0];
					MyPrice = MyPrice + OffSet * MinPoint;
					BuyToCover(0, MyPrice);
					return;
				}//end;
			}//end;
			//}
			//SetExitOnClose;
			//收盘平仓
			if (Time[0] >= ExitOnCloseMins / 100)
			{
				Sell((int)0, Open[0] - OffSet * MinPoint);
				BuyToCover(0, Open[0] + OffSet * MinPoint);
			}
			//End
		}
	}
}

