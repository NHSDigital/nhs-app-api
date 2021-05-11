using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class AppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Appointments");

        private WebMenuItem GpSurgeryAppointmentsMenuItem => WebMenuItem.WithTitle(_interactor, "GP surgery appointments");

        private WebMenuItem AdditionalGpServicesMenuItem => WebMenuItem.WithTitle(_interactor, "Additional GP services");

        private WebMenuItem HospitalAndOtherAppointmentsMenuItem => WebMenuItem.WithTitle(_interactor, "Hospital and other appointments");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GpSurgeryAppointmentsMenuItem,
            AdditionalGpServicesMenuItem,
            HospitalAndOtherAppointmentsMenuItem,
        };

        public IEnumerable<IFocusable> FocusableElementsNoOlc => new IFocusable[]
        {
            GpSurgeryAppointmentsMenuItem,
            HospitalAndOtherAppointmentsMenuItem,
        };

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public void AssertPageElements()
        {
            Title.AssertVisible();
        }

        public void HospitalAndOtherAppointments()
        {
            HospitalAndOtherAppointmentsMenuItem.Click();
        }

        public void KeyboardNavigateToHospitalAndOtherAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(HospitalAndOtherAppointmentsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GpSurgeryAppointmentsMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
