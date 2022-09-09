using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.LinkedProfile;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LinkedProfile
{
    public class IOSSwitchToLinkedProfilePage
    {
        private readonly IIOSInteractor _interactor;
        private IOSFullNavigation Navigation { get; }

        private SwitchToLinkedProfilePageContent PageContent { get; }

        private IOSSwitchToLinkedProfilePage(IIOSDriverWrapper driver, EmisPatient linkedProfilePatientName)
        {
            _interactor = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new SwitchToLinkedProfilePageContent(driver.Web.NhsAppLoggedInWebView(), linkedProfilePatientName);
        }

        public static IOSSwitchToLinkedProfilePage AssertOnPage(IIOSDriverWrapper driver,
            EmisPatient linkedProfile, bool screenshot = false)
        {
            var page = new IOSSwitchToLinkedProfilePage(driver, linkedProfile);
            page.PageContent.AssertOnPage();
            if (screenshot)
            {
                driver.Screenshot(nameof(IOSSwitchToLinkedProfilePage));
            }
            return page;
        }

        public IOSSwitchToLinkedProfilePage AssertPageElements()
        {
            PageContent.AssertPageElements();
            return this;
        }

        public void NavigateToSwitchToLinkedProfile() => PageContent.SwitchToLinkedProfile();

    }
}