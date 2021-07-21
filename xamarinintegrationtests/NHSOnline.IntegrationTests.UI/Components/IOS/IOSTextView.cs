using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSTextView
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSTextView(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSTextView WithText(IIOSInteractor interactor, string text)
            => new IOSTextView(interactor, new TextLocatorStrategy(interactor, text));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a text view {_locatorStrategy.Description} should be displayed"));

        public IOSTextView ScrollIntoView()
            => new IOSTextView(_interactor, new IOSScrollLocatorStrategy(_interactor, _locatorStrategy));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _text;

            public TextLocatorStrategy(IIOSInteractor interactor, string text)
            {
                _interactor = interactor;
                _text = text;
            }

            public string Description => $"with text '{_text}'";

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeTextView' AND value == {_text.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}