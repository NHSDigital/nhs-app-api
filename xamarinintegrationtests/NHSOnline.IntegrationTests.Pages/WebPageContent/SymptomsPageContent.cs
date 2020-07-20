using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class SymptomsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal SymptomsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => new WebText(_interactor, "h1", "Symptoms");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public SymptomsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
