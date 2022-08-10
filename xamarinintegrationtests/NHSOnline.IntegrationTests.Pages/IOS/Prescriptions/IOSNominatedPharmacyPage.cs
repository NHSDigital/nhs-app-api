using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public sealed class IOSNominatedPharmacyPage
    {
        private IOSNominatedPharmacyPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NominatedPharmacyPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public NominatedPharmacyPageContent PageContent { get; }

        public static IOSNominatedPharmacyPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSNominatedPharmacyPage(driver);

            // API calls required to load this page
            using var timeout = ExtendedTimeout.FromSeconds(20);

            page.PageContent.AssertPageElements();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSNominatedPharmacyPage));
            }

            return page;
        }
    }
}

