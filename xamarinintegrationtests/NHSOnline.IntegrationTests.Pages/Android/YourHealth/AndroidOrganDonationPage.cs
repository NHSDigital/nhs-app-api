using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidOrganDonationPage
    {
        private AndroidOrganDonationPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new OrganDonationPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public AndroidFullNavigation Navigation { get; }

        public OrganDonationPageContent PageContent { get; }

        public static AndroidOrganDonationPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidOrganDonationPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }
    }
}