using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class PrescriptionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal PrescriptionsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Prescriptions");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public PrescriptionsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
