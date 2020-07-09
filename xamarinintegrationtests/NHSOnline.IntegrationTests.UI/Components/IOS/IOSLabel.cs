using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSLabel
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        public IOSLabel(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public void AssertVisible() => ActOnElement(e => e.Displayed.Should().BeTrue("a label with text {1} should be displayed", _text));

        public void AssertNotVisible() => _interactor.AssertElementDoesntExist(FindBy);

        public void AssertLabelVisible() => ActOnElement(e => e.Displayed.Should().BeTrue("a label with text {1} should be displayed", _text));

        public void Click() => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IOSElement> action) => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value == {_text.QuotePredicateLiteral()} and visible == 1");
    }
}