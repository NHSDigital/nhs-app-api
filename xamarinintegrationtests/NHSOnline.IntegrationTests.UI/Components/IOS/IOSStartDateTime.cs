using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
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
            string date,
            string year,
            string time)
            => new IOSStartDateTime(new TextLocatorStrategy(interactor, prefixText, month, year, date, time));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a cell with the label {_locatorStrategy.Description} should be displayed"));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _prefixText;
            private readonly string _month;
            private readonly string _year;
            private readonly string _date;
            private readonly string _time;

            public TextLocatorStrategy(
                IIOSInteractor interactor,
                string prefixText,
                string month,
                string year,
                string date,
                string time)
            {
                _interactor = interactor;
                _prefixText = prefixText;
                _month = month;
                _year = year;
                _date = date;
                _time = time;
            }

            public string Description => "with date time";

            public By FindBy =>
                MobileBy.IosNSPredicate($"type == 'XCUIElementTypeCell' AND label CONTAINS {_prefixText.QuotePredicateLiteral()} AND label CONTAINS {_date.QuotePredicateLiteral()} AND label CONTAINS {_month.QuotePredicateLiteral()} AND label CONTAINS {_year.QuotePredicateLiteral()} AND label CONTAINS {_time.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}