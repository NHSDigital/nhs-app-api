using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class PrescriptionConfirmedPageContent
    {
        private readonly IWebInteractor _interactor;

        internal PrescriptionConfirmedPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Your prescription has been ordered");

        internal void AssertOnPage() => TitleText.AssertVisible();
    }
}