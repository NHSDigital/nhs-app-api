using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidPkbViewAppointmentsPage
    {
        private const string PhrPath = "/diary/listAppointments.action";

        private AndroidPkbViewAppointmentsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PkbPageContent(driver.Web(WebViewContext.PkbWebIntegration), PhrPath);
        }

        private AndroidFullNavigation Navigation { get; }

        private PkbPageContent PageContent { get; }

        public static AndroidPkbViewAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidPkbViewAppointmentsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidPkbViewAppointmentsPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}