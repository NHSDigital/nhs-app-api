using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidPkbPage
    {
        //private const string PhrPath = "/diary/listAppointments.action";

        private AndroidPkbPage(IAndroidDriverWrapper driver, string phrPath)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PkbPageContent(driver.Web(WebViewContext.PkbWebIntegration), phrPath);
        }

        private AndroidFullNavigation Navigation { get; }

        private PkbPageContent PageContent { get; }

        public static AndroidPkbPage AssertOnPage(IAndroidDriverWrapper driver, string phrPath)
        {
            var page = new AndroidPkbPage(driver, phrPath);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidPkbPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}