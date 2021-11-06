using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BrokerLib.BrokerLib;

namespace BotLib.Models
{
    public class SmartTransaction
    {
        public float PriceAtCreation { get; set; }
        public float TakeProfitPrice { get; set; }
        public float Quantity { get; set; }
        public TransactionType Type { get; set; }

        public Transaction BuyTransaction { get; set; }
        public float Count = 0;

        public SmartTransaction(float _PriceAtCreation, Transaction _BuyTransaction, TransactionType _Type, float _Quantity = 0.0001F)
        {
            PriceAtCreation = _PriceAtCreation;
            BuyTransaction = _BuyTransaction;
            Type = _Type;
            Count = 0;
            Quantity = _Quantity;
        }

    }
}
