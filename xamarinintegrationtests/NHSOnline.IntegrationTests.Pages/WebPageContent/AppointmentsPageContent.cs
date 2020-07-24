using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class AppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Appointments");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public AppointmentsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}
