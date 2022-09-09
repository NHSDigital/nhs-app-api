using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.LinkedProfile;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LinkedProfile
{
    public class IOSLinkedProfilesPage
    {
        private readonly IIOSInteractor _interactor;

        private EmisPatient _linkedProfile;

        IOSMenuItem LinkedProfileMenuItem =>
            IOSMenuItem.StartsWith(_interactor, $"{_linkedProfile.PersonalDetails.Name.GivenName + " " + _linkedProfile.PersonalDetails.Name.FamilyName}");

        private IOSLinkedProfilesPage(IIOSDriverWrapper driver, EmisPatient linkedProfile)
        {
            _interactor = driver;
            _linkedProfile = linkedProfile;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new LinkedProfilesPageContent(driver.Web.NhsAppLoggedInWebView(), linkedProfile);
        }

        public IOSFullNavigation Navigation { get; }

        public LinkedProfilesPageContent PageContent { get; }

        public static IOSLinkedProfilesPage AssertOnPage(IIOSDriverWrapper driver, EmisPatient linkedProfile,  bool screenshot = false)
        {
            var page = new IOSLinkedProfilesPage(driver, linkedProfile);
            page.PageContent.AssertOnPage();
            if (screenshot)
            {
                driver.Screenshot(nameof(IOSLinkedProfilesPage));
            }
            return page;
        }

        public void AssertPageElements() => PageContent.AssertPageElements();

        public void NavigateToLinkedProfile() => LinkedProfileMenuItem.Click();

    }
}