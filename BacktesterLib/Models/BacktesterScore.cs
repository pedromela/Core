using BotLib.Models;
using System;

namespace BacktesterLib.Models
{
    public class BacktesterScore : Score
    {
        public BacktesterScore()
        : base()
        {

        }

        public override void Store()
        {
            try
            {
                BacktesterDBContext.Execute((backtesterContext) => {
                    backtesterContext.BacktesterScores.Add(this);
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
                    backtesterContext.BacktesterScores.Update(this);
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
