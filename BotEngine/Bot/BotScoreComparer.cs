using BotLib.Models;
using System.Collections.Generic;

namespace BotEngine.Bot
{
    public class BotScoreComparer : IComparer<Score>
    {
        public int Compare(Score x, Score y)
        {
            float scorex = x.CalculateFinalScore();
            float scorey = y.CalculateFinalScore();

            return scorey.CompareTo(scorex);
        }
    }
}
