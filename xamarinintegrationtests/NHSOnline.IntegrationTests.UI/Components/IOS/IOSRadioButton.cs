using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public class IOSRadioButton
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOSRadioButton(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOSRadioButton StartsWith(IIOSInteractor interactor, string text)
            => new IOSRadioButton(interactor, new TextLocatorStrategy(interactor, text));

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action)
            => _interactor.ActOnElementContext(_locatorStrategy.FindBy, action);

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

            public string Description => $"which has text '{_text}'";

            public By FindBy =>
                By.XPath($"//XCUIElementTypeOther[starts-with(@label,{_text.QuoteXPathLiteral()})]");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) =>
                _interactor.ActOnElementContext(FindBy, action);

            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}