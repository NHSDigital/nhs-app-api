using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.LegalAndCookies.ManageCookies;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.LegalAndCookies.ManageCookies
{
    public sealed class IOSManageCookiesPage
    {
        private ManageCookiesPageContent PageContent { get; }
        private IOSManageCookiesPageContent IOSPageContent { get; }

        private IOSManageCookiesPage(IIOSDriverWrapper driver)
        {
            PageContent = new ManageCookiesPageContent(driver.Web.NhsAppLoggedInWebView());
            IOSPageContent = new IOSManageCookiesPageContent(driver);
        }

        public static IOSManageCookiesPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSManageCookiesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void ToggleCookies() => IOSPageContent.ToggleOptionalCookies();
    }
}