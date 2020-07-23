using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class AndroidAppTabBrowserChoice
    {
        private readonly IAndroidDriverWrapper _driver;

        public AndroidAppTabBrowserChoice(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidAppTabBrowserChoiceOption ChromeOption => AndroidAppTabBrowserChoiceOption.WithText(_driver, "Chrome");
        private AndroidButton JustOnceButton => new AndroidButton(_driver, "JUST ONCE");

        public AndroidAppTabBrowserChoice ChooseChrome()
        {
            ChromeOption.Click();
            return this;
        }

        public AndroidAppTab JustOnce()
        {
            JustOnceButton.Click();
            return new AndroidAppTab(_driver);
        }
    }
}