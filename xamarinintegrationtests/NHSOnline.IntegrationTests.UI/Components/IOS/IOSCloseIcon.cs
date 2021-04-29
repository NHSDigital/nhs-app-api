using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    // NHS0-13681: Temporary implementation - to be migrated to IOSIcon
    public class IOSCloseIcon
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _name;

        private IOSCloseIcon(IIOSInteractor interactor, string name)
        {
            _interactor = interactor;
            _name = name;
        }

        public static IOSCloseIcon WithDescription(IIOSInteractor interactor, string description)
            => new IOSCloseIcon(interactor, description);

        public void Click()
            => ActOnElement(e => e.Click(), FindBy);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with name {1} should be displayed", _name), FindBy );

        private void ActOnElement(Action<IOSElement> action, By findBy)
            => _interactor.ActOnElement(findBy, action);

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND name == {_name.QuotePredicateLiteral()}");
    }
}