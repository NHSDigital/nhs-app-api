using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSIcon
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _description;

        private IOSIcon(IIOSInteractor interactor, string description)
        {
            _interactor = interactor;
            _description = description;
        }

        public static IOSIcon WithDescription(IIOSInteractor interactor, string description)
            => new IOSIcon(interactor, description);

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with description {1} should be displayed", _description));

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND name == {_description.QuotePredicateLiteral()}");
    }
}