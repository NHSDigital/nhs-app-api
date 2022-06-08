using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    public class AndroidChromeApp
    {
        private const string AppPackage = "com.android.chrome";
        private const string AppActivity = "com.google.android.apps.chrome.Main";

        private readonly IAndroidBrowserStackDriver _driver;
        private readonly IAndroidInteractor _interactor;

        private AndroidChromeApp(IAndroidBrowserStackDriver driver, IAndroidInteractor interactor)
        {
            _driver = driver;
            _interactor = interactor;
        }

        public static AndroidChromeApp Launch(IAndroidBrowserStackDriver driver, IAndroidInteractor interactor)
        {
            driver.StartActivity(AppPackage, AppActivity, stopApp: false);
            return new AndroidChromeApp(driver, interactor);
        }

        public static void VerifyUrl(IAndroidDriverWrapper driver, string expectedDestination) =>
            AndroidEditText.WithText(driver, expectedDestination).AssertVisible();

        public void NavigateToDeepLinkLauncher()
            => NavigateTo("http://deeplinklauncher.stubs.local.bitraft.io:8080/deeplinks");

        public void NavigateToDeepLinkWebIntegrationLauncher()
            => NavigateTo("http://deeplinklauncher.stubs.local.bitraft.io:8080/deeplinkswebintegration");

        public void ClickLink() => _interactor.PressEnterKey();

        private void NavigateTo(string destination)
        {
            var element = (AndroidElement)_driver.FindElement(MobileBy.Id($"{AppPackage}:id/search_box_text"));
            element.ReplaceValue(destination);
            ClickLink();
        }
    }
}