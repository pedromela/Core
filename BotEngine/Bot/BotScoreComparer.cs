using BotLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotEngine.Bot
{
    public class BotScoreComparer : IComparer<Score>
    {
        public int Compare(Score x, Score y)
        {
            float scorex = x.CalculateFinalScore();//(x.Positions > 0 ? x.Successes / x.Positions : 0)  + ;
            float scorey = y.CalculateFinalScore();//(y.Positions > 0 ? y.Successes / y.Positions : 0) + ;

            return scorey.CompareTo(scorex);

        }
    }
}
