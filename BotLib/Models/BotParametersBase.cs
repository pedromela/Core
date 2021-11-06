using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BotLib.Models
{
    public class BotParametersBase : DBModel
    {
        [Key]
        public int id { get; set; }
        [Column(TypeName = "bigint")]
        public int MutatedBotId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string BotName { get; set; }
        [Column(TypeName = "bigint")]
        public TimeFrames TimeFrame { get; set; }
        [Column(TypeName = "float")]
        public float Decrease { get; set; }
        [Column(TypeName = "float")]
        public float Increase { get; set; }
        [Column(TypeName = "int")]
        public bool SmartBuyTransactions { get; set; }
        [Column(TypeName = "int")]
        public bool SmartSellTransactions { get; set; }
        [Column(TypeName = "int")]
        public bool StopLoss { get; set; }
        [Column(TypeName = "int")]
        public bool TakeProfit { get; set; }
        [Column(TypeName = "int")]
        public bool TrailingStop { get; set; }
        [Column(TypeName = "int")]
        public bool LockProfits { get; set; }
        [Column(TypeName = "float")]
        public float UpPercentage { get; set; }
        [Column(TypeName = "float")]
        public float DownPercentage { get; set; }
        [Column(TypeName = "int")]
        public int minSmartBuyTransactions { get; set; }
        [Column(TypeName = "int")]
        public int minSmartSellTransactions { get; set; }
        public int InitLastProfitablePrice { get; set; }
        [Column(TypeName = "int")]
        public int StopAfterStopLossMinutes { get; set; }
        [Column(TypeName = "int")]
        public int StopLossMaxAtemptsBeforeStopBuying { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Market { get; set; }
        [Column(TypeName = "bigint")]
        public Brokers BrokerId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Channel { get; set; }
        [Column(TypeName = "bigint")]
        public int StrategyId { get; set; }

        public override void Store()
        {
            try
            {

            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
        }

        public override void Update()
        {
            try
            {

            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
        }

        public override void Delete()
        {
            try
            {

            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
        }
    }
}
