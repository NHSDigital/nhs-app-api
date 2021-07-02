using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSAppIcon
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _name;

        private IOSAppIcon(IIOSInteractor interactor, string name)
        {
            _interactor = interactor;
            _name = name;
        }

        public static IOSAppIcon WithName(IIOSInteractor interactor, string name)
            => new IOSAppIcon(interactor, name);

        public void Click()
            => ActOnElement(e => e.Click(), FindBy);

        public void AssertSelected()
            => ActOnElement(e => e.GetAttribute("value").Should().Be("1", "a selected icon with value {0} should be selected", _name), FindBy);

        public void AssertNotSelected()
            => ActOnElement(e => e.GetAttribute("value").Should().BeNullOrEmpty(), FindBy);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with name {0} should be displayed", _name), FindBy );

        private void ActOnElement(Action<IOSElement> action, By findBy)
            => _interactor.ActOnElement(findBy, action);

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeButton' AND name == {_name.QuotePredicateLiteral()}");
    }
}