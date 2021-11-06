using BotLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotEngine.Bot
{
    public class BotRanking
    {
        private List<BotParametersRanking> _rankingBots = null;

        public BotRanking() 
        {
            _rankingBots = new List<BotParametersRanking>();
        }

        public void CalculateBestRankings()
        {
            try
            {
                if (_rankingBots.Count > 0)
                {
                    _rankingBots.Clear();
                }
                using (BotDBContext botContext = BotDBContext.newDBContext())
                {
                    List<BotParameters> botParametersList = botContext.GetBotsFromDB();
                    List<BotParametersRanking> botRankingsList = botContext.GetBotRankingsFromDB();

                    Dictionary<string, BotParameters> dictBots = new Dictionary<string, BotParameters>();
                    List<Score> scoreList = botContext.GetScoresFromDB();
                    List<Bot> toKill = new List<Bot>();
                    List<BotParameters> auxbotParametersList = new List<BotParameters>();

                    foreach (BotParameters botParameters in botParametersList)
                    {
                        dictBots.Add(botParameters.id, botParameters);
                    }

                    //sort by score/fitness
                    scoreList.Sort(new BotScoreComparer());
                    int MaxBotCount = botParametersList.Count;

                    for (int i = 0; i < MaxBotCount && i < scoreList.Count; i++)
                    {
                        if (dictBots.ContainsKey(scoreList[i].BotParametersId))
                        {
                            auxbotParametersList.Add(dictBots[scoreList[i].BotParametersId]);
                        }
                    }
                    for (int i = 0; i < auxbotParametersList.Count; i++)
                    {
                        _rankingBots.Add(new BotParametersRanking(i + 1, auxbotParametersList[i].id));
                    }
                    botContext.SaveBotRankings(_rankingBots);
                    using (BotDBContext botContextClient = BotDBContext.newDBContextClient())
                    {
                        botContextClient.SaveBotRankings(_rankingBots);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public BotParameters KillWorstBot() 
        {
            try
            {
                using (BotDBContext botContext = BotDBContext.newDBContext())
                {
                    List<BotParametersRanking> botRankingsList = botContext.GetBotRankingsFromDB();
                    BotParametersRanking botParametersRanking = botRankingsList.Last();
                    BotParameters botParameters = botContext.BotsParameters.Find(botParametersRanking.BotId);
                    botParameters.Delete();
                    return botParameters;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }
    }
}
