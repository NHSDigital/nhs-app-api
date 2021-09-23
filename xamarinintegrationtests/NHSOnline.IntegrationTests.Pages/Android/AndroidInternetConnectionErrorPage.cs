using NHSOnline.IntegrationTests.Pages.WebPageContent.Errors;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidInternetConnectionErrorPage
    {
        private InternetConnectionErrorPageContent PageContent { get; set; }

        private AndroidInternetConnectionErrorPage(IAndroidDriverWrapper driver)
        {
            PageContent = new InternetConnectionErrorPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidInternetConnectionErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidInternetConnectionErrorPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements() => PageContent.AssertPageElements();
    }
}