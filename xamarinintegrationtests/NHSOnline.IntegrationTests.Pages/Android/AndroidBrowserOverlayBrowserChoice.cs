using System;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidBrowserOverlayBrowserChoice
    {
        private AndroidBrowserChoice AndroidBrowserChoice { get; }

        private AndroidBrowserOverlayBrowserChoice(IAndroidDriverWrapper driver)
        {
            AndroidBrowserChoice = new AndroidBrowserChoice(driver, "Chrome");
        }

        public static void IfDisplayed(IAndroidDriverWrapper driver, Action<AndroidBrowserOverlayBrowserChoice> action)
        {
            var browserChoice = new AndroidBrowserOverlayBrowserChoice(driver);
            if (browserChoice.AndroidBrowserChoice.IsDisplayed())
            {
                action(browserChoice);
            }
        }

        public AndroidBrowserOverlayBrowserChoice ChooseChrome()
        {
            AndroidBrowserChoice.ChooseTargetApp();
            return this;
        }
    }
}