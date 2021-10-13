using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.Prescriptions
{
    public sealed class AndroidPrescriptionsPage
    {
        private readonly IAndroidDriverWrapper _driver;

        public AndroidFullNavigation Navigation { get; }

        public PrescriptionsPageContent PageContent { get; }

        private AndroidPrescriptionsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new PrescriptionsPageContent(driver.Web.NhsAppLoggedInWebView());
            _driver = driver;
        }

        public static AndroidPrescriptionsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidPrescriptionsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardHomeNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidPrescriptionsPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }

        public void KeyboardNavigateToOrderARepeatPrescriptionPage() =>
            PageContent.KeyboardNavigateToOrderARepeatPrescription(KeyboardPageContentNavigation);

        public void KeyboardNavigateToViewYourOrders() =>
            PageContent.KeyboardNavigateToViewYourOrders(KeyboardPageContentNavigation);

        public void KeyboardNavigateToPkbHospitalAndOtherPrescriptions() =>
            PageContent.KeyboardNavigateToPkbHospitalAndOtherPrescriptions(KeyboardPageContentNavigation);

        public void KeyboardNavigateToPkbCieHospitalAndOtherMedicines() =>
            PageContent.KeyboardNavigateToPkbCieHospitalAndOtherMedicines(KeyboardPageContentNavigation);

        public void KeyboardNavigateToPkbSecondaryCareHospitalAndOtherMedicines() =>
            PageContent.KeyboardNavigateToPkbSecondaryCareHospitalAndOtherMedicines(KeyboardPageContentNavigation);

        public void KeyboardNavigateToPkbMyCareViewHospitalAndOtherMedicines() =>
            PageContent.KeyboardNavigateToPkbMyCareViewHospitalAndOtherMedicines(KeyboardPageContentNavigation);
    }
}
