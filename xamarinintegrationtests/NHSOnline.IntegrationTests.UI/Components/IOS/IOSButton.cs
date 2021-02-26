using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSButton
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSButton(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSButton WithText(IIOSInteractor interactor, string text)
            => new IOSButton(interactor, new TextLocatorStrategy(interactor, text));

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertVisible() => _locatorStrategy.ActOnElementContext(
            context => context.Element.Displayed.Should().BeTrue($"a button with text {_locatorStrategy.Description} should be displayed"));

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeButton' AND label == {_locatorStrategy.Description}");

        public IOSButton ScrollIntoView()
            => new IOSButton(_interactor, new IOSScrollLocatorStrategy(_interactor, _locatorStrategy));

        private sealed class TextLocatorStrategy : IIOSLocatorStrategy
        {
            private readonly IIOSInteractor _interactor;
            private readonly string _text;

            public TextLocatorStrategy(IIOSInteractor interactor, string text)
            {
                _interactor = interactor;
                _text = text;
            }

            public string Description => $"'{_text}'";

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeButton' AND label == {_text.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);
        }
    }
}
