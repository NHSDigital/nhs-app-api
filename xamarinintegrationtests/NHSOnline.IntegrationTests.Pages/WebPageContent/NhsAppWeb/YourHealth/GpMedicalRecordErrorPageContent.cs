using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class GpMedicalRecordErrorPageContent
    {
        private readonly IWebInteractor _interactor;

        internal GpMedicalRecordErrorPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Cannot show GP health record");

        private WebText PostTryAgainTitleText => WebText.WithTagAndText(_interactor,
            "h1", "Cannot show GP health record");

        private WebText UnableToBookText => WebText.WithTagAndText(
            _interactor,
            "p",
            "You are not currently able to view your GP health record online.");

        private WebText TemporaryProblemText => WebText.WithTagAndText(
            _interactor,
            "p",
            "This may be a temporary problem. You can try loading your GP health record again.");

        private WebButton TryAgainButton => WebButton.WithText(
            _interactor,
            "Try again");

        private WebLink ReportAProblemLink => WebLink.WithText(_interactor, "Report a problem");

        internal void AssertOnPage(bool postTryAgain = false)
        {
            if (postTryAgain)
            {
                PostTryAgainTitleText.AssertVisible();
            }
            else
            {
                TitleText.AssertVisible();
                UnableToBookText.AssertVisible();
                TemporaryProblemText.AssertVisible();
                TryAgainButton.AssertVisible();
            }
        }

        public void TryAgain() => TryAgainButton.Click();

        public void ReportAProblem() => ReportAProblemLink.Click();
    }
}
