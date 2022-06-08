using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    public class IOSSafariApp {

        private IIOSBrowserStackDriver Driver { get; }

        public IOSSafariApp(IIOSBrowserStackDriver driver)
        {
            Driver = driver;
        }

        public static void VerifyUrl(IIOSDriverWrapper driver, string expectedHeader) =>
            IOSLabel.WithText(driver, expectedHeader).AssertVisible();
    }
}