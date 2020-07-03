using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebRadioOption
    {
        private readonly IWebInteractor _driver;
        private readonly By _labelBy;

        public WebRadioOption(IWebInteractor driver, string legend, string option)
        {
            _driver = driver;
            _labelBy = By.XPath($"//fieldset[legend[normalize-space(text()) = {legend.QuoteXPathLiteral()}]]//label[normalize-space(text()) = {option.QuoteXPathLiteral()}]");
        }

        public void Click() => _driver.ActOnElement(_labelBy, e => e.Click());
    }
}