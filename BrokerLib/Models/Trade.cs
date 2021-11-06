using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Models
{
    public class Trade : DBModelBase //Trade, associated with a specific person
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string BuyTradeId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string TransactionId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string BrokerTransactionId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string AccessPointId { get; set; }
        [Column(TypeName = "float")]
        public float Amount { get; set; }
        [Column(TypeName = "float")]
        public float Price { get; set; }
        [Column(TypeName = "nvarchar(10)")]
        public string Market { get; set; }
        [Column(TypeName = "bigint")]
        public int Leverage { get; set; }
        [Column(TypeName = "bigint")]
        public TransactionType Type { get; set; }
        [Column(TypeName = "float")]
        public float Profit { get; set; }
        public Trade()
        : base(BrokerDBContext.providers)
        {

        }

        public Trade(string AccessPointId, string TransactionId, string BrokerTransactionId, float Amount, float Price, string Market, TransactionType Type, string BuyTradeId = null)
        : base(BrokerDBContext.providers)
        {
            this.AccessPointId = AccessPointId;
            this.TransactionId = TransactionId;
            this.BrokerTransactionId = BrokerTransactionId;
            this.Amount = Amount;
            this.Price = Price;
            this.Market = Market;
            this.Type = Type;
            this.BuyTradeId = BuyTradeId;
        }

        public override void Store()
        {
            id = Guid.NewGuid().ToString();
            base.Store();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
