using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Advice
{
    public class IOSAskYourGpForAdviceStartPage
    {
        public AskYourGpForAdviceStartPageContent PageContent { get; }

        private IOSAskYourGpForAdviceStartPage(IIOSDriverWrapper driver) =>
            PageContent = new AskYourGpForAdviceStartPageContent(driver.Web.NhsAppLoggedInWebView());

        public static IOSAskYourGpForAdviceStartPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAskYourGpForAdviceStartPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}