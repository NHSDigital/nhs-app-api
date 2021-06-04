using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSCell
    {
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSCell(IIOSLocatorStrategy locatorStrategy)
        {
            _locatorStrategy = locatorStrategy;
        }

        public static IOSCell WithLabel(IIOSInteractor interactor, string label)
            => new IOSCell(new TextLocatorStrategy(interactor, label));

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a cell with the label {_locatorStrategy.Description} should be displayed"));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _label;

            public TextLocatorStrategy(IIOSInteractor interactor, string label)
            {
                _interactor = interactor;
                _label= label;
            }

            public string Description => $"with text '{_label}'";

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeCell' AND label == {_label.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}