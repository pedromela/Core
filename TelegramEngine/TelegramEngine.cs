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
using BotEngine;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;
using SignalsEngine.Indicators;
using System.Threading.Tasks;

namespace TelegramEngine
{
    class TelegramEngine : TradingEngine
    {
        private readonly TelegramDBContext _telegramContext;

        public CFDBot _forexBot = null;
        public TelegramBotFather _botFather = null;
        public bool first = true;
        public const TimeFrames defaultTimeFrame = TimeFrames.M5;
        public string[] channelUrls = new string[] 
        {
            "https://t.me/s/M15_SIGNALS_LLC",
            //"https://t.me/s/easyforexpips",
            //"https://t.me/s/Forexsignalsstreet",
            //"https://t.me/s/freesignalsfxnl",
            //"https://t.me/s/Free_Forex_Signals_ElevatingFX",
            //"https://t.me/s/free_forex_signals_fxlifestyle",
            //"https://t.me/s/TFXC_FREE",
            //"https://t.me/s/millionmoneyfx"
        };

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
                BotDBContext.InitProviders();
                BrokerDBContext.InitProviders();
                IndicatorsSharedData.InitInstance();
                LoadTelegramBots();
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        private void LoadTelegramBots(bool backtest = false) 
        {
            try
            {
                foreach (var channelUrl in channelUrls)
                {
                    if (!_botDict.ContainsKey(defaultTimeFrame))
                    {
                        _botDict.Add(defaultTimeFrame, new Dictionary<string, BotBase>());
                    }
                    _botDict[defaultTimeFrame].Add(channelUrl, TelegramBot.GenerateTelegramBotFromChannelUrl(channelUrl, defaultTimeFrame, backtest));
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
            MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<TelegramEngine>(Update, "TelegramBotsUpdate", this, (int) defaultTimeFrame, 1, 0);
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
                                //Task.Run(() => {
                                    TelegramBot telegramBot = (TelegramBot)bot;
                                    TelegramTransaction t = telegramBot.ProcessNewTelegramTransaction();
                                    if (t != null && t.IsConsistent())
                                    {
                                        transactions.Add(t);
                                    }
                                //});
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
                        if (order == null)
                        {
                            continue;
                        }
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
