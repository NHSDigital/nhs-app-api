using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.LegalAndCookies.ManageCookies;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings.LegalAndCookies.ManageCookies
{
    public class AndroidManageCookiesPage
    {
        private ManageCookiesPageContent PageContent { get; }

        private AndroidManageCookiesPage(IAndroidDriverWrapper driver)
        {
            PageContent = new ManageCookiesPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidManageCookiesPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidManageCookiesPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void ToggleOptionalCookies() => PageContent.ToggleOptionalCookies();

        public void ToggleOptionalCookiesOn() => PageContent.ToggleOptionalCookiesOn();
    }
}