using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Advice
{
    public class AndroidAskYourGpForAdviceTermsPage
    {
        public AskYourGpForAdviceTermsPageContent PageContent { get; }

        private AndroidAskYourGpForAdviceTermsPage(IAndroidDriverWrapper driver) =>
            PageContent = new AskYourGpForAdviceTermsPageContent(driver.Web.NhsAppLoggedInWebView());

        public static AndroidAskYourGpForAdviceTermsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAskYourGpForAdviceTermsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}