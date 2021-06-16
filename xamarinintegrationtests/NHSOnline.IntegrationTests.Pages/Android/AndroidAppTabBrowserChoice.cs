using System;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidAppTabBrowserChoice
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidAppTabBrowserChoice(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidSystemLabel OpenWithText => AndroidSystemLabel.WhichMatches(_driver, "Open( links)? with");
        private AndroidAppChoiceOption ChromeOption => AndroidAppChoiceOption.WithText(_driver, "Chrome");
        private AndroidSystemButton AlwaysButton => AndroidSystemButton.WhichMatches(_driver, "(ALWAYS|Always)");

        public static AndroidAppTabBrowserChoice AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var browserChoice = new AndroidAppTabBrowserChoice(driver);
            browserChoice.OpenWithText.AssertVisible();
            return browserChoice;
        }

        public static void IfDisplayed(IAndroidDriverWrapper driver, Action<AndroidAppTabBrowserChoice> action)
        {
            var browserChoice = new AndroidAppTabBrowserChoice(driver);
            if (browserChoice.OpenWithText.IsPresent())
            {
                action(browserChoice);
            }
        }

        public AndroidAppTabBrowserChoice ChooseChrome()
        {
            ChromeOption.Click();
            return this;
        }

        public void Always()
        {
            AlwaysButton.Click();
        }
    }
}