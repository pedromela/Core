using BotLib.Models;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace BotEngine.Bot
{
    public class ExchangeBot : BotBase
    {
        public ExchangeBot(BotParameters botParameters, bool backtest = false)
        : base(botParameters, backtest)
        {

        }

        public override bool IsTransactionBuyTypes(TransactionType type)
        {
            return type == TransactionType.buy;
        }

        public override bool IsTransactionSellTypes(TransactionType type)
        {
            return type == TransactionType.buyclose;
        }

        public override TransactionType GetTransactionLongType()
        {
            return TransactionType.buy;
        }

        public override TransactionType GetTransactionShortType()
        {
            return TransactionType.sell;
        }

        public override TransactionType GetTransactionLongCloseType()
        {
            return TransactionType.buyclose;
        }

        public override TransactionType GetTransactionShortCloseType()
        {
            return TransactionType.sellclose;
        }

        public override void ProcessTransactions()
        {
            try
            {
                if (Keepgoin)
                {
                    ProcessErrors();

                    Candle lastCandle = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);

                    IEnumerable<Transaction> buyTransactions = GetTransactionsByType(TransactionType.buy);
                    IEnumerable<Transaction> sellTransactions = new List<Transaction>();

                    buyTransactions = buyTransactions != null ? buyTransactions : new List<Transaction>();

                    if (ProcessCloseTransactions(lastCandle, buyTransactions, sellTransactions) == 0)
                    {
                        ProcessTransactions(lastCandle, buyTransactions, sellTransactions);
                    }

                    //FIXME
                    //if (_botParameters.SmartBuyTransactions)
                    //{
                    //    Candle lastCandle = _signalsEngine.indicatorsEngine.GetCurrentCandle(_botParameters.TimeFrame);
                    //    ProcessSmartBuyTransactions(lastCandle);
                    //}
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }
        public override int ProcessCloseTransactions(Candle lastCandle, IEnumerable<Transaction> buyTransactions, IEnumerable<Transaction> sellTransactions)
        {
            try
            {
                int countSells = 0;
                CurrentProfits currentProfits = GetAllProfits(lastCandle, buyTransactions, ref countSells);

                StoreScore(true, 0.0f, true, currentProfits);

                return countSells;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public override float CalculateBuyFitness()
        {
            try
            {
                return CalculateBuyFitness(TransactionType.buy);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }
    }
}
