using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class StubbedLoginUpliftPageContent
    {
        private readonly IWebInteractor _interactor;

        internal StubbedLoginUpliftPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;
        
        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "NHS Login - Uplift");

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
        }
    }
}