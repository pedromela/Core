using System;
using System.Collections.Generic;
using System.Text;

namespace BotEngine.BotStuff.Exceptions
{
    class CandleIdNotFoundException : Exception
    {
        public CandleIdNotFoundException()
        :base("CandleIdNotFoundException: Candle not found! Possibly an old transaction. Trying to use PriceId...")
        {

        }
    }
}
