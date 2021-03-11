using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSSystemLinkLabel
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSSystemLinkLabel(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSSystemLinkLabel WithText(IIOSInteractor interactor, string text)
            => new IOSSystemLinkLabel(interactor, new TextLocatorStrategy(interactor, text));

        public static IOSSystemLinkLabel WhichMatches(IIOSInteractor interactor, string pattern)
            => new IOSSystemLinkLabel(interactor, new MatchesLocatorStrategy(interactor, pattern));

        public void Click() => _interactor.ActOnElementContext(_locatorStrategy.FindBy, context=>context.Element.Click());

        public IOSSystemLinkLabel ScrollIntoView()
            => new IOSSystemLinkLabel(_interactor, new IOSScrollLocatorStrategy(_interactor, _locatorStrategy));

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeCell' AND name == {_text.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeCell' AND name MATCHES {_pattern.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);

            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}