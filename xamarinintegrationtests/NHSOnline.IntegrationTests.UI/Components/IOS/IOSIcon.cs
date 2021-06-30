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

        public void Click()
            => ActOnElement(e => e.Click(), FindBy);

        private void ActOnElement(Action<IOSElement> action, By findBy)
            => _interactor.ActOnElement(findBy, action);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with name {1} should be displayed", _name), FindBy );

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeIcon' AND name == {_name.QuotePredicateLiteral()}");
    }
}