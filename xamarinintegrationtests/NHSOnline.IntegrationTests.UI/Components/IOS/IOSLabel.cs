using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSLabel
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSLabel(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSLabel WithText(IIOSInteractor interactor, string text)
            => new IOSLabel(interactor, new TextLocatorStrategy(interactor, text));

        public static IOSLabel WhichMatches(IIOSInteractor interactor, string pattern)
            => new IOSLabel(interactor, new MatchesLocatorStrategy(interactor, pattern));

        public IOSLabel ScrollIntoView()
            => new IOSLabel(_interactor, new IOSScrollLocatorStrategy(_interactor, _locatorStrategy));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a label {_locatorStrategy.Description} should be displayed"));

        public void AssertNotPresent() => _locatorStrategy.AssertCannotBeFound($"a label {_locatorStrategy.Description} should not be displayed");

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value == {_text.QuotePredicateLiteral()}");

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value MATCHES {_pattern.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}