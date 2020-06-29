using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidBeforeYouStartPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidBeforeYouStartPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => new AndroidLabel(_driver, "Before You Start");

        private AndroidButton ContinueButton => new AndroidButton(_driver, "Continue");

        internal static AndroidBeforeYouStartPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidBeforeYouStartPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        internal void Continue()
        {
            ContinueButton.Click();
        }
    }
}