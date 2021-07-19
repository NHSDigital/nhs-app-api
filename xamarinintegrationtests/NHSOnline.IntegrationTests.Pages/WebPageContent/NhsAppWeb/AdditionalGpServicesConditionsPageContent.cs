using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class AdditionalGpServicesConditionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AdditionalGpServicesConditionsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "p",
            "To ensure we ask you relevant questions, choose your condition.");

        internal void AssertOnPage() => TitleText.AssertVisible();
    }
}