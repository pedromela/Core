using Azure.AI.OpenAI;
using OpenAI;
using System;

namespace TelegramEngine.OpenAI
{
    public class OpenAITradingSignalParser
    {
        private OpenAIClient _client;
        public OpenAITradingSignalParser()
        {
            _client = new OpenAIClient("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");
        }

        public string Parse(string signal) 
        {
            var chatClient = _client.GetChatClient("gpt-3.5-turbo");
            var results = chatClient.CompleteChatStreaming("This agent should receive trading signals in text and parse the signals to a standard format. The signals contain information about the market, take profits, and stop losses. The response should be the parsed signal and nothing more.\nFormat:\r\n<parsed_buy_or_sell> <parsed_market> <parsed_entry_value>\nTPx <parsed_tpx>\r\nSLx  <parsed_tpx>", "Sell Gold 2587.5 - 2584.5\r\n\r\nStop Loss 2590.5\r\n\r\nTP1 2583\r\nTP2 2581\r\nTP3 2579\r\nTP4 Open (2577/2575)");
            foreach (var result in results)
            {
                Console.WriteLine(result.GetAzureMessageContext());
            }
            return "";
        }
    }
}
