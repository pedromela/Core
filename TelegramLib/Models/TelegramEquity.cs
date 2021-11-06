using BrokerLib.Models;
using System;

namespace TelegramLib.Models
{
    public class TelegramEquity : Equity
    {
        public TelegramEquity() 
        : base()
        {

        }

        public override void Store()
        {
            try
            {
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    telegramContext.TelegramEquities.Add(this);
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
                    telegramContext.TelegramEquities.Update(this);
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
