using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramEngine.Telegram
{
    public static class Configuration
    {
        public readonly static string BotToken = "1454417789:AAHvIsLBMCyv-DEBeHuQxaMX_OI1IxDTdV8";

#if USE_PROXY
        public static class Proxy
        {
            public readonly static string Host = "{PROXY_ADDRESS}";
            public readonly static int Port = 8080;
        }
#endif
    }
}
