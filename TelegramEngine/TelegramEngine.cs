using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NLog;
using TelegramEngine.Telegram;
using TelegramLib.Models;
using UtilsLib.Utils;
using BotEngine.Bot;
using BotLib.Models;
using TelegramEngine.Telegram.Channels;
using BotEngine;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;

namespace TelegramEngine
{
    class TelegramEngine : TradingEngine
    {
        private readonly TelegramDBContext _telegramContext;

        public CFDBot _forexBot = null;
        public TelegramBotFather _botFather = null;
        public bool first = true;

        public TelegramEngine(TelegramDBContext telegramContext)
        : base()
        {
            _telegramContext = telegramContext;
            _botFather = new TelegramBotFather();
            Run();
        }

        public TelegramEngine()
        : base()
        {
            _telegramContext = TelegramDBContext.newDBContext();
            _botFather = new TelegramBotFather();
            Run();
        }

        public override void Run()
        {
            try
            {
                AutoResetEvent autoEvent = new AutoResetEvent(false);

                Init();
                Started = true;
               // _bot = new TelegramBot();

                UpdateCycle();

                autoEvent.WaitOne();
            }
            catch (Exception e)
            {
                if (Verbose)
                    Console.WriteLine(e);
                if (Logging)
                    log.Error(e);
            }
        }

        public new void Init()
        {
            try
            {
                List<TelegramParameters> botsParametersList = _telegramContext.GetBotsFromDB();

                if (botsParametersList.Count == Channel.MAX)
                {
                    LoadTelegramBots(botsParametersList);
                }
                else if (botsParametersList.Count < Channel.MAX && botsParametersList.Count > 0)
                {
                    LoadTelegramBots(botsParametersList);
                    GenerateAndLoadTelegramBots(Channel.MAX - botsParametersList.Count);
                }
                else
                {
                    GenerateAndLoadTelegramBots(Channel.MAX);
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        private void LoadTelegramBots(List<TelegramParameters> botsParametersList) 
        {
            try
            {
                foreach (TelegramParameters telegramParameters in botsParametersList)
                {
                    if (!_botDict.ContainsKey(telegramParameters.TimeFrame))
                    {
                        _botDict.Add(telegramParameters.TimeFrame, new Dictionary<string, BotBase>());
                    }
                    _botDict[telegramParameters.TimeFrame].Add(telegramParameters.id, TelegramBot.GenerateTelegramBotFromParameters(telegramParameters));
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        private void GenerateAndLoadTelegramBots(int count) 
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    TelegramBot bot = TelegramBot.GenerateRandomTelegramBot(-1, i);
                    bot.SaveBotParameters();
                    if (!_botDict.ContainsKey(bot._botParameters.TimeFrame))
                    {
                        _botDict.Add(bot._botParameters.TimeFrame, new Dictionary<string, BotBase>());
                    }

                    _botDict[bot._botParameters.TimeFrame].Add(bot._botParameters.id, bot);
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        public override void UpdateCycle()
        {
            DebugMessage("############################################################");
            DebugMessage("Starting Telegram Bots...");
            MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<TelegramEngine>(Update, "TelegramBotsUpdate", this, 1, 1, 0);
            DebugMessage("############################################################");
        }
        private static void Update(TelegramEngine telegramEngine) 
        {
            try
            {
                if (telegramEngine.Started)
                {
                    List<TelegramTransaction> transactions = new List<TelegramTransaction>();

                    foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                    {
                        if (!telegramEngine._botDict.ContainsKey(timeFrame))
                        {
                            continue;
                        }

                        foreach (var bot in telegramEngine._botDict[timeFrame].Values)
                        {
                            if (bot is TelegramBot)
                            {
                                TelegramBot telegramBot = (TelegramBot)bot;
                                TelegramTransaction t = telegramBot.ProcessNewTelegramTransaction();
                                if (t != null)
                                {
                                    transactions.Add(t);
                                }
                            }
                        }
                    }

                    DebugMessage(transactions.Count + " positions to process.");

                    if (telegramEngine.first)
                    {
                        telegramEngine.first = false;
                        foreach (var t in transactions)
                        {
                            string order = t.ParseBeautify();
                            DebugMessage(order);
                        }
                        return;
                    }

                    foreach (var t in transactions)
                    {
                        string order = t.ParseBeautify();
                        DebugMessage(order);
                        telegramEngine._botFather.SendMessage(order);
                    }
                }
            }
            catch (Exception e)
            {

                TelegramEngine.DebugMessage(e);
            }
        }
    }
}
