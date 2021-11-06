using BotLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramLib.Models
{
    public class TelegramUserBotRelation : UserBotRelation
    {
        public TelegramUserBotRelation() 
        : base()
        {

        }

        public override void Store()
        {
            try
            {
                using (TelegramDBContext botContext = TelegramDBContext.newDBContext())
                {
                    botContext.TelegramUserBotRelations.Add(this);
                    botContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
        }

        public override void Update()
        {
            try
            {
                using (TelegramDBContext botContext = TelegramDBContext.newDBContext())
                {
                    botContext.TelegramUserBotRelations.Update(this);
                    botContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
        }
    }
}
