﻿using BotLib.Models;
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
                using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
                {
                    backtesterContext.BacktesterScores.Add(this);
                    backtesterContext.SaveChanges();
                }
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
                using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
                {
                    backtesterContext.BacktesterScores.Update(this);
                    backtesterContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                BacktesterLib.DebugMessage(e);
            }
        }
    }
}
