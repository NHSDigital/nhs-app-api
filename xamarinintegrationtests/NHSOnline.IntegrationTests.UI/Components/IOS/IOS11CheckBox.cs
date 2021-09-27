using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public class IOS11CheckBox
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _locatorStrategy;

        private IOS11CheckBox(IIOSInteractor interactor, IIOSLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static IOS11CheckBox StartsWith(IIOSInteractor interactor, string text)
            => new IOS11CheckBox(interactor, new TextLocatorStrategy(interactor, text));

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

            // In testing, the XCUIElementTypeOther containing the XCUIElementTypeSwitch and the label elements had to be clicked to cause the checkbox to be selected
            // This XPath was the selected way of finding it because it must be selected based on its children
            public By FindBy =>
                By.XPath($"//XCUIElementTypeOther[child::XCUIElementTypeSwitch and child::XCUIElementTypeOther[child::XCUIElementTypeStaticText[starts-with(@value,{_text.QuoteXPathLiteral()})]]]");

            public void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action) =>
                _interactor.ActOnElementContext(FindBy, action);

            public void AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(FindBy, because);
        }
    }
}