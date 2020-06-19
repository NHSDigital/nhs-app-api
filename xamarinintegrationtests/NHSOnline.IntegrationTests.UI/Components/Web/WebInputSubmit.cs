using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebInputSubmit
    {
        private readonly IWebInteractor _driver;
        private readonly string _text;

        public WebInputSubmit(IWebInteractor driver, string text)
        {
            _driver = driver;
            _text = text;
        }

        public void Click()
        {
            _driver.ActOnElement(
                By.XPath($"//input[normalize-space(@value)={_text.QuoteXPathLiteral()}]"),
                e => e.Click());
        }
    }
}