using System.Collections.Generic;
using NHSOnline.HttpMocks.SecondaryCare;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder
{
    public sealed class SecondaryCareSummaryPageContent
    {
        private readonly IWebInteractor _interactor;
        private readonly WayfinderErrorType _errorType;
        private readonly int _totalReferralsOrAppointments;
        private readonly int _totalConfirmedAppointments;
        private readonly int _totalReferralsInReview;

        internal SecondaryCareSummaryPageContent(IWebInteractor interactor,
            WayfinderErrorType errorType,
            int totalReferralsOrAppointments,
            int totalConfirmedAppointments,
            int totalReferralsInReview)
        {
            _interactor = interactor;
            _errorType = errorType;
            _totalReferralsOrAppointments = totalReferralsOrAppointments;
            _totalConfirmedAppointments = totalConfirmedAppointments;
            _totalReferralsInReview = totalReferralsInReview;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Referrals, hospital and other appointments");

        private WebText ErrorTitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Cannot view or manage referrals and appointments");

        private WebText GeneralErrorSubtext => WebText.WithTagAndText(
            _interactor,
            "p",
            "Try again. If the problem continues and you need to access your referrals " +
                "or appointments now you may be able to do this using other services.");

        private WebText UnderSixteenErrorSubtext => WebText.WithTagAndText(
            _interactor,
            "p",
            "If you're aged 15 or under you may be able to access " +
                "your referrals and appointments using other services.");

        private WebText NoOtherServicesShowingSubtext => WebText.WithTagAndText(
            _interactor,
            "p",
            "If no other services are showing, you'll need to contact the relevant organisation " +
                "or healthcare provider for more information.");

        private WebText BookOrManageReferralsOrAppointmentsHeader => WebText.WithTagAndText(
            _interactor,
            "h2",
            $"You have {_totalReferralsOrAppointments} referrals or appointments you need to action");

        private WebText AppointmentsHeader => WebText.WithTagAndText(
            _interactor,
            "h2",
            $"You have {_totalConfirmedAppointments} upcoming appointments");

        private WebText InReviewReferralsHeader => WebText.WithTagAndText(
            _interactor,
            "h2",
            $"You have {_totalReferralsInReview} referrals being reviewed");

        public WebMenuItem MissingOrIncorrectReferralsOrAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "What to do if a referral or appointment is missing, incorrect or has not been cancelled");

        public WebMenuItem AppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "What to do if an appointment is missing, incorrect or has not been changed or cancelled");

        public WebMenuItem InReviewReferralsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "What to do if a referral being reviewed by a clinic is missing or incorrect");

        public void AssertPageElements()
        {
            switch (_errorType)
            {
                case WayfinderErrorType.generalError:
                    ErrorTitleText.AssertVisible();
                    GeneralErrorSubtext.AssertVisible();
                    break;
                case WayfinderErrorType.underSixteen:
                    ErrorTitleText.AssertVisible();
                    UnderSixteenErrorSubtext.AssertVisible();
                    NoOtherServicesShowingSubtext.AssertVisible();
                    break;
                case WayfinderErrorType.none:
                default:
                    TitleText.AssertVisible();
                    BookOrManageReferralsOrAppointmentsHeader.AssertVisible();
                    MissingOrIncorrectReferralsOrAppointmentsMenuItem.AssertVisible();
                    AppointmentsHeader.AssertVisible();
                    AppointmentsMenuItem.AssertVisible();
                    InReviewReferralsHeader.AssertVisible();
                    InReviewReferralsMenuItem.AssertVisible();
                    break;
            }
        }

        public void ScrollToAppointmentsHeader() =>
            AppointmentsHeader.ScrollTo();
    }
}

