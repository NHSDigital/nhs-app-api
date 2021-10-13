using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public sealed class AndroidAppointmentsPage
    {
        public AndroidFullNavigation Navigation { get; }
        public AppointmentsPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidAppointmentsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new AppointmentsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static AndroidAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidAppointmentsPage(driver);
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

        public void NavigateToGpSurgeryAppointments() => PageContent.NavigateToGpSurgeryAppointments();

        public void KeyboardNavigateToGpSurgeryAppointments() =>
            PageContent.KeyboardNavigateToGpSurgeryAppointments(KeyboardPageContentNavigation);

        public void KeyboardNavigateToAdditionalGpServices() =>
            PageContent.KeyboardNavigateToAdditionalGpServices(KeyboardPageContentNavigation);

        public void KeyboardNavigateToHospitalAndOtherAppointments() =>
            PageContent.KeyboardNavigateToHospitalAndOtherAppointments(KeyboardPageContentNavigation);

        // Focus needs to be set on webview on page load, NHSO-14668 and tabbing functionality needs to be updated before this can be removed.
        public AndroidAppointmentsPage TabIntoFocus()
        {
            _driver.SendKey(AndroidKeyCode.Keycode_TAB);
            return this;
        }
    }
}
