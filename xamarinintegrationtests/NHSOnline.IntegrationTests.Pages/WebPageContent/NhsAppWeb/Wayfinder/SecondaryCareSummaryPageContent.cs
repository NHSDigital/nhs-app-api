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
        private IEnumerable<IFocusable>? _focusableElements;
        private readonly WayfinderErrorType _errorType;

        internal SecondaryCareSummaryPageContent(IWebInteractor interactor, WayfinderErrorType errorType)
        {
            _interactor = interactor;
            _errorType = errorType;
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
            "Try again. If the problem continues and you need to access your referrals "
            + "or appointments now you may be able to do this using other services.");

        private WebText UnderSixteenErrorSubtext => WebText.WithTagAndText(
            _interactor,
            "p",
            "If you're aged 15 or under you may be able to access" +
            "  your referrals and appointments using other services.");

        private WebText NoOtherServicesShowingSubtext => WebText.WithTagAndText(
            _interactor,
            "p",
            "If no other services are showing, you'll need to contact the relevant organisation" +
            " or healthcare provider for more information.");

        private WebText BookOrManageReferralsOrAppointmentsHeader => WebText.WithTagAndText(
            _interactor,
            "h2",
            "Book or manage your referrals and appointments");

        private WebText ConfirmedAppointmentsHeader => WebText.WithTagAndText(
            _interactor,
            "h2",
            "Confirmed Appointments");

        private WebText InReviewReferralsHeader => WebText.WithTagAndText(
            _interactor,
            "h2",
            "In review");

        public WebMenuItem MissingOrIncorrectReferralsOrAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Missing or incorrect referrals or appointments");

        public WebMenuItem ConfirmedAppointmentsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Missing or incorrect confirmed appointments");

        public WebMenuItem InReviewReferralsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Missing or incorrect referrals in review");

        public  WebButton ReadyToConfirmAppointmentDeepLinkButton => WebButton.WithText(
            _interactor,
            "Contact the clinic to confirm"
            );

        public WebLink CancelledAppointmentDeepLinkButton => WebLink.WithText(
            _interactor,
            "View this appointment"
        );

        public IEnumerable<IFocusable> FocusableElements
        {
            get
            {
                _focusableElements = new IFocusable[]
                {
                    MissingOrIncorrectReferralsOrAppointmentsMenuItem,
                    ConfirmedAppointmentsMenuItem,
                    InReviewReferralsMenuItem,
                };

                return _focusableElements;
            }
        }

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
                    ConfirmedAppointmentsHeader.AssertVisible();
                    ConfirmedAppointmentsMenuItem.AssertVisible();
                    InReviewReferralsHeader.AssertVisible();
                    InReviewReferralsMenuItem.AssertVisible();
                    break;
            }
        }

        public void ScrollToConfirmedAppointmentsHeader() =>
            ConfirmedAppointmentsHeader.ScrollTo();
    }
}

