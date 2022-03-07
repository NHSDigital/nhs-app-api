using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Advice
{
    public class IOSAskYourGpForAdviceTermsPage
    {
        public AskYourGpForAdviceTermsPageContent PageContent { get; }

        private IOSAskYourGpForAdviceTermsPage(IIOSDriverWrapper driver) =>
            PageContent = new AskYourGpForAdviceTermsPageContent(driver.Web.NhsAppLoggedInWebView());

        public static IOSAskYourGpForAdviceTermsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAskYourGpForAdviceTermsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}