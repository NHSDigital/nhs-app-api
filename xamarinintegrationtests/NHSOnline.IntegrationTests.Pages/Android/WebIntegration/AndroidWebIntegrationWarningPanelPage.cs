using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidWebIntegrationWarningPanelPage
    {
        private AndroidWebIntegrationWarningPanelPage(IAndroidDriverWrapper driver, string pageTitle)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new WebIntegrationWarningPanelPageContent(driver.Web(WebViewContext.NhsApp), pageTitle);
        }

        private AndroidFullNavigation Navigation { get; }

        public WebIntegrationWarningPanelPageContent PageContent { get; }

        public static AndroidWebIntegrationWarningPanelPage AssertOnPage(IAndroidDriverWrapper driver, string pageTitle)
        {
            var page = new AndroidWebIntegrationWarningPanelPage(driver, pageTitle);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidWebIntegrationWarningPanelPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}