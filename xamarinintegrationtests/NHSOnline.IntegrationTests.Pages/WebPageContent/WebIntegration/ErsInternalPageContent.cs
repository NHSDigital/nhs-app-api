using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class ErsInternalPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ErsInternalPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "eRS Internal Page");

        private WebLink BackLink => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public ErsInternalPageContent Back()
        {
            BackLink.Click();
            return this;
        }
    }
}