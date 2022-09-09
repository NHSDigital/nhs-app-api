using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.LinkedProfile
{
    public class LinkedProfilesPageContent
    {
        private readonly IWebInteractor _interactor;

        private EmisPatient _linkedProfile;

        internal LinkedProfilesPageContent(IWebInteractor interactor, EmisPatient linkedProfile)
        {
            _interactor = interactor;
            _linkedProfile = linkedProfile;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", "Linked profiles");

        private WebText YouCanAccessServicesText => WebText.WithTagAndText(_interactor,
            "p",
            "You can access services in the app for the following people." );

        private WebMenuItem LinkedProfileMenuItem =>
            WebMenuItem.WithTitle(_interactor, $"{_linkedProfile.PersonalDetails.Name.GivenName + " " + _linkedProfile.PersonalDetails.Name.FamilyName}");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public LinkedProfilesPageContent AssertPageElements()
        {
            LinkedProfileMenuItem.AssertVisible();
            YouCanAccessServicesText.AssertVisible();
            return this;
        }

        public void NavigateToLinkedProfile() => LinkedProfileMenuItem.Click();
    }
}