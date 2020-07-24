using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class MyRecordPageContent
    {
        private readonly IWebInteractor _interactor;

        internal MyRecordPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Health records");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public MyRecordPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
