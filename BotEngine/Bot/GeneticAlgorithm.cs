using System.Collections.Generic;
using System.Linq;
using BotLib.Models;
using System;
using System.Threading;
using UtilsLib.Utils;
using Utils.Utils;

namespace BotEngine.Bot
{
    public class GeneticAlgorithm
    {
        private const float EvolutionTimeIntervalHours = 24;
        public const float MutationRate = 0.05F;
        private const float KillRate = 0.10F;

        int GenerationCounter = 0;

        private readonly int N;

        public GeneticAlgorithm(int NumberOfBots, TradingEngine botEngine)
        {
            N = NumberOfBots;
        }

        private void Evolution(/*ref Dictionary<int, Bot> botList*/) 
        {
            try
            {
                //Thread.Sleep(EvolutionTimeInterval);
                //parar todas as outras threads com wait
                //parar todas as outras threads com wait
                //for (int i = 0; i < _botEngine.botList.Count; i++)
                //{
                //    _botEngine.botList[i].Stop();
                //}

                //_botEngine.Started = false;
                List<Score> scoreList = BotDBContext.Execute(botContext => {
                    return botContext.GetScoresFromDB();
                });

                List<BotParameters> toKillParameters = new List<BotParameters>();
                List<Bot> toKill = new List<Bot>();

                List<Bot> toMutate = new List<Bot>();
                List<BotParameters> toMutateParameters = new List<BotParameters>();

                //sort by score/fitness
                //scoreList.Sort(new GFG());
                //int adjust = 0;
                ////get worst bots
                //for (int i = 0; i < scoreList.Count * KillRate; i++)
                //{
                //    BotParameters botParameters = _context.BotsParameters.Single(e => e.id == scoreList[i].BotParametersId);
                //    if (botParameters.IsARealBot)// in theory should not happen after many generations, this means the real bot was choosen to be killed
                //    {
                //        adjust = 1;//less one best bot
                //        continue;
                //    }
                //    toKillParameters.Add(botParameters);
                //    toKill.Add(_botEngine.botList[scoreList[i].BotParametersId]);
                //}

                ////get best bots
                //for (int i = scoreList.Count - 1; i >= scoreList.Count - (scoreList.Count * KillRate) + adjust; i--)
                //{
                //    toMutateParameters.Add(_context.BotsParameters.Single(e => e.id == scoreList[i].BotParametersId));
                //    toMutate.Add(_botEngine.botList[scoreList[i].BotParametersId]);
                //}

                ////kill worst bots
                //foreach (Bot bot in toKill)
                //{
                //    _botEngine.botList.Remove(bot.BotId);
                //}
                //_context.BotsParameters.RemoveRange(toKillParameters);

                ////generate babies from best bots
                //for (int i = 0; i < toMutate.Count; i++)
                //{
                //    Bot bot2 = Bot.GenerateMutatedBot(toMutate[i], toKill[i]);
                //    bot2.BotId = toKill[i].BotId;
                //    bot2.SaveBotParameters();
                //    _botEngine.botList.Add(bot2.BotId, bot2);
                //}

                ////InitFirst200Candles();

                ////for (int i = 0; i < _botEngine.botList.Count; i++)
                ////{
                ////    _botEngine.botList[i].InitMACD();
                ////    _botEngine.botList[i].InitMovingAverages();
                ////}

                ////restart bots
                //_botEngine.Started = true;

                //for (int i = 0; i < _botEngine.botList.Count; i++)
                //{
                //    _botEngine.botList[i].Process();
                //}
                foreach (var score in scoreList)
                {
                    score.AmountGainedDaily = 0.0F;
                }
                BotDBContext.Execute(botContext => {
                    botContext.Scores.UpdateRange(scoreList);
                    botContext.SaveChanges();
                    return 0;
                }, true);
                GenerationCounter++;
                Console.WriteLine("Evolution generation number : " + GenerationCounter);

            }
            catch (Exception e)
            {
                if (BotEngine.Verbose)
                    Console.WriteLine(e);
                //if (BotEngine.Logging)
                BotEngine.log.Error(e);
            }
        }


        public void Evolutions()
        {
            //TaskScheduler.Instance.ScheduleTaskByInitOfHour(-2, EvolutionTimeIntervalHours, 10, Evolution);
            MyTaskScheduler.Instance.ScheduleTaskDaily(Evolution , "BotEvolution", 0, 0, 10);
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public static int Mutate(int x, float mutationRate = MutationRate)
        {
            bool positive = RandomGenerator.RandomBoolean();
            if (positive)
            {
                return x + (int)(x * RandomGenerator.RandomDouble(0, mutationRate) * 2);
            }
            else
            {
                return x - (int)(x * RandomGenerator.RandomDouble(0, mutationRate) * 2);
            }
        }
        public static float Mutate(float x, float mutationRate = MutationRate)
        {
            bool positive = RandomGenerator.RandomBoolean();
            if (positive)
            {
                return x + (x * RandomGenerator.RandomDouble(0, mutationRate));
            }
            else
            {
                return x - (x * RandomGenerator.RandomDouble(0, mutationRate));
            }
        }
    }

}
