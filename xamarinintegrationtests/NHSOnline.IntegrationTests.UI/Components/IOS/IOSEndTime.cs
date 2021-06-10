using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSEndTime
    {
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSEndTime(
            IIOSLocatorStrategy locatorStrategy)
        {
            _locatorStrategy = locatorStrategy;
        }

        public static IOSEndTime WithLabel(
            IIOSInteractor interactor,
            string prefixText,
            string time)
            => new IOSEndTime(new TextLocatorStrategy(interactor, prefixText, time));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a cell with the label {_locatorStrategy.Description} should be displayed"));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _prefixText;
            private readonly string _time;

            public TextLocatorStrategy(
                IIOSInteractor interactor,
                string prefixText,
                string time)
            {
                _interactor = interactor;
                _prefixText = prefixText;
                _time = time;
            }

            public string Description => $"{_prefixText} and time of {_time}";

            public By FindBy =>
                MobileBy.IosNSPredicate(
                    $"type == 'XCUIElementTypeCell' AND label CONTAINS {_prefixText.QuotePredicateLiteral()} AND label CONTAINS {_time.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}