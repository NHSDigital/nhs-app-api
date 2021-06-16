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
            string twelveHourTime,
            string twentyFourHourTime)
            => new IOSEndTime(new TextLocatorStrategy(interactor, prefixText, twelveHourTime, twentyFourHourTime));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a cell with the label {_locatorStrategy.Description} should be displayed"));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _prefixText;
            private readonly string _twelveHourTime;
            private readonly string _twentyFourHourTime;

            public TextLocatorStrategy(
                IIOSInteractor interactor,
                string prefixText,
                string twelveHourTime,
                string twentyFourHourTime)
            {
                _interactor = interactor;
                _prefixText = prefixText;
                _twelveHourTime = twelveHourTime;
                _twentyFourHourTime = twentyFourHourTime;
            }

            public string Description => $"{_prefixText} and time of {_twentyFourHourTime} or {_twelveHourTime}";

            public By FindBy =>
                MobileBy.IosNSPredicate(
                    $"type == 'XCUIElementTypeCell' " +
                    $"AND label CONTAINS {_prefixText.QuotePredicateLiteral()} " +
                    $"AND (label CONTAINS {_twentyFourHourTime.QuotePredicateLiteral()} " +
                    $"OR label CONTAINS {_twelveHourTime.QuotePredicateLiteral()})");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}