using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class AppointmentsTemporaryProblemPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AppointmentsTemporaryProblemPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Cannot show GP appointments");
        private WebText UnableToBookText => WebText.WithTagAndText(_interactor,
            "p", "You can try loading GP appointments again.");
        private WebButton TryAgainButton =>  WebButton.WithText(_interactor,"Try again");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
            UnableToBookText.AssertVisible();
            TryAgainButton.AssertVisible();
        }

        public void TryAgain() => TryAgainButton.Click();
    }
}