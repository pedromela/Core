using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace UnitTestsCore
{
    public class TestUtils
    {
		public static List<Candle> GetCandles(TimeFrames timeFrame)
		{
			try
			{
				List<Candle> candles = new List<Candle>();
				DateTime timestamp = DateTime.MinValue;
				string market = "BTCUSD";
				Candle candle = null; 
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10700, 10800, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10600, 10700, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10670, 10600, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10800, 10700, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10900, 10860, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10950, 10890, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10910, 10880, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10890, 10840, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10840, 10890, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10850, 10860, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10700, 10800, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10600, 10690, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10500, 10600, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10300, 10400, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10200, 10350, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10100, 10200, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 10000, 11000, 10000, 10100, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 9900, 10000, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10100, 9950, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10160, 10110, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10100, 10150, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10200, 10100, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10270, 10220, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10350, 10260, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10500, 10400, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10550, 10500, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10450, 10500, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10400, 10450, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10500, 10400, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10550, 104700, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10700, 10560, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10600, 10650, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10500, 10580, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10450, 10500, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10300, 10400, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10300, 10340, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10200, 10290, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10010, 10080, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10120, 10100, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10160, 10110, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10100, 10150, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10200, 10100, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10270, 10220, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10350, 10260, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10450, 10350, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10500, 10450, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10600, 10560, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10700, 10620, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10810, 10700, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10750, 10800, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10850, 10750, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11000, 10930, 10840, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11020, 11020, 10910, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11100, 11100, 10980, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11150, 11150, 11100, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11300, 11300, 11200, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11300, 11260, 11300, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11300, 11210, 11260, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11300, 11110, 11210, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11300, 11010, 11110, 200, 2000000);
				candle = new Candle(timeFrame, market, timestamp, 9900, 11300, 11060, 11010, 200, 2000000);
			}
			catch (Exception e)
			{
				UtilsLib.UtilsLib.DebugMessage(e);
			}
			return null;
		}
	}
}
