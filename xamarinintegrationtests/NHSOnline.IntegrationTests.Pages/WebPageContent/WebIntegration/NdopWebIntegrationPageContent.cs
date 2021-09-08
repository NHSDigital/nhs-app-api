using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class NdopWebIntegrationPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NdopWebIntegrationPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "NDOP Landing page");

        private WebDefinitionTerm NhsNumberDefinitionTerm => WebDefinitionTerm.WithTerm(_interactor, "nhs_number:");

        internal NdopWebIntegrationPageContent AssertOnPage()
        {
            Title.AssertVisible();
            return this;
        }

        public void AssertNhsNumberDisplayedFor(string nhsNumber) => NhsNumberDefinitionTerm.AssertValue(nhsNumber);
    }
}