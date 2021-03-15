using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSStubbedLoginTermsAndConditionsPage
    {
        private IOSStubbedLoginTermsAndConditionsPage(IIOSDriverWrapper driver)
        {
            PageContent = new StubbedLoginTermsAndConditionsPageContent(driver.Web(WebViewContext.NhsLogin));
        }

        public StubbedLoginTermsAndConditionsPageContent PageContent { get; }

        public static IOSStubbedLoginTermsAndConditionsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}