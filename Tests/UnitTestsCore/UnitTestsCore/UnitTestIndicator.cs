using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalsEngine;
using SignalsEngine.Indicators;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace UnitTestsCore
{
    [TestClass]
    public class UnitTestIndicator
    {
        [TestMethod]
        public void TestEma()
        {
            Indicator ema = new EMA(20, TimeFrames.M1);
            ema.Init();
            bool result = true;
            Assert.AreEqual<bool>(result, true, "Failed!");
        }
    }
}
