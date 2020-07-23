using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSBanner
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        private IOSBanner(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static IOSBanner WithText(IIOSInteractor interactor, string text) => new IOSBanner(interactor, text);

        public void Click() => ActOnElement(e => e.Click());

        public void AssertVisible() => ActOnElement(e => e.Displayed.Should().BeTrue("a banner with text {1} should be displayed", _text));

        private void ActOnElement(Action<IOSElement> action) => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND label == {_text.QuotePredicateLiteral()}");
    }
}