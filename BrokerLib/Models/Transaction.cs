using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Models
{
    public class Transaction : DBModelBase
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string BotId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string BuyId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string TelegramTransactionId { get; set; }
        [Column(TypeName = "float")]
        public float Amount { get; set; }
        [Column(TypeName = "float")]
        public float AmountSymbol2 { get; set; }
        [Column(TypeName = "bigint")]
        public TransactionType Type { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Market { get; set; }
        [Column(TypeName = "float")]
        public float LastProfitablePrice { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string States { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [Column(TypeName = "float")]
        public float Price { get; set; }
        [Column(TypeName = "float")]
        public float StopLoss { get; set; }
        [Column(TypeName = "float")]
        public float TakeProfit { get; set; }

        public Transaction()
        : base(BrokerDBContext.providers)
        {

        }

        public Transaction(Transaction t)
        : base(BrokerDBContext.providers)
        {
            BotId = t.BotId;
            BuyId = t.BuyId;
            Type = t.Type;
            TakeProfit = t.TakeProfit;
            StopLoss = t.StopLoss;
            Market = t.Market;
            Price = t.Price;
            Timestamp = t.Timestamp;
            Amount = t.Amount;
            AmountSymbol2 = t.AmountSymbol2;
            LastProfitablePrice = t.LastProfitablePrice;
            TelegramTransactionId = t.TelegramTransactionId;
        }

        public override void Store()
        {
            if (string.IsNullOrEmpty(Market))
            {
                return;
            }
            if (string.IsNullOrEmpty(BotId))
            {
                return;
            }
            id = Guid.NewGuid().ToString();
            base.Store();
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
