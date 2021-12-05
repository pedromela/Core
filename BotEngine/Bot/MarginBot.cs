using BotLib.Models;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace BotEngine.Bot
{
    public class MarginBot : BotBase
    {
        public MarginBot(BotParameters botParameters, bool backtest = false)
        : base(botParameters, backtest)
        {

        }

        public override bool IsTransactionBuyTypes(TransactionType type)
        {
            return type == GetTransactionLongType() || type == GetTransactionShortType();
        }

        public override bool IsTransactionSellTypes(TransactionType type)
        {
            return type == GetTransactionLongCloseType() || type == GetTransactionShortCloseType();
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

                    IEnumerable<Transaction> sellTransactions = GetTransactionsByType(TransactionType.sell);
                    IEnumerable<Transaction> buyTransactions = GetTransactionsByType(TransactionType.buy);

                    buyTransactions = buyTransactions != null ? buyTransactions : new List<Transaction>();
                    sellTransactions = sellTransactions != null ? sellTransactions : new List<Transaction>();

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
                CurrentProfits currentProfits = GetAllProfits(lastCandle, sellTransactions, ref countSells);
                currentProfits = currentProfits + GetAllProfits(lastCandle, buyTransactions, ref countSells);

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
                float fitnessBuy = CalculateBuyFitness(TransactionType.buy);
                float fitnessSell = CalculateBuyFitness(TransactionType.sell);
                float fitnessMax = Math.Max(fitnessBuy, fitnessSell);
                if (fitnessBuy > fitnessSell)
                {
                    return fitnessBuy;
                }
                else
                {
                    return -fitnessSell;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

    }
}
