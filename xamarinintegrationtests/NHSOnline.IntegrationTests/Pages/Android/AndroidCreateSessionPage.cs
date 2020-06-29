using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidCreateSessionPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Message => new AndroidLabel(_driver, "Creating Session");

        internal static AndroidCreateSessionPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionPage(driver);
            page.Message.AssertVisible();
            return page;
        }
    }
}