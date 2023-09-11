using OpenQA.Selenium;
using System.Collections.ObjectModel;
using Utils.Selenium;

namespace TelegramEngine.Telegram
{
    public class TelegramChromeController : Driver
    {
        private readonly string channelUrl;
        public TelegramChromeController(string channelUrl)
        : base()
        {
            this.channelUrl = channelUrl;
            base.Init();
        }

        public void LoadPage()
        {
            Get(channelUrl);
        }

        public void GoToTopOfPage()
        {
            var element = WaitForElementByXPath("//div[text()='Channel created']", 1);
            while(element == null) 
            {
                ScrollUp();
                ScrollUp();
                ScrollUp();
                ScrollUp();
                ScrollUp();
                element = WaitForElementByXPath("//div[text()='Channel created']", 1);
            }
        }

        public ReadOnlyCollection<IWebElement> GetAllMessages() 
        {
            //return browser.FindElements(By.XPath("//div[@class='tgme_widget_message_text js-message_text before_footer']|//a[@class='tgme_widget_message_date']/time[@class='time']"));
            return browser.FindElements(By.XPath("//div[@class='tgme_widget_message_bubble']"));
        }
    }
}
