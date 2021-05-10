using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSPkbViewAppointmentsPage
    {
        private const string PhrPath = "/diary/listAppointments.action";

        private IOSPkbViewAppointmentsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PkbPageContent(driver.Web(WebViewContext.PkbWebIntegration), PhrPath);
        }

        private IOSFullNavigation Navigation { get; }

        private PkbPageContent PageContent { get; }

        public static IOSPkbViewAppointmentsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSPkbViewAppointmentsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
        }
    }
}