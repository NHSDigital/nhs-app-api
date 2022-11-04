using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class PrescriptionConfirmedErrorPageContent
    {
        private readonly IWebInteractor _interactor;

        internal PrescriptionConfirmedErrorPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "There was a problem sending your order");

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
        }
    }
}