using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidStoragePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Recent");

        private AndroidLabel FileLabel => AndroidLabel.WhichMatches(_driver, ".*.jpg");

        private AndroidStoragePage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
        }

        public static AndroidStoragePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStoragePage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void SelectFile() => FileLabel.Click();
    }
}