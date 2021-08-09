using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class LocationServicesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal LocationServicesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Web Integration Functionality - Location Services");

        private WebText LatitudeText => WebText.WithTagAndText(_interactor, "li", "Latitude: 40.73061");

        private WebText LongitudeText => WebText.WithTagAndText(_interactor, "li", "Longitude: -73.935242");

        private WebText ErrorText => WebText.WithTagAndText(_interactor, "div", "Error getting geolocation");

        private WebButton CheckLocationButton => WebButton.WithText(_interactor, "Show location");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void ShowLocation() => CheckLocationButton.Click();

        public void AssertLocationPresented()
        {
            LatitudeText.AssertVisible();
            LongitudeText.AssertVisible();
        }

        public void AssertErrorTextPresented() => ErrorText.AssertVisible();
    }
}