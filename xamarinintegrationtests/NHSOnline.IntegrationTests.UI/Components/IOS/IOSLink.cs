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
            => new IOSLink(interactor, new MatchesLocatorStrategy(interactor, pattern, true));

        public static IOSLink WhichContains(IIOSInteractor interactor, string pattern)
            => new IOSLink(interactor, new MatchesLocatorStrategy(interactor, pattern, false));

        public IOSLink ScrollIntoView()
            => new IOSLink(_interactor, new IOSScrollLocatorStrategy(_interactor, _locatorStrategy));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a link with text  {_locatorStrategy.Description} should be displayed"));

        public void Touch() => _interactor.ActOnElementContext(_locatorStrategy.FindBy, context=>context.Tap());

        private abstract class BaseLinkLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly string _pattern;
            protected IIOSInteractor Interactor { get; set; }

            protected BaseLinkLocatorStrategy(IIOSInteractor interactor, string pattern, By findBy)
            {
                Interactor = interactor;
                _pattern = pattern;
                FindBy = findBy;
            }

            public string Description => $"which matches '{_pattern}'";

            public virtual By FindBy { get; }

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => Interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => Interactor.AssertElementCannotBeFound(FindBy, because);
        }

        private sealed class TextLocatorStrategy : BaseLinkLocatorStrategy
        {
            public TextLocatorStrategy(IIOSInteractor interactor, string text) :
                base(interactor,
                    text,
                    MobileBy.IosNSPredicate($"type == 'XCUIElementTypeLink' AND label == {text.QuotePredicateLiteral()}"))
            {
            }
        }

        private sealed class MatchesLocatorStrategy : BaseLinkLocatorStrategy
        {
            public MatchesLocatorStrategy(IIOSInteractor interactor, string pattern, bool exactMatch) :
                base(interactor,
                    pattern,
                    MobileBy.IosNSPredicate($"type == 'XCUIElementTypeLink' AND label {(exactMatch ? "MATCHES" : "CONTAINS")} {pattern.QuotePredicateLiteral()}"))
            {
            }
        }
    }
}