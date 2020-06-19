using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebButton
    {
        private readonly IWebInteractor _driver;
        private readonly string _text;

        public WebButton(IWebInteractor driver, string text)
        {
            _driver = driver;
            _text = text;
        }

        public void Click()
        {
            _driver.ActOnElement(
                By.XPath($"//button[normalize-space(text())={_text.QuoteXPathLiteral()}]"),
                e => e.Click());
        }
    }
}