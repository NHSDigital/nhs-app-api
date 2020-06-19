using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidLoggedOutHomePage
    {
        private readonly IAndroidDriverWrapper _androidDriver;

        private AndroidLoggedOutHomePage(IAndroidDriverWrapper androidDriver) => _androidDriver = androidDriver;

        private AndroidLabel WelcomeMessage => new AndroidLabel(_androidDriver, "Welcome to the NHS App");

        internal static AndroidLoggedOutHomePage AssertOnPage(IAndroidDriverWrapper androidDriver)
        {
            var page = new AndroidLoggedOutHomePage(androidDriver);
            page.WelcomeMessage.AssertVisible();
            return page;
        }
    }
}