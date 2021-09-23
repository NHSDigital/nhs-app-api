using NHSOnline.IntegrationTests.Pages.WebPageContent.Errors;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSInternetConnectionErrorPage
    {
        private InternetConnectionErrorPageContent PageContent { get; set; }

        private IOSInternetConnectionErrorPage(IIOSDriverWrapper driver)
        {
            PageContent = new InternetConnectionErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSInternetConnectionErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSInternetConnectionErrorPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements() => PageContent.AssertPageElements();
    }
}