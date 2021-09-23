using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.LegalAndCookies;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.LegalAndCookies
{
    public sealed class IOSLegalAndCookiesPage
    {
        private readonly IIOSDriverWrapper _driver;

        private LegalAndCookiesPageContent PageContent { get; }

        private IOSLegalAndCookiesPage(IIOSDriverWrapper driver)
        {
            _driver = driver;

            PageContent = new LegalAndCookiesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSLegalAndCookiesPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSLegalAndCookiesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        private IOSLink ManageCookiesMenuItem => IOSLink.WithText(_driver, "Manage cookies");

        public void NavigateToManageCookies() => ManageCookiesMenuItem.Touch();
    }
}