using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.MultiTouch;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    public class AndroidChromeApp
    {
        private const string AppPackage = "com.android.chrome";
        private const string AppActivity = "com.google.android.apps.chrome.Main";

        private readonly AndroidDriver<AndroidElement> _driver;
        private readonly IAndroidInteractor _interactor;

        private AndroidChromeApp(AndroidDriver<AndroidElement> driver, IAndroidInteractor interactor)
        {
            _driver = driver;
            _interactor = interactor;
        }

        public static AndroidChromeApp Launch(AndroidDriver<AndroidElement> driver, IAndroidInteractor interactor)
        {
            driver.StartActivity(AppPackage, AppActivity);
            return new AndroidChromeApp(driver, interactor);
        }

        public void NavigateToDeepLinkLauncher()
            => NavigateTo("http://deeplinklauncher.stubs.local.bitraft.io:8080/deeplinks");

        private void NavigateTo(string destination)
        {
            _driver.FindElement(MobileBy.Id($"{AppPackage}:id/search_box_text"))
                .ReplaceValue(destination);
            _interactor.PressEnterKey();
        }
    }
}