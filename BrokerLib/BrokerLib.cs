using NLog;
using System;
using UtilsLib.Utils;

namespace BrokerLib
{
    public class BrokerLib : MyLogger
    {
        public const float FEE = 0.002f;
        public const float SPREAD = 0.0001f;

        public enum Brokers
        {
            HitBTC,
            OANDA,
            //Bitstamp,
            //Poloniex,
            //Kraken,
        }
        public enum TimeFrames
        {
            M1 = 1,
            M5 = 5,
            M15 = 15,
            M30 = 30,
            H1 = 60
        }

        public enum BrokerType
        {
            exchange,
            margin,
            exchange_dev,
            margin_dev
        }

        public enum TransactionType
        {
            buy,
            buyclose,
            buydone,
            buylimit,
            sell,
            sellclose,
            selldone,
            selllimit,
            smartsell,
            smartbuy
        }

        public enum MarketTypes
        {
            Crypto = 1,
            Forex = 2,
            Stocks = 3
        }
        public static TransactionType OppositeTransactionType(TransactionType transactionType)
        {
            try
            {
                if (transactionType == TransactionType.buy)
                {
                    return TransactionType.sell;
                }
                else if (transactionType == TransactionType.sell)
                {
                    return TransactionType.buy;
                }
                else if (transactionType == TransactionType.buyclose)
                {
                    return TransactionType.sellclose;
                }
                else if (transactionType == TransactionType.sellclose)
                {
                    return TransactionType.buyclose;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return transactionType;
        }

        public static TransactionType CloseTransactionType(TransactionType transactionType) 
        {
            try
            {
                if (transactionType == TransactionType.buy)
                {
                    return TransactionType.buyclose;
                }
                else if (transactionType == TransactionType.sell)
                {
                    return TransactionType.sellclose;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return transactionType;
        }

        public static TransactionType DoneTransactionType(TransactionType transactionType)
        {
            try
            {
                if (transactionType == TransactionType.buy)
                {
                    return TransactionType.buydone;
                }
                else if (transactionType == TransactionType.sell)
                {
                    return TransactionType.selldone;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return transactionType;
        }
    }
}
