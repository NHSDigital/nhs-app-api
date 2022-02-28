using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthTemporaryProblemPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthTemporaryProblemPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Sorry, there is a problem getting your GP health record");

        private WebPanel ErrorPanel => WebPanel.WithTitle(_interactor, "Error");

        private WebText UnableToBookText => ErrorPanel.ContainingTagWithText(
            "p", "You are not currently able to view your GP health record online.");

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