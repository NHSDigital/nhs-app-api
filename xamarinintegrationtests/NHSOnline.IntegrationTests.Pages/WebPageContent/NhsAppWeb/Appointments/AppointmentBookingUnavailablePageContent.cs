using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class AppointmentBookingUnavailablePageContent
    {
        private readonly IWebInteractor _interactor;

        internal AppointmentBookingUnavailablePageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Cannot book GP appointment");

        private WebText UnableToBookText => WebText.WithTagAndText(_interactor,
            "p", "You are not currently able to book and manage GP appointments online.");

        private WebLink ReportAProblemLink => WebLink.WithText(_interactor, "Report a problem");

        private WebText WhatYouCanDoNext => WebText.WithTagAndText(_interactor,
            "h2", "What you can do next");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
            UnableToBookText.AssertVisible();
            ReportAProblemLink.AssertVisible();
            WhatYouCanDoNext.AssertVisible();
        }

        public void ReportAProblem() => ReportAProblemLink.Click();
    }
}