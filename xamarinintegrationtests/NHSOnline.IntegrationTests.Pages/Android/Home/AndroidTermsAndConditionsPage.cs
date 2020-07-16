using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidTermsAndConditionsPage
    {
        public TermsAndConditionsPageContent PageContent { get; }

        private AndroidTermsAndConditionsPage(IAndroidDriverWrapper driver)
        {
            PageContent = new TermsAndConditionsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidTermsAndConditionsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}