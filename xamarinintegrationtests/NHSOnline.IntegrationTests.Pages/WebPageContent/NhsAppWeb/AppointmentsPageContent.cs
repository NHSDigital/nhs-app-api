using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public sealed class AppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
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

        private WebText AdditionalGpServicesMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Get sick notes and GP letters or ask about recent tests");

        private WebText EngageAdditionalGpServicesMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Get sick notes and GP letters or ask your GP surgery about something else");

        private WebMenuItem HospitalAndOtherAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other appointments");

        private WebText HospitalAndOtherAppointmentsMenuText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View and manage appointments, like your referral appointments");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GpSurgeryAppointmentsMenuItem,
            AdditionalGpServicesMenuItem,
            HospitalAndOtherAppointmentsMenuItem,
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public AppointmentsPageContent AssertPageElements()
        {
            TitleText.AssertVisible();
            GpSurgeryAppointmentsMenuItem.AssertVisible();
            GpSurgeryAppointmentsMenuText.AssertVisible();
            HospitalAndOtherAppointmentsMenuItem.AssertVisible();
            HospitalAndOtherAppointmentsMenuText.AssertVisible();
            return this;
        }

        public AppointmentsPageContent AssertEngageElements()
        {
            AdditionalGpServicesMenuItem.AssertVisible();
            EngageAdditionalGpServicesMenuText.AssertVisible();
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

        public void KeyboardNavigateToGpSurgeryAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(GpSurgeryAppointmentsMenuItem, navigation);

        public void KeyboardNavigateToAdditionalGpServices(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(AdditionalGpServicesMenuItem, navigation);

        public void KeyboardNavigateToHospitalAndOtherAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(HospitalAndOtherAppointmentsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GpSurgeryAppointmentsMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}