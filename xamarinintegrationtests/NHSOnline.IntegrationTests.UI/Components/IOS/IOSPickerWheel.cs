using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSPickerWheel
    {
        private readonly IIOSInteractor _interactor;

        public IOSPickerWheel(IIOSInteractor interactor)
        {
            _interactor = interactor;
        }

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);

        public void SetValue(string value)
            => ActOnElement(e => e.SetImmediateValue(value));

        public void Click() => ActOnElement(e => e.Click());

        private By FindBy
            => MobileBy.IosNSPredicate($"type == 'XCUIElementTypePickerWheel'");

    }
}
