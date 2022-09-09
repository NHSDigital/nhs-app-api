using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.LinkedProfile
{
    public class SwitchToLinkedProfilePageContent
    {
        private readonly IWebInteractor _interactor;

        private EmisPatient _linkedProfile;

        internal SwitchToLinkedProfilePageContent(IWebInteractor interactor, EmisPatient linkedProfile)
        {
            _interactor = interactor;
            _linkedProfile = linkedProfile;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor,
            "h1", $"Switch to {_linkedProfile.PersonalDetails.Name.GivenName + " " + _linkedProfile.PersonalDetails.Name.FamilyName}'s profile to act on their behalf");

        private WebButton SwitchToThisProfile => WebButton.WithText(_interactor, $"Switch to {_linkedProfile.PersonalDetails.Name.GivenName}'s profile");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements() => SwitchToThisProfile.AssertVisible();

        public void SwitchToLinkedProfile() => SwitchToThisProfile.Click();

    }
}