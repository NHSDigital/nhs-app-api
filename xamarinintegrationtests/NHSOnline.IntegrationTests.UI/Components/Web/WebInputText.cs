using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebInputText
    {
        private readonly IWebInteractor _driver;
        private readonly InputBy _by;

        public WebInputText(IWebInteractor driver, string label)
        {
            _driver = driver;
            _by = new InputBy("text", label);
        }

        public void EnterText(string text) => _driver.ActOnElement(_by.Input, e => e.SendKeys(text));
    }
}