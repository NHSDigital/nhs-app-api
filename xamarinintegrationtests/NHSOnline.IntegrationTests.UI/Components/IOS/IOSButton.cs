using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSButton
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        public IOSButton(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public void Click()
        {
            _interactor.ActOnElement(
                By.XPath($"//XCUIElementTypeButton[normalize-space(@label)={_text.QuoteXPathLiteral()} and @visible = 'true']"),
                e => e.Click());
        }
    }
}