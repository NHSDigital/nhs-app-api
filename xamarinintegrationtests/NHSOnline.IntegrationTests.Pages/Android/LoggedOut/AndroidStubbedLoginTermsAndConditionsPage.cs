using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsLogin;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidStubbedLoginTermsAndConditionsPage
    {
        private AndroidStubbedLoginTermsAndConditionsPage(IAndroidDriverWrapper driver)
        {
            PageContent = new StubbedLoginTermsAndConditionsPageContent(driver.Web.WebIntegrationWebView());
        }

        public StubbedLoginTermsAndConditionsPageContent PageContent { get; }

        public static AndroidStubbedLoginTermsAndConditionsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginTermsAndConditionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}