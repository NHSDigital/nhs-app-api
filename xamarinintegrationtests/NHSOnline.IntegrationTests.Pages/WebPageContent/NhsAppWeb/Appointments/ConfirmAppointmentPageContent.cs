using System;
using System.Globalization;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments
{
    public class ConfirmAppointmentPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ConfirmAppointmentPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Confirm your GP appointment");

        public  WebButton BookingButton => WebButton.WithText(_interactor, "Confirm and book appointment");

        public WebTextarea AppointmentReason => WebTextarea.WithId(_interactor, "reasonText");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void InsertReason() => AppointmentReason.InsertText("I have a sore head");

        public void ClickBook() => BookingButton.Click();
    }
}