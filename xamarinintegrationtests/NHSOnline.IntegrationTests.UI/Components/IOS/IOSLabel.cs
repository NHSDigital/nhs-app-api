using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSLabel
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        public IOSLabel(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public void AssertVisible(Action<InteractorOptions>? configure = null)
        {
            _interactor.ActOnElement(
                By.XPath($"//XCUIElementTypeStaticText[normalize-space(@value)={_text.QuoteXPathLiteral()} and @visible = 'true']"),
                e => e.Displayed.Should().BeTrue("a label with text {1} should be displayed", _text),
                configure);
        }

        public void AssertNotVisible()
        {
            _interactor.AssertElementDoesntExist(
                By.XPath($"//XCUIElementTypeStaticText[normalize-space(@value)={_text.QuoteXPathLiteral()} and @visible = 'true']"));
        }

        public void AssertLabelVisible()
        {
            _interactor.ActOnElement(
                By.XPath($"//XCUIElementTypeStaticText[normalize-space(@label)={_text.QuoteXPathLiteral()} and @visible = 'true']"),
                e => e.Displayed.Should().BeTrue("a label with text {1} should be displayed", _text));
        }

        public void Click()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Click());
        }

        private By XPath =>
            By.XPath(
                $"//XCUIElementTypeStaticText[normalize-space(@value)={_text.QuoteXPathLiteral()} and @visible = 'true']");
    }
}