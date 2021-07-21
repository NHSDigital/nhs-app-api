using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    internal interface IIOSLocatorStrategy
    {
        string Description { get; }
        By FindBy { get; }
        void ActOnElementContext(Action<ElementContext<IIOSBrowserStackDriver, IOSElement>> action);
        void AssertCannotBeFound(string because);
    }
}