using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class SubstraktPageContent
    {
        private readonly IWebInteractor _interactor;

        internal SubstraktPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Substrakt First Domain Internal Page");

        private WebLink ExternalDomainLink => WebLink.WithText(_interactor, "External domain");

        internal SubstraktPageContent AssertOnPage()
        {
            TitleText.AssertVisible();
            return this;
        }

        public void NavigateToExternalDomain()
        {
            ExternalDomainLink.Click();
        }
    }
}