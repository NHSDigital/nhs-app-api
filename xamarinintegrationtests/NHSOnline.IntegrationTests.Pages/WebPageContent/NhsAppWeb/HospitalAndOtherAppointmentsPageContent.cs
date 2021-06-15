using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public sealed class HospitalAndOtherAppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal HospitalAndOtherAppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Hospital and other appointments");

        private WebMenuItem BookOrCancelYourReferralAppointmentMenuItem => WebMenuItem.WithTitle(_interactor, "Book or cancel your referral appointment");

        private WebMenuItem ViewYourAppointmentsMenuItem => WebMenuItem.WithTitle(_interactor, "View appointments");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            BookOrCancelYourReferralAppointmentMenuItem,
            ViewYourAppointmentsMenuItem,
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements() => TitleText.AssertVisible();

        public void NavigateToBookOrCancelYourReferralAppointment() => BookOrCancelYourReferralAppointmentMenuItem.Click();

        public void NavigateToViewAppointments() => ViewYourAppointmentsMenuItem.Click();

        public void ClickBackBreadcrumb() => BackBreadcrumb.Click();

        public void KeyboardNavigateViewAppointments(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(ViewYourAppointmentsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabTo(menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}