using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidAppTabBrowserChoice
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidAppTabBrowserChoice(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel OpenWithText => AndroidLabel.WithText(_driver, "Open with");
        private AndroidAppTabBrowserChoiceOption ChromeOption => AndroidAppTabBrowserChoiceOption.WithText(_driver, "Chrome");
        private AndroidButton AlwaysButton => AndroidButton.WithText(_driver, "ALWAYS");

        public static AndroidAppTabBrowserChoice AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var browserChoice = new AndroidAppTabBrowserChoice(driver);
            browserChoice.OpenWithText.AssertVisible();
            return browserChoice;
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