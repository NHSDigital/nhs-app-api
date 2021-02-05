using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class AdvicePageContent
    {
        private readonly IWebInteractor _interactor;

        internal AdvicePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Advice");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public AdvicePageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
