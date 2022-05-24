using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More
{
    public class LinkedProfilesTemporaryProblemPageContent
    {
        private readonly IWebInteractor _interactor;

        internal LinkedProfilesTemporaryProblemPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Sorry, there is a problem getting your linked profiles information");

        private WebText UnableToBookText => WebText.WithTagAndText(_interactor,
            "p", "You are not currently able to access linked profiles.");

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