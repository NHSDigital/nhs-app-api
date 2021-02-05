using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class MessagesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal MessagesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Messages");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public MessagesPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
