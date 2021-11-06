using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalsEngine.Conditions;
using UtilsLib.Utils;

namespace UnitTestsCore
{
    [TestClass]
    public class UnitTestTextCondition
    {
        [TestMethod]
        public void TestVWAPMACondition()
        {
            //TextCondition textCondition = new TextCondition();
            string _expression = "3 > 2";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }
    }
}
