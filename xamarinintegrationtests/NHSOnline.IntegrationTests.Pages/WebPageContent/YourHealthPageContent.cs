using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Your health");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public YourHealthPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
