using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSUpdatedTermsAndConditionsPage
    {
        public UpdatedTermsAndConditionsPageContent PageContent { get; }

        private IOSUpdatedTermsAndConditionsPage(IIOSDriverWrapper driver)
        {
            PageContent = new UpdatedTermsAndConditionsPageContent(driver.Web.NhsAppPreHomeWebView());
        }

        public static IOSUpdatedTermsAndConditionsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSUpdatedTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageContent()
        {
            PageContent.AssertPageContent();
        }
    }
}