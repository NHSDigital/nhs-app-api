using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder
{
    public sealed class WayfinderHelpPageContent
    {
        private readonly IWebInteractor _interactor;

        internal WayfinderHelpPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText PageTitle => WebText.WithTagAndText(_interactor,
            "h1",
            "What to do if something is missing, incorrect or has not been changed or cancelled");

        private WebText ReferralsHelpTitle => WebText.WithTagAndText(_interactor,
            "h2",
            "Referrals");

        private WebLinkExpander MissingReferralsExpanderTitle => WebLinkExpander.WithText(_interactor,
            "Missing referrals");

        private WebText MissingReferralsText => WebText.WithTagAndText(_interactor,
            "p",
            "You may have referrals not shown that are in other services. Contact the healthcare provider that referred you for more information.");

        private WebLinkExpander IncorrectOrCancelledReferralsExpanderTitle => WebLinkExpander.WithText(_interactor,
            "Incorrect or cancelled referrals");

        private WebText CancelledReferralsContactText => WebText.WithTagAndText(_interactor,
            "p",
            "If you have cancelled a referral and it's still showing, you need to contact the healthcare provider that referred you.");

        private WebText AppointmentsHelpTitle => WebText.WithTagAndText(_interactor,
            "h2",
            "Appointments");

        private WebLinkExpander MissingAppointmentsExpanderTitle => WebLinkExpander.WithText(_interactor,
            "Missing appointments");

        private WebText MissingAppointmentsTextOne => WebText.WithTagAndText(_interactor,
            "p",
            "You may have appointments not shown that are in other services. " +
                "Contact the relevant organisation or healthcare provider for more information.");

        private WebText MissingAppointmentsTextTwo => WebText.WithTagAndText(_interactor,
            "p",
            "If you're aged 16 to 17, you may not be able to view or manage some of your hospital appointments. " +
                "This is because some NHS Trusts require you to be aged 18 or over to access these appointments.");

        private WebLinkExpander IncorrectChangedCancelledAppointmentsExpanderTitle => WebLinkExpander.WithText(_interactor,
            "Incorrect, changed or cancelled appointments");

        private WebText IncorrectChangedCancelledAppointmentsTextOne => WebText.WithTagAndText(_interactor,
            "p",
            "You may have requested to change or permanently cancel a confirmed appointment. This request may not automatically be accepted.");

        private WebText IncorrectChangedCancelledAppointmentsTextTwo => WebText.WithTagAndText(_interactor,
            "p",
            "The appointment will show as pending while the request is reviewed by the relevant organisation or healthcare provider it's booked with.");

        private WebText IncorrectChangedCancelledAppointmentsTextThree => WebText.WithTagAndText(_interactor,
            "p",
            "If the request to change or cancel the appointment is not accepted it will still show as booked in your confirmed appointments.");

        private WebText IncorrectChangedCancelledAppointmentsTextFour => WebText.WithTagAndText(_interactor,
            "p",
            "If the cancellation is accepted the appointment will show as cancelled in your confirmed appointments.");

        private WebLink BackButton => WebLink.WithText(_interactor, "Back");

        public void AssertPageElements()
        {
            //click each expander title
            ExpandElements();

            PageTitle.AssertVisible();
            ReferralsHelpTitle.AssertVisible();
            MissingReferralsExpanderTitle.AssertVisible();
            MissingReferralsText.AssertVisible();
            IncorrectOrCancelledReferralsExpanderTitle.AssertVisible();
            CancelledReferralsContactText.AssertVisible();
            AppointmentsHelpTitle.AssertVisible();
            MissingAppointmentsExpanderTitle.AssertVisible();
            MissingAppointmentsTextOne.AssertVisible();
            MissingAppointmentsTextTwo.AssertVisible();
            IncorrectChangedCancelledAppointmentsExpanderTitle.AssertVisible();
            IncorrectChangedCancelledAppointmentsTextOne.AssertVisible();
            IncorrectChangedCancelledAppointmentsTextTwo.AssertVisible();
            IncorrectChangedCancelledAppointmentsTextThree.AssertVisible();
            IncorrectChangedCancelledAppointmentsTextFour.AssertVisible();
            BackButton.AssertVisible();
        }

        public void NavigateViaBackButton() =>
            BackButton.Click();

        private void ExpandElements()
        {
            MissingReferralsExpanderTitle.Toggle();
            MissingAppointmentsExpanderTitle.Toggle();
            IncorrectOrCancelledReferralsExpanderTitle.Toggle();
            IncorrectChangedCancelledAppointmentsExpanderTitle.Toggle();
        }
    }
}