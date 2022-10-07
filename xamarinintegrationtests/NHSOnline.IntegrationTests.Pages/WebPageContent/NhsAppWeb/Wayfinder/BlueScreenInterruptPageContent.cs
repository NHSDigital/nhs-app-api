using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder
{
    public sealed class BlueScreenInterruptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal BlueScreenInterruptPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View or manage appointment");

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
        }
    }
}