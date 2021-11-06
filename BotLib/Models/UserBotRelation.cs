using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;

namespace BotLib.Models
{
    public class UserBotRelation : DBModelBase
    {
        [Key]
        [Column(Order = 1, TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 2, TypeName = "nvarchar(450)")]
        public string BotId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string AccessPointId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string EquityId { get; set; }
        [Column(TypeName = "int")]
        public bool IsVirtual { get; set; }
        [Column(TypeName = "float")]
        public float DefaultTransactionAmount { get; set; }

        public UserBotRelation()
        : base(BotDBContext.providers)
        {

        }

        public UserBotRelation(string botId, BotParametersChanges botParametersChanges)
        : base(BotDBContext.providers)
        {
            BotId                       = botId;
            UserId                      = botParametersChanges.UserId;
            AccessPointId               = botParametersChanges.AccessPointId;
            IsVirtual                   = botParametersChanges.IsVirtual;
            DefaultTransactionAmount    = botParametersChanges.DefaultTransactionAmount;
        }

        public string Validation() 
        {
            try
            {
                string error = "";
                if (string.IsNullOrEmpty(BotId))
                {
                    error += "BotId is 0." + Environment.NewLine;
                }
                if (!IsVirtual)
                {
                    if (string.IsNullOrEmpty(AccessPointId))
                    {
                        error += "Bot is not virtual and AccessPointId is 0." + Environment.NewLine;
                    }
                    //if (EquityId == 0)
                    //{
                    //    error += "Bot is not virtual and EquityId is 0." + Environment.NewLine;
                    //}
                    if (DefaultTransactionAmount <= 0)
                    {
                        error += "Bot is not virtual and DefaultTransactionAmount is 0." + Environment.NewLine;
                    }
                }
                if (string.IsNullOrEmpty(error))
                {
                    return "ok";
                }
                return error;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
            return null;
        }

        //public override void Store()
        //{
        //    try
        //    {
        //        using (BotDBContext botContext = BotDBContext.newDBContext())
        //        {
        //            botContext.UserBotRelations.Add(this);
        //            botContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BotLib.DebugMessage(e);
        //    }
        //}

        //public override void Update()
        //{
        //    try
        //    {
        //        using (BotDBContext botContext = BotDBContext.newDBContext())
        //        {
        //            botContext.UserBotRelations.Update(this);
        //            botContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BotLib.DebugMessage(e);
        //    }
        //}

        //public override void Delete()
        //{
        //    try
        //    {
        //        using (BotDBContext botContext = BotDBContext.newDBContext())
        //        {
        //            botContext.UserBotRelations.Remove(this);
        //            botContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BotLib.DebugMessage(e);
        //    }
        //}

    }
}
