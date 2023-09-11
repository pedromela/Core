using BotLib.Models;
using System;

namespace TelegramLib.Models
{
    public class TelegramScore : Score
    {
        public TelegramScore() 
        : base()
        {

        }

        public override void Store()
        {
            try
            {
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    telegramContext.TelegramScores.Add(this);
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
                    telegramContext.TelegramScores.Update(this);
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
