using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public sealed class AppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;
        private readonly bool _isWayfinderEnabled;
        private List<IFocusable>? _focusableElements;

        internal AppointmentsPageContent(IWebInteractor interactor, bool isWayfinderEnabled = false)
        {
            _interactor = interactor;
            _isWayfinderEnabled = isWayfinderEnabled;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Appointments");

        private WebMenuItem GpSurgeryAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "GP surgery appointments");

        private WebText GpSurgeryAppointmentsMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View and manage appointments at your surgery");

        private WebMenuItem AdditionalGpServicesMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Additional GP services");

        private WebMenuItem NBSAppointmentBookingsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Book or manage a coronavirus (COVID-19) vaccination");

        private WebText AdditionalGpServicesMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Get fit notes (sick notes) and GP letters or ask about recent tests");

        private WebText EngageAdditionalGpServicesMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Get fit notes (sick notes) and GP letters or ask your GP surgery about something else");

        private WebMenuItem HospitalAndOtherAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other appointments");

        private WebText HospitalAndOtherAppointmentsMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View and manage appointments, like your referral appointments");

        private WebMenuItem ReferralsHospitalAndOtherAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Referrals, hospital and other appointments");

        private WebText ReferralsHospitalAndOtherAppointmentsMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View and manage your referrals and appointments");

        public IEnumerable<IFocusable> FocusableElements
        {
            get
            {
                _focusableElements = new List<IFocusable>
                {
                    GpSurgeryAppointmentsMenuItem,
                    AdditionalGpServicesMenuItem,
                    _isWayfinderEnabled
                        ? ReferralsHospitalAndOtherAppointmentsMenuItem
                        : HospitalAndOtherAppointmentsMenuItem,
                    NBSAppointmentBookingsMenuItem
                };

                return _focusableElements;
            }
        }

        internal void AssertOnPage() => TitleText.AssertVisible();

        public AppointmentsPageContent AssertPageElements()
        {
            TitleText.AssertVisible();

            if (_isWayfinderEnabled)
            {
                ReferralsHospitalAndOtherAppointmentsMenuItem.AssertVisible();
                ReferralsHospitalAndOtherAppointmentsMenuText.AssertVisible();
            }
            else
            {
                GpSurgeryAppointmentsMenuItem.AssertVisible();
                GpSurgeryAppointmentsMenuText.AssertVisible();
            }

            return this;
        }

        public AppointmentsPageContent AssertEngageElements()
        {
            AdditionalGpServicesMenuItem.AssertVisible();
            EngageAdditionalGpServicesMenuText.AssertVisible();
            return this;
        }

        public AppointmentsPageContent AssertNbsElements()
        {
            NBSAppointmentBookingsMenuItem.AssertVisible();
            return this;
        }

        public AppointmentsPageContent AssertAdditionalGpServicesElements()
        {
            AdditionalGpServicesMenuItem.AssertVisible();
            AdditionalGpServicesMenuText.AssertVisible();
            return this;
        }

        public void NavigateToGpSurgeryAppointments() => GpSurgeryAppointmentsMenuItem.Click();

        public void NavigateToHospitalAndOtherAppointments() => HospitalAndOtherAppointmentsMenuItem.Click();

        public void NavigateToAdditionalGpServices() => AdditionalGpServicesMenuItem.Click();

        public void NavigateToNbsAdditionsBookings() => NBSAppointmentBookingsMenuItem.Click();

        public void NavigateToSecondaryCareSummaryPage() => ReferralsHospitalAndOtherAppointmentsMenuItem.Click();

        public void KeyboardNavigateToGpSurgeryAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(GpSurgeryAppointmentsMenuItem, navigation);

        public void KeyboardNavigateToAdditionalGpServices(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(AdditionalGpServicesMenuItem, navigation);

        public void KeyboardNavigateToNbsAppointmentBookings(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(NBSAppointmentBookingsMenuItem, navigation);

        public void KeyboardNavigateToHospitalAndOtherAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(HospitalAndOtherAppointmentsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GpSurgeryAppointmentsMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}