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

        private WebPanel ErrorPanel => WebPanel.WithTitle(_interactor, "Error");

        private WebText UnableToBookText => ErrorPanel.ContainingTagWithText(
            "p", "You are not currently able to access linked profiles.");

        private WebText TemporaryProblemText => ErrorPanel.ContainingTagWithText(
            "p", "This may be a temporary problem.");

        private WebButton TryAgainButton => ErrorPanel.ContainingButtonWithText("Try again");

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