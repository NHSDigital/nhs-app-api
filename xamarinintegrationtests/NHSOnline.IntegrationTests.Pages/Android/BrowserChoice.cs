using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public sealed class BrowserChoice
    {
        private readonly IAndroidDriverWrapper _driver;

        public BrowserChoice(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel ChromeOption => AndroidLabel.WithText(_driver, "Chrome");
        private AndroidButton JustOnceButton => new AndroidButton(_driver, "JUST ONCE");

        public BrowserChoice ChooseChrome()
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