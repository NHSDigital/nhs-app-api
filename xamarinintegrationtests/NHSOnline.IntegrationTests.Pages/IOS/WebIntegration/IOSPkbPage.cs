using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSPkbPage
    {
        private IOSPkbPage(IIOSDriverWrapper driver, string phrPath)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new PkbPageContent(driver.Web.WebIntegrationWebView(), phrPath);
        }

        public IOSFullNavigation Navigation { get; }

        private PkbPageContent PageContent { get; }

        public static IOSPkbPage AssertOnPage(IIOSDriverWrapper driver, string phrPath)
        {
            // Extending timeout to allow page to finish reloading
            using var extendedTimeout = ExtendedTimeout.FromSeconds(10);

            var page = new IOSPkbPage(driver, phrPath);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void NavigateToCalendar() => PageContent.NavigateToCalendar();

        public void NavigateToGoToPage() => PageContent.NavigateToGoToPage();

        public void NavigateToOpenBrowserOverlay() => PageContent.NavigateToOpenBrowserOverlay();

        public void NavigateToOpenExternalBrowser() => PageContent.NavigateToOpenExternalBrowser();

        public void NavigateToNativeBackAction() => PageContent.NavigateToNativeBackAction();

        public void NavigateToFileUpload() => PageContent.NavigateToFileUpload();

        public void NavigateToDocumentDownload() => PageContent.NavigateToDocumentDownload();

        public void NavigateToLocationServices() => PageContent.NavigateToLocationServices();

        public IOSPkbPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }
    }
}