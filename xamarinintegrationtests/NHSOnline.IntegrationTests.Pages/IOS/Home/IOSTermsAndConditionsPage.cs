using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Home
{
    public class IOSTermsAndConditionsPage
    {
        public TermsAndConditionsPageContent PageContent { get; }

        private IOSTermsAndConditionsPage(IIOSDriverWrapper driver)
        {
            PageContent = new TermsAndConditionsPageContent(driver.Web(WebViewContext.NhsAppPreHome));
        }

        public static IOSTermsAndConditionsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageContent()
        {
            PageContent.AssertPageContent();
        }
    }
}