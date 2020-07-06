using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidInteractor
    {
        internal void ActOnElement(By by, Action<AndroidElement> action, Action<InteractorOptions>? configure = null);
        internal void AssertElementDoesntExist(By by);
    }
}