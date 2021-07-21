using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSStartDateTime
    {
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSStartDateTime(
            IIOSLocatorStrategy locatorStrategy)
        {
            _locatorStrategy = locatorStrategy;
        }

        public static IOSStartDateTime WithLabel(
            IIOSInteractor interactor,
            string prefixText,
            string month,
            string day,
            string year,
            string twelveHourTime,
            string twentyFourHourTime)
            => new IOSStartDateTime(new TextLocatorStrategy(interactor, prefixText, month, year, day, twelveHourTime, twentyFourHourTime));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a cell with the label {_locatorStrategy.Description} should be displayed"));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _prefixText;
            private readonly string _month;
            private readonly string _year;
            private readonly string _date;
            private readonly string _twelveHourTime;
            private readonly string _twentyFourHourTime;

            public TextLocatorStrategy(
                IIOSInteractor interactor,
                string prefixText,
                string month,
                string year,
                string date,
                string twelveHourTime,
                string twentyFourHourTime)
            {
                _interactor = interactor;
                _prefixText = prefixText;
                _month = month;
                _year = year;
                _date = date;
                _twelveHourTime = twelveHourTime;
                _twentyFourHourTime = twentyFourHourTime;
            }

            public string Description => $"with date time";

            public By FindBy =>
                MobileBy.IosNSPredicate($"type == 'XCUIElementTypeCell' " +
                                        $"AND label CONTAINS {_prefixText.QuotePredicateLiteral()} " +
                                        $"AND label CONTAINS {_date.QuotePredicateLiteral()} " +
                                        $"AND label CONTAINS {_month.QuotePredicateLiteral()} " +
                                        $"AND label CONTAINS {_year.QuotePredicateLiteral()} " +
                                        $"AND (label CONTAINS {_twelveHourTime.QuotePredicateLiteral()} " +
                                        $"OR label CONTAINS {_twentyFourHourTime.QuotePredicateLiteral()})");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}