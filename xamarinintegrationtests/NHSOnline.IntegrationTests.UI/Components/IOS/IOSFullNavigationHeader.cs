using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public class IOSFullNavigationHeader
    {
        private const string Name = "NHS App Full Navigation Header";

        private readonly IIOSInteractor _interactor;

        private IOSFullNavigationHeader(IIOSInteractor interactor)
            => _interactor = interactor;

        public static IOSFullNavigationHeader Create(IIOSInteractor interactor)
            => new IOSFullNavigationHeader(interactor);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a navigation bar should be displayed"));

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private static By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND name == {Name.QuotePredicateLiteral()}");
    }
}
