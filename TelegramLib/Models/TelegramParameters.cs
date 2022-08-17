using BotLib.Models;
using System;

namespace TelegramLib.Models
{
    public class TelegramParameters : BotParameters
    {

        public TelegramParameters() 
        : base()
        {

        }

        public TelegramParameters(BotParameters botParameters)
        : base(botParameters)
        {

        }

        public override void Store()
        {
            try
            {
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    id = Guid.NewGuid().ToString();
                    telegramContext.TelegramParameters.Add(this);
                    telegramContext.SaveChanges();
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
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    telegramContext.TelegramParameters.Update(this);
                    telegramContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
        }
    }
}
