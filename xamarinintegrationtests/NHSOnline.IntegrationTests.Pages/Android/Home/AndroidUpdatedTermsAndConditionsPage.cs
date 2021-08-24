using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Home
{
    public class AndroidUpdatedTermsAndConditionsPage
    {
        private UpdatedTermsAndConditionsPageContent PageContent { get; }

        private AndroidUpdatedTermsAndConditionsPage(IAndroidDriverWrapper driver)
        {
            PageContent = new UpdatedTermsAndConditionsPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static AndroidUpdatedTermsAndConditionsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidUpdatedTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageContent()
        {
            PageContent.AssertPageContent();
        }
    }
}