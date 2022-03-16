using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class BookAppointmentsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal BookAppointmentsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Book a GP appointment");

        private WebSelect TypeSelect => WebSelect.WithId(_interactor, "type");

        private WebLinkExpander AppointmentExpander => WebLinkExpander.WithText(_interactor, "Wednesday 10 March 2100");

        public WebText AppointmentBookingText => WebText.WithTagAndText(_interactor, "span", "8:00am");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void ChooseType() => TypeSelect.Click();

        public void Toggle() => AppointmentExpander.Toggle();

        public void ClickLink() => AppointmentBookingText.Click();
    }
}