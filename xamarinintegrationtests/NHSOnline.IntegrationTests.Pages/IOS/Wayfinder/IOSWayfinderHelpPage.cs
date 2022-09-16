using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Wayfinder
{
    public class IOSWayfinderHelpPage
    {
        private IOSWayfinderHelpPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new WayfinderHelpPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public WayfinderHelpPageContent PageContent { get; }

        public static IOSWayfinderHelpPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSWayfinderHelpPage(driver);
            page.PageContent.AssertPageElements();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSWayfinderHelpPage));
            }

            return page;
        }
    }
}

