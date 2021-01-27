using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public sealed class HospitalAndOtherAppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal HospitalAndOtherAppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Hospital and other appointments");

        private WebMenuItem BookOrCancelYourReferralAppointmentMenuItem => WebMenuItem.WithTitle(_interactor, "Book or cancel your referral appointment");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public HospitalAndOtherAppointmentsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }

        public HospitalAndOtherAppointmentsPageContent BookOrCancelYourReferralAppointment()
        {
            BookOrCancelYourReferralAppointmentMenuItem.Click();
            return this;
        }
    }
}