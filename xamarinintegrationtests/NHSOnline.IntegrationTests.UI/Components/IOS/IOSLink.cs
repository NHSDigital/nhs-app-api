using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSLink
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSLink(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSLink WithText(IIOSInteractor interactor, string text)
            => new IOSLink(interactor, new TextLocatorStrategy(interactor, text));

        public static IOSLink WhichMatches(IIOSInteractor interactor, string pattern)
            => new IOSLink(interactor, new MatchesLocatorStrategy(interactor, pattern));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a link with text  {_locatorStrategy.Description} should be displayed"));

        public void Touch() => _interactor.ActOnElementContext(_locatorStrategy.FindBy, context=>context.Tap());

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeLink' AND label == {_text.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }

        private sealed class MatchesLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _pattern;

            public MatchesLocatorStrategy(IIOSInteractor interactor, string pattern)
            {
                _interactor = interactor;
                _pattern = pattern;
            }

            public string Description => $"which matches '{_pattern}'";

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeLink' AND label MATCHES {_pattern.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }


    }
}