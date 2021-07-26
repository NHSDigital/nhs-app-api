using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidStubbedNetCompanyInternalPage
    {
        private AndroidStubbedNetCompanyInternalPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new NetCompanyInternalPageContent(driver.Web.WebIntegrationWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public NetCompanyInternalPageContent PageContent { get; }

        public static AndroidStubbedNetCompanyInternalPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedNetCompanyInternalPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}