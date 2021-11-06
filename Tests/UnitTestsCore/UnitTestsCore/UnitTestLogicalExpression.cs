using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UtilsLib.Utils;

namespace UnitTestsCore
{
    [TestClass]
    public class UnitTestLogicalExpression
    {
        [TestMethod]
        public void TestGreaterThan()
        {
            string _expression = "3 > 2";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestLesserThan()
        {
            string _expression = "2 < 3";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestAnd()
        {
            string _expression = "2 < 3 and 3 > 2";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestOr()
        {
            string _expression = "2 < 3 or 2 > 3";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestParenthesis()
        {
            string _expression = "( 2 < 3 and 2 > 3 ) or 3 > 2";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestVariablesGreaterThan()
        {
            string _expression = "a > b";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["a"].Set(3);
            _logicExpression["b"].Set(2);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestVariablesLesserThan()
        {
            string _expression = "a < b";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["a"].Set(2);
            _logicExpression["b"].Set(3);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestVariablesAnd()
        {
            string _expression = "a < b and b > a";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["a"].Set(2);
            _logicExpression["b"].Set(3);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestVariablesOr()
        {
            string _expression = "a < b or b > a";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["a"].Set(2);
            _logicExpression["b"].Set(3);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestSingleBooleanVariableTrue()
        {
            string _expression = "a";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["a"].Set(true);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestSingleBooleanVariableFalse()
        {
            string _expression = "a";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["a"].Set(false);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, false, "Failed!");
        }

        [TestMethod]
        public void TestVWAPMABuyConditionTrue()
        {
            string _expression = "i_price:200_middle < i_VWAP_middle and i_price:200_middle < i_SMA:200_middle";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(105);
            _logicExpression["i_SMA:200_middle"].Set(105);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        public void TestVWAPMABuyConditionFalse()
        {
            string _expression = "i_price:200_middle < i_VWAP_middle and i_price:200_middle < i_SMA:200_middle";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(95);
            _logicExpression["i_SMA:200_middle"].Set(105);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, false, "Failed!");
        }

        [TestMethod]
        public void TestVWAPMASellConditionTrue()
        {
            string _expression = "i_price:200_middle > i_VWAP_middle and i_price:200_middle > i_SMA:200_middle";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(95);
            _logicExpression["i_SMA:200_middle"].Set(95);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        public void TestVWAPMASellConditionFalse()
        {
            string _expression = "i_price:200_middle > i_VWAP_middle and i_price:200_middle > i_SMA:200_middle";
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(95);
            _logicExpression["i_SMA:200_middle"].Set(105);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, false, "Failed!");
        }

        [TestMethod]
        public void TestVWAPMAPercentageBuyConditionTrue()
        {
            float percentage = 0.05f;
            string _expression = String.Format("i_price:200_middle < ( i_VWAP_middle + i_SMA:200_middle ) * ( 1 - {0}) / 2", percentage);
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(110);
            _logicExpression["i_SMA:200_middle"].Set(110);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestVWAPMAPercentageBuyConditionFalse()
        {
            float percentage = 0.05f;
            string _expression = String.Format("i_price:200_middle < ( i_VWAP_middle + i_SMA:200_middle ) * ( 1 - {0}) / 2", percentage);
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(105);
            _logicExpression["i_SMA:200_middle"].Set(105);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, false, "Failed!");
        }

        [TestMethod]
        public void TestVWAPMAPercentageSellConditionTrue()
        {
            float percentage = 0.05f;
            string _expression = String.Format("i_price:200_middle > ( i_VWAP_middle + i_SMA:200_middle ) * ( 1 + {0}) / 2", percentage);
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(90);
            _logicExpression["i_SMA:200_middle"].Set(90);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, true, "Failed!");
        }

        [TestMethod]
        public void TestVWAPMAPercentageSellConditionFalse()
        {
            float percentage = 0.05f;
            string _expression = String.Format("i_price:200_middle > ( i_VWAP_middle + i_SMA:200_middle ) * ( 1 + {0}) / 2", percentage);
            LogicExpressionParser _logicExpressionParser = new LogicExpressionParser();
            LogicExpression _logicExpression = _logicExpressionParser.Parse(_expression);
            _logicExpression["i_price:200_middle"].Set(100);
            _logicExpression["i_VWAP_middle"].Set(95);
            _logicExpression["i_SMA:200_middle"].Set(95);
            bool result = _logicExpression.GetResult();
            Assert.AreEqual<bool>(result, false, "Failed!");
        }
    }
}
