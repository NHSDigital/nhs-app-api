using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSNavigationHeader: IIOSContainer
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _name;
        private readonly IIOSInteractor _containerInteractor;

        private IOSNavigationHeader(IIOSInteractor interactor, string name)
        {
            _interactor = interactor;
            _name = name;
            _containerInteractor = _interactor.CreateContainedInteractor(FindBy);
        }

        public static IOSNavigationHeader WithName(IIOSInteractor interactor, string name)
            => new (interactor, name);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a {0} should be displayed", _name));

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND name == {_name.QuotePredicateLiteral()}");

        IIOSInteractor IIOSContainer.ContainerInteractor => _containerInteractor;
    }
}