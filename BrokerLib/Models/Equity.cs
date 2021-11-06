using BrokerLib.Brokers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Models
{
    public class Equity : DBModelBase
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public float Amount { get; set; }
        [Column(TypeName = "money")]
        public float RealAvailableAmountSymbol2 { get; set; }
        [Column(TypeName = "money")]
        public float RealAvailableAmountSymbol1 { get; set; }
        [Column(TypeName = "money")]
        public float EquityValue { get; set; }
        [Column(TypeName = "money")]
        public float VirtualBalance { get; set; } // Amount to Balance
        [Column(TypeName = "money")]
        public float VirtualNAV{ get; set; } //rename EquityValue to NAV

        public Equity()
        : base(BrokerDBContext.providers)
        {
        }

        public static Equity Initialize(Broker broker, AccessPoint ap, string market) 
        {
            try
            {
                float lastClose = broker.GetLastCandle(market, TimeFrames.M1).Close;
                float balance = broker.GetTotalMarketBalance(ap, market, lastClose);
                Equity equity = new Equity();
                equity.Amount = balance;
                equity.VirtualBalance = balance;
                equity.EquityValue = balance;
                equity.VirtualNAV = balance;
                equity.RealAvailableAmountSymbol1 = broker.GetCurrencyBalance(ap, market, lastClose);
                equity.RealAvailableAmountSymbol2 = broker.GetCurrencyBalance(ap, market, lastClose, true);
                equity.Name = ap.Name;
                equity.id = Guid.NewGuid().ToString();
                equity.Store();
                return equity;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public override void Store()
        {
            try
            {
                id = Guid.NewGuid().ToString();
                base.Update();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }
    }
}
