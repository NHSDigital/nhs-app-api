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
        private readonly string _name;

        private IOSIcon(IIOSInteractor interactor, string name)
        {
            _interactor = interactor;
            _name = name;
        }

        public static IOSIcon WithName(IIOSInteractor interactor, string name)
            => new IOSIcon(interactor, name);

        public static IOSIcon WithDescription(IIOSInteractor interactor, string description)
            => new IOSIcon(interactor, description);

        public void ClickButton()
            => ActOnElement(e => e.Click(), FindButtonBy);

        public void AssertButtonVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with name {1} should be displayed", _name), FindButtonBy );

        public void ClickOther()
            => ActOnElement(e => e.Click(), FindOtherBy);

        public void AssertOtherVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with name {1} should be displayed", _name), FindOtherBy );

        private void ActOnElement(Action<IOSElement> action, By findBy)
            => _interactor.ActOnElement(findBy, action);

        private By FindButtonBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeButton' AND name == {_name.QuotePredicateLiteral()}");

        private By FindOtherBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND name == {_name.QuotePredicateLiteral()}");
    }
}