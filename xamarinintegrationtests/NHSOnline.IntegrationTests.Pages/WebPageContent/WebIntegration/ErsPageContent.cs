using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class ErsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ErsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "eRS Web Integration");

        private WebLink InternalPageLink => WebLink.WithText(_interactor, "Internal Page");

        private WebLink NhsLoginLink => WebLink.WithText(_interactor, "NHS login");

        private WebLink CovidLink => WebLink.WithText(_interactor, "Covid");

        private WebLink NhsAppAppointmentsLink => WebLink.WithText(_interactor, "NHS App Appointments");

        internal void AssertOnPage() => Title.AssertVisible();

        public void NavigateToInternalPage() => InternalPageLink.Click();

        public void NavigateToNhsLogin() => NhsLoginLink.Click();

        public void NavigateToCovid() => CovidLink.Click();

        public void NavigateToNhsAppAppointments() => NhsAppAppointmentsLink.Click();
    }
}
