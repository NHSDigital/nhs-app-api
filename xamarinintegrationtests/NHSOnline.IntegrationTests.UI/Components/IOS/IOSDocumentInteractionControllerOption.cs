using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSDocumentInteractionControllerOption
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSDocumentInteractionControllerOption(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSDocumentInteractionControllerOption WithText(IIOSInteractor interactor, string text)
            => new IOSDocumentInteractionControllerOption(interactor, new TextLocatorStrategy(interactor, text));

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(_locatorStrategy.FindBy, action);

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

            public By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeCell' AND label == {_text.QuotePredicateLiteral()}");

            public void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) => _interactor.ActOnElementContext(FindBy, action);

            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}