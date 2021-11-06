
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utils.Utils;
using UtilsLib.Utils;
using System.Collections.Generic;
using System.Linq;

namespace BotLib.Models
{
    public class ConditionStrategyData : DBModelBase
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string BuyCondition { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string SellCondition { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string BuyCloseCondition { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string SellCloseCondition { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        public ConditionStrategyData()
        : base(BotDBContext.providers)
        {

        }

        public bool Valid()
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(BuyCondition) && string.IsNullOrEmpty(SellCondition))
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }

            return false;
        }

        public override void Store()
        {
            try
            {
                id = Guid.NewGuid().ToString();
                base.Store();
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
        }

        public static string GetRandomStrategyId() 
        {
            try
            {
                int count = -1;
                List<ConditionStrategyData> strategies = null;
                using (BotDBContext botContext = BotDBContext.newDBContext())
                {
                    strategies = botContext.GetStrategiesFromDB();
                }
                count = strategies.Count;
                int idx = RandomGenerator.RandomNumber(0, count);
                return strategies[idx].id;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
            return null;
        }
    }
}
