using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSLink
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        private IOSLink(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static IOSLink WithText(IIOSInteractor interactor, string text)
            => new IOSLink(interactor, text);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue($"a link with text '{_text}' should be displayed"));

        public void Touch()
            => ActOnElementContext(context => context.Tap());

        private void ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action)
            => _interactor.ActOnElementContext(FindBy, action);

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value == {_text.QuotePredicateLiteral()}");
    }
}