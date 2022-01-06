using BotLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;

namespace BotEngine.Bot
{
    public class CFDBot : BotBase
    {
        public CFDBot(BotParameters botParameters, bool backtest = false)
        : base(botParameters, backtest)
        {

        }

        public override int ProcessCloseTransactions(Candle lastCandle, IEnumerable<Transaction> sellTransactions, IEnumerable<Transaction> buyTransactions)
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

        public void ProcessBuyTransactions(Candle lastCandle, IEnumerable<Transaction> sellTransactions, IEnumerable<Transaction> buyTransactions)
        {
            try
            {
                buyFitness = CalculateBuyFitness();
                if (buyFitness > FitnessLimit)
                {
                    if (_botParameters.BotName.Equals("macdcross-ada"))
                    {
                        Console.WriteLine("macdcross-ada DEBUG");
                    }
                    if (buyFitness < 1.0f)
                    {
                        buyFitness = 1.0f;
                    }

                    buyFitness = MathF.Round(buyFitness);
                    if (!_botParameters.QuickReversal && !_botParameters.SuperReversal) {
                        if (sellTransactions.Any())
                        {
                            Transaction t = sellTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        else
                        {
                            StoreOrderTransaction(lastCandle, TransactionType.buy, MinimumTransactionAmount * buyFitness);
                        }
                    }
                    else if (_botParameters.QuickReversal) {
                        if (sellTransactions.Any())
                        {
                            Transaction t = sellTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        StoreOrderTransaction(lastCandle, TransactionType.buy, MinimumTransactionAmount * buyFitness);
                    }
                    else if (_botParameters.SuperReversal) {
                        if (sellTransactions.Any())
                        {
                            int max = sellTransactions.Count();
                            foreach (Transaction transaction in sellTransactions)
                            {
                                //StoreSellOrderTransaction(t, lastCandle);
                                CloseTrades(transaction);
                            }
                            for (int i = 0; i < max; i++)
                            {
                                StoreOrderTransaction(lastCandle, TransactionType.buy, MinimumTransactionAmount * buyFitness);
                            }
                        }
                    }
                }
                else if (buyFitness < -FitnessLimit)
                {
                    if (_botParameters.BotName.Equals("macdcross-ada"))
                    {
                        Console.WriteLine("macdcross-ada DEBUG");
                    }
                    buyFitness = -buyFitness;
                    if (buyFitness < 1.0f)
                    {
                        buyFitness = 1.0f;
                    }

                    buyFitness = MathF.Round(buyFitness);

                    if (!_botParameters.QuickReversal && !_botParameters.SuperReversal)
                    {
                        if (buyTransactions.Any())
                        {
                            Transaction t = buyTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        else
                        {
                            StoreOrderTransaction(lastCandle, TransactionType.sell, MinimumTransactionAmount * buyFitness);

                        }
                    }
                    else if (_botParameters.QuickReversal)
                    {
                        if (buyTransactions.Any())
                        {
                            Transaction t = buyTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        StoreOrderTransaction(lastCandle, TransactionType.sell, MinimumTransactionAmount * buyFitness);
                    }
                    else if (_botParameters.SuperReversal)
                    {
                        if (buyTransactions.Any())
                        {
                            int max = sellTransactions.Count();
                            foreach (Transaction transaction in buyTransactions)
                            {
                                //StoreSellOrderTransaction(t, lastCandle);
                                CloseTrades(transaction);
                            }
                            for (int i = 0; i < max; i++)
                            {
                                StoreOrderTransaction(lastCandle, TransactionType.sell, MinimumTransactionAmount * buyFitness);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
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
                else if (fitnessBuy < fitnessSell)
                {
                    return -fitnessSell;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// OVERRIDE METHODS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public override bool IsTransactionBuyTypes(TransactionType type)
        {
            return type == TransactionType.buy || type == TransactionType.sell;
        }

        public override bool IsTransactionSellTypes(TransactionType type)
        {
            return type == TransactionType.buyclose || type == TransactionType.sellclose;
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

        public override void ProcessTransactions(bool processBuyTypesTransactions = true)
        {
            try
            {
                if (Keepgoin)
                {
                    ProcessErrors();

                    Candle lastCandle = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);

                    IEnumerable<Transaction> sellTransactions = GetTransactionsByType(TransactionType.sell);
                    IEnumerable<Transaction> buyTransactions = GetTransactionsByType(TransactionType.buy);

                    if (ProcessCloseTransactions(lastCandle, sellTransactions, buyTransactions) == 0 && processBuyTypesTransactions)
                    {
                        ProcessBuyTransactions(lastCandle, sellTransactions, buyTransactions);
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
    }
}
