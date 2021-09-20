using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOS11HeaderLabel
    {
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOS11HeaderLabel(IIOSLocatorStrategy locatorStrategy)
        {
            _locatorStrategy = locatorStrategy;
        }

        public static IOS11HeaderLabel WithText(IIOSInteractor interactor, string text)
            => new IOS11HeaderLabel(new TextLocatorStrategy(interactor, text));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a label {_locatorStrategy.Description} should be displayed"));

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND label == {_text.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}