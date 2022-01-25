using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSSystemImage
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSSystemImage(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSSystemImage WhichMatches(IIOSInteractor interactor, string pattern)
            => new IOSSystemImage(interactor, new MatchesLocatorStrategy(interactor, pattern));

        public void Click() => _interactor.ActOnElementContext(_locatorStrategy.FindBy, context=>context.Element.Click());

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeImage' AND name MATCHES {_pattern.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);

            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}