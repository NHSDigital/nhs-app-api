using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.YourHealth
{
    public class IOSYourHealthSubstraktPage
    {
        private IOSYourHealthSubstraktPage(IIOSDriverWrapper driver)
            => PageContent = new YourHealthSubstraktPageContent(driver.Web.NhsAppLoggedInWebView());

        public YourHealthSubstraktPageContent PageContent { get; }

        public static IOSYourHealthSubstraktPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSYourHealthSubstraktPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}