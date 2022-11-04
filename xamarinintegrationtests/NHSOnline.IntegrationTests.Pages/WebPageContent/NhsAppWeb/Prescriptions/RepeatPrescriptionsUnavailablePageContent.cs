using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class RepeatPrescriptionsUnavailablePageContent
    {
        private readonly IWebInteractor _interactor;

        internal RepeatPrescriptionsUnavailablePageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Repeat prescriptions unavailable");

        private WebText NotAbleToOrderRepeat => WebText.WithTagAndText(_interactor,
            "p", "You are not currently able to order repeat prescriptions online.");

        public void AssertPageElements()
        {
            // API call required to load this link
            using var timeout = ExtendedTimeout.FromSeconds(10);

            TitleText.AssertVisible();
            NotAbleToOrderRepeat.AssertVisible();
        }
    }
}