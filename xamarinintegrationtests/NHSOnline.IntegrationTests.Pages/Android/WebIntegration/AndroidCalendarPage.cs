using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidCalendarPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCalendarPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new CalendarPageContent(driver.Web(WebViewContext.TestProviderWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        public CalendarPageContent PageContent { get; }

        public static AndroidCalendarPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCalendarPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidCalendarPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}