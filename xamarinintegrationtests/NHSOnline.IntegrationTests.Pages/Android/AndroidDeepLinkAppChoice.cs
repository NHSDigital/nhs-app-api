using System;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidDeepLinkAppChoice
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidDeepLinkAppChoice(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidSystemLabel OpenWithText => AndroidSystemLabel.WhichMatches(_driver, "Open( web.local.bitraft.io links)? with");

        private AndroidSystemLabel OpenWithAppText => AndroidSystemLabel.WhichMatches(_driver, "Open with NHS App - BrowserStack");

        private AndroidAppChoiceOption NhsAppChoice => AndroidAppChoiceOption.WithText(_driver, "NHS App - BrowserStack");

        private AndroidSystemButton AlwaysButton => AndroidSystemButton.WhichMatches(_driver, "(ALWAYS|Always)");

        private AndroidSystemButton JustOnceButton => AndroidSystemButton.WhichMatches(_driver, "(JUST ONCE|Just once)");

        public static AndroidDeepLinkAppChoice AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var appChoice = new AndroidDeepLinkAppChoice(driver);
            appChoice.OpenWithText.AssertVisible();
            return appChoice;
        }

        public void ChooseNhsApp()
        {
            IfLabelDisplayed(OpenWithAppText, ChooseNhsAppPreselected);
            IfLabelDisplayed(OpenWithText, ChooseNhsAppFirstTime);
        }

        private static void IfLabelDisplayed(AndroidSystemLabel label, Action action)
        {
            if (label.IsPresent())
            {
                action();
            }
        }

        private void ChooseNhsAppPreselected() => JustOnceButton.Click();

        private void ChooseNhsAppFirstTime()
        {
            NhsAppChoice.Click();
            JustOnceButton.Click();
        }
    }
}