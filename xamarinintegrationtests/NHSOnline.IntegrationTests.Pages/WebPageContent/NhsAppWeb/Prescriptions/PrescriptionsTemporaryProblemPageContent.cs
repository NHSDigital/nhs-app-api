using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class PrescriptionsTemporaryProblemPageContent
    {
        private readonly IWebInteractor _interactor;

        internal PrescriptionsTemporaryProblemPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Cannot access repeat prescriptions");

        private WebText UnableToBookText => WebText.WithTagAndText(_interactor,
            "p", "You are not currently able to order or view repeat prescriptions online.");

        private WebText TemporaryProblemText => WebText.WithTagAndText(_interactor,
            "p", "This may be a temporary problem.");

        private WebButton TryAgainButton => WebButton.WithText(_interactor, "Try again");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
            UnableToBookText.AssertVisible();
            TemporaryProblemText.AssertVisible();
            TryAgainButton.AssertVisible();
        }

        public void TryAgain() => TryAgainButton.Click();
    }
}