using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public sealed class AndroidHospitalAndOtherAppointmentsPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidHospitalAndOtherAppointmentsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new HospitalAndOtherAppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        internal AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            GetAllKeyboardHomeNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        private AndroidFullNavigation Navigation { get; }

        public HospitalAndOtherAppointmentsPageContent PageContent { get; }

        public static AndroidHospitalAndOtherAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidHospitalAndOtherAppointmentsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }

        public void KeyboardNavigateToViewAppointments() =>
            PageContent.KeyboardNavigateViewAppointments(KeyboardPageContentNavigation);
    }
}