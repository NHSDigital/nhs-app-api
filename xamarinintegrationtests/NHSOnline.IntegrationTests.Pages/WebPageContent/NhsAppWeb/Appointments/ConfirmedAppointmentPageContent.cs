using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class ConfirmedAppointmentPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ConfirmedAppointmentPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Your GP appointment has been booked");

        internal void AssertOnPage() => TitleText.AssertVisible();
    }
}