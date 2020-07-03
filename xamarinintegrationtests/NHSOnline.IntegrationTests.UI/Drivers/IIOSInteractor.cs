using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSInteractor
    {
        internal void ActOnElement(By by, Action<IOSElement> action, Action<InteractorOptions>? configure = null);
    }
}