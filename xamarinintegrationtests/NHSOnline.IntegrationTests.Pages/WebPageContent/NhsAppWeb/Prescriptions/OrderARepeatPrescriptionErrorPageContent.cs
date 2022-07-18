using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class OrderARepeatPrescriptionErrorPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OrderARepeatPrescriptionErrorPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebButton TryAgainButton => WebButton.WithText(_interactor, "Try again");

        private WebText ErrorTitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Sorry, there is a problem getting your repeat prescription information");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);

            ErrorTitleText.AssertVisible();
        }

        public void ClickTryAgainButton() => TryAgainButton.Click();
    }
}