using System;
using System.Collections.Generic;
using System.Text;
using TelegramLib.Models;
using Utils.Utils;

namespace TelegramEngine.Telegram.Channels
{
    public class Channel
    {
        public static readonly int MAX = 8;
        public string _name = null;
        public string _url = null;

        public Channel(string name, string url)
        {
            _name = name;
            _url = url;
        }

        public virtual TelegramTransaction Parse(string rawData)
        {
            try
            {

            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }
        public virtual int GetIndexOfAny(string str, string[] toks, ref int idx) 
        {
            try
            {
                int index = -1;
                for (int i = 0; i < toks.Length; i++)
                {
                    index = str.IndexOf(toks[i]);

                    if (index != -1)
                    {
                        idx = i;
                        break;
                    }
                }
                return index;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return -1;
        }
        public virtual string GetNumberAfterString(string str, string tok) 
        {
            try
            {
                int index = str.IndexOf(tok);
                
                if (index == -1)
                {
                    return "-1";
                }

                string number = "";
                str = str.Substring(index + tok.Length);
                foreach (var c in str)
                {
                    if (c == ',')
                    {
                        number += '.';
                    }
                    else if ((c >= '0' && c <= '9') || c == '.')
                    {
                        number += c;
                    }
                    else if (c == ' ')
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                return number;
            }
            catch (Exception e)
            {
                
                TelegramEngine.DebugMessage(e);
            }
            return "-1";
        }

        public virtual string GetNumberAfterStrings(string str, string[] tok, ref int index)
        {
            try
            {
                string number = "";
                for (int i = 0; i < tok.Length; i++)
                {
                    number = GetNumberAfterString(str, tok[i]);
                    if (number == "-1")
                    {
                        continue;
                    }
                    else
                    {
                        index = i;
                        break;
                    }
                }

                return number;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return "-1";
        }


        public static string RandomChannelURL()
        {
            try
            {
                int len = RandomGenerator.RandomNumber(0, 8);
                switch (len)
                {
                    case 0:
                        return Forexsignalzz.URL;
                    case 1:
                        return ITrendsFxFree.URL;
                    case 2:
                        return ForexSignalsStreet.URL;
                    case 3:
                        return Hypetrades.URL;
                    case 4:
                        return Fxthunder.URL;
                    case 5:
                        return FXNL.URL;
                    case 6:
                        return EasyForexPips.URL;
                    case 7:
                        return Youthtribe.URL;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public static string DecideChannelURL(int channelId)
        {
            try
            {
                switch (channelId)
                {
                    case 0:
                        return Forexsignalzz.URL;
                    case 1:
                        return ITrendsFxFree.URL;
                    case 2:
                        return ForexSignalsStreet.URL;
                    case 3:
                        return Hypetrades.URL;
                    case 4:
                        return Fxthunder.URL;
                    case 5:
                        return FXNL.URL;
                    case 6:
                        return EasyForexPips.URL;
                    case 7:
                        return Youthtribe.URL;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public static Channel RandomChannel()
        {
            try
            {
                int len = RandomGenerator.RandomNumber(0, 9);
                switch (len)
                {
                    case 0:
                        return new Forexsignalzz();
                    case 1:
                        return new ITrendsFxFree();
                    case 2:
                        return new ForexSignalsStreet();
                    case 3:
                        return new Hypetrades();
                    case 4:
                        return new Fxthunder();
                    case 5:
                        return new FXNL();
                    case 6:
                        return new EasyForexPips();
                    case 7:
                        return new Youthtribe();
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public static string DecideChannelName(string url)
        {
            try
            {
                Channel channel = DecideChannel(url);
                return channel._name;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public static Channel DecideChannel(string url)
        {
            try
            {
                if (url.Equals(Forexsignalzz.URL))
                {
                    return new Forexsignalzz();
                }
                else if (url.Equals(ITrendsFxFree.URL))
                {
                    return new ITrendsFxFree();
                }
                else if (url.Equals(ForexSignalsStreet.URL))
                {
                    return new ForexSignalsStreet();
                }
                else if (url.Equals(Hypetrades.URL))
                {
                    return new Hypetrades();
                }
                else if (url.Equals(Fxthunder.URL))
                {
                    return new Fxthunder();
                }
                else if (url.Equals(FXNL.URL))
                {
                    return new FXNL();
                }
                else if (url.Equals(EasyForexPips.URL))
                {
                    return new EasyForexPips();
                }
                else if (url.Equals(Youthtribe.URL))
                {
                    return new Youthtribe();
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }
    }
}
