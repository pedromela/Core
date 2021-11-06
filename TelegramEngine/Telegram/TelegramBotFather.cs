using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramEngine.Telegram
{
    public class TelegramBotFather
    {
        private TelegramBotClient _bot;
        private Task _main;
        public TelegramBotFather() 
        {
            _bot = new TelegramBotClient(Configuration.BotToken);
            //Start();
        }

        private  void StartReceiving()
        {
            try
            {
                _main = Main();
                _main.Start();

            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        public async Task Main()
        {
            try
            {
#if USE_PROXY
            var Proxy = new WebProxy(Configuration.Proxy.Host, Configuration.Proxy.Port) { UseDefaultCredentials = true };
            Bot = new TelegramBotClient(Configuration.BotToken, webProxy: Proxy);
#else
                _bot = new TelegramBotClient(Configuration.BotToken);
#endif

                var me = await _bot.GetMeAsync();
                Console.Title = me.Username;

                _bot.OnMessage += BotOnMessageReceived;
                _bot.OnMessageEdited += BotOnMessageReceived;
                _bot.OnReceiveError += BotOnReceiveError;
                _bot.StartReceiving(Array.Empty<UpdateType>());
                Console.WriteLine($"Start listening for @{me.Username}");

                Console.ReadLine();

                _bot.StopReceiving();
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        public void SendMessage(string message) 
        {
            try
            {
                Task<Message> res = _bot.SendTextMessageAsync("@m4d32tr4d3", message);
                res.Wait();
                TelegramEngine.DebugMessage(res.Result.Text);
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var message = messageEventArgs.Message;
                if (message == null || message.Type != MessageType.Text)
                    return;

                TelegramEngine.DebugMessage(message.Text);

            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e.StackTrace);
            }
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            try
            {
                Console.WriteLine("Received error: {0} — {1}",
                    receiveErrorEventArgs.ApiRequestException.ErrorCode,
                    receiveErrorEventArgs.ApiRequestException.Message
                );
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e.StackTrace);
            }
        }
    }
}
