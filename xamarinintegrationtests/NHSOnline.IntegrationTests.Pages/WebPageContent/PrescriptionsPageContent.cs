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

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Prescriptions");

        private WebMenuItem HospitalAndOtherPrescriptionsMenuItem => WebMenuItem.WithTitle(_interactor, "Hospital and other prescriptions");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements() => TitleText.AssertVisible();

        public void NavigateToHospitalAndOtherPrescriptions() => HospitalAndOtherPrescriptionsMenuItem.Click();
    }
}
