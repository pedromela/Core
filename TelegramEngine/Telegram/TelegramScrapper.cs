﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelegramEngine.Telegram.Channels;
using TelegramLib.Models;
using UtilsLib.Utils;

namespace TelegramEngine.Telegram
{
    public class TelegramScrapper
    {
        private string _url = null;
        private Channel _channel = null;
        private LinkedList<TelegramTransaction> _transactions = null;
        public TelegramScrapper(Channel channel)
        {
            _url = channel._url;
            _transactions = new LinkedList<TelegramTransaction>();
            _channel = channel;
        }

        public TelegramTransaction ProcessFunction() 
        {
            try
            {
                TelegramEngine.DebugMessage("###################### PARSING " + _channel._name +"######################");

                var response = Request.Get(_url);

                if (string.IsNullOrEmpty(response))
                {
                    TelegramEngine.DebugMessage("###################### PARSING " + _channel._name + " END ######################");
                    return null;
                }

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var html = htmlDoc.DocumentNode.SelectNodes("/html");

                //var entrys1 = htmlDoc.DocumentNode.SelectNodes("/html/body/main/div/section");
                var entrys = htmlDoc.DocumentNode.SelectNodes("//div[@class='tgme_widget_message_text js-message_text']");

                //var entrys = htmlDoc.DocumentNode.SelectNodes("/html/body/main/div/section/div[@class='tgme_widget_message_bubble']/div[@class='tgme_widget_message_text js-message_text']");

                if (entrys == null)
                {
                    TelegramEngine.DebugMessage("###################### PARSING " + _channel._name + " END ######################");
                    return null;
                }

                for (int i = entrys.Count; i > 0; i--)
                {
                    TelegramTransaction last = GetLastTransaction();
                    TelegramTransaction result = _channel.Parse(RemoveUnwantedTags(entrys[i - 1].InnerHtml));
                    if ((last == null || !last.IsEqual(result)) && result != null)
                    {
                        TelegramEngine.DebugMessage(entrys[i - 1].InnerText);
                        _transactions.AddLast(result);
                        TelegramEngine.DebugMessage("###################### PARSING " + _channel._name + " END ######################");
                        return result;
                    }
                    else if (_transactions.Count > 0)
                    {
                        break;// after we have the first position we dont need to search backwards anymore
                    }
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e.StackTrace);
            }
            TelegramEngine.DebugMessage("###################### PARSING " + _channel._name + " END ######################");
            return null;
        }

        private TelegramTransaction GetLastTransaction() 
        {
            try
            {
                return _transactions.Count > 0 ? _transactions.ElementAt(_transactions.Count - 1) : null;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e.StackTrace);
            }
            return null;
        }

        internal static string RemoveUnwantedTags(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(data);

            var acceptableTags = new String[] { "br" };

            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
                {
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);

                }
            }

            return document.DocumentNode.InnerHtml;
        }
    }
}
