using System;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidAppTabBrowserChoice
    {
        private AndroidAppChoice AndroidAppChoice { get; }

        private AndroidAppTabBrowserChoice(IAndroidDriverWrapper driver)
        {
            AndroidAppChoice = new AndroidAppChoice(driver, "Chrome");
        }

        public static void IfDisplayed(IAndroidDriverWrapper driver, Action<AndroidAppTabBrowserChoice> action)
        {
            var browserChoice = new AndroidAppTabBrowserChoice(driver);
            if (browserChoice.AndroidAppChoice.IsDisplayed())
            {
                action(browserChoice);
            }
        }

        public AndroidAppTabBrowserChoice ChooseChrome()
        {
            AndroidAppChoice.ChooseTargetApp();
            return this;
        }
    }
}