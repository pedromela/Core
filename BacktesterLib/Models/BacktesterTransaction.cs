using BrokerLib.Models;
using System;

namespace BacktesterLib.Models
{
    public class BacktesterTransaction : Transaction
    {
        public BacktesterTransaction()
        : base()
        {
        }

        public BacktesterTransaction(BacktesterTransaction t)
        : base(t)
        {
        }

        public BacktesterTransaction(Transaction t)
        : base(t)
        {
        }

        public override void Store()
        {
            try
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
                BacktesterDBContext.Execute((backtesterContext) => {
                    backtesterContext.BacktesterTransactions.Add(this);
                    return backtesterContext.SaveChanges();
                }, true);
            }
            catch (Exception e)
            {
                BacktesterLib.DebugMessage(e);
            }
        }

        public override void Update()
        {
            try
            {
                BacktesterDBContext.Execute((backtesterContext) => {
                    //backtesterContext.BacktesterTransactions.Update(this);
                    return backtesterContext.SaveChanges();
                }, true);
            }
            catch (Exception e)
            {
                BacktesterLib.DebugMessage(e);
            }
        }
    }
}
