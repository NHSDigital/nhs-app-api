using FluentAssertions;
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
                XPath,
                e => e.Click());
        }

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Displayed.Should().BeTrue("a label with text {1} should be displayed", _text));
        }

        private By XPath =>
            By.XPath(
                $"//XCUIElementTypeButton[normalize-space(@label)={_text.QuoteXPathLiteral()} and @visible='true']");
    }
}
