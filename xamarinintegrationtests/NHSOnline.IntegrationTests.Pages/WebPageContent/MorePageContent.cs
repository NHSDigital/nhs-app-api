using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class MorePageContent
    {
        private readonly IWebInteractor _interactor;

        internal MorePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => new WebText(_interactor, "h1", "More");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public MorePageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
