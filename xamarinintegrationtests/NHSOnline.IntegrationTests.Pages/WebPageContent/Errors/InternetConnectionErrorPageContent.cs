using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.Errors
{
    public class InternetConnectionErrorPageContent
    {
        private readonly IWebInteractor _interactor;

        internal InternetConnectionErrorPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Internet connection error");

        private WebPanel ErrorPanel => WebPanel.WithTitle(_interactor, "Error");

        private WebText ProblemWithInternetConnection => ErrorPanel.ContainingTagWithText(
            "p", "There is a problem with your internet connection");

        private WebText CheckConnectionAndTryAgain => ErrorPanel.ContainingTagWithText(
            "p", "Check your connection and try again. If the problem continues and you need to book an appointment " +
                 "or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to " +
                 "111.nhs.uk or call 111.");

        public void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            ProblemWithInternetConnection.AssertVisible();
            CheckConnectionAndTryAgain.AssertVisible();
        }
    }
}