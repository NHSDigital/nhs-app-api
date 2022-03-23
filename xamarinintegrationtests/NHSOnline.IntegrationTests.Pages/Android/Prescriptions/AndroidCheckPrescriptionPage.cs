using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public class AndroidCheckPrescriptionPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidFullNavigation Navigation { get; }

        public CheckPrescriptionPageContent PageContent { get; }

        private AndroidCheckPrescriptionPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new CheckPrescriptionPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidCheckPrescriptionPage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidCheckPrescriptionPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidCheckPrescriptionPage));
            }

            return page;
        }
    }
}