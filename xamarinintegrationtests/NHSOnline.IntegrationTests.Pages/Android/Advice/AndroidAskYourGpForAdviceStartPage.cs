using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Advice
{
    public class AndroidAskYourGpForAdviceStartPage
    {
        public AskYourGpForAdviceStartPageContent PageContent { get; }

        private AndroidAskYourGpForAdviceStartPage(IAndroidDriverWrapper driver) =>
            PageContent = new AskYourGpForAdviceStartPageContent(driver.Web.NhsAppLoggedInWebView());

        public static AndroidAskYourGpForAdviceStartPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAskYourGpForAdviceStartPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}