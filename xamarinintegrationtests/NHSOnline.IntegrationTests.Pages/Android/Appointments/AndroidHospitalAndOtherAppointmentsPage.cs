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
        private AndroidFullNavigation Navigation { get; }
        public HospitalAndOtherAppointmentsPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidHospitalAndOtherAppointmentsPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new HospitalAndOtherAppointmentsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public static AndroidHospitalAndOtherAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidHospitalAndOtherAppointmentsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }

        public void KeyboardNavigateToViewAppointments() => PageContent.KeyboardNavigateViewAppointments(KeyboardPageContentNavigation);
    }
}