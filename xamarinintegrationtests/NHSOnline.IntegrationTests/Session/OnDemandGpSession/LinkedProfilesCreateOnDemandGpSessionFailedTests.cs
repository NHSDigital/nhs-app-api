using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session.OnDemandGpSession
{
    [TestClass]
    [BusinessRule("BR-GEN-04.6", "Failure to obtain a GP session on the first attempt in the user session displays a service specific 'try again' error to the user")]
    [BusinessRule("BR-GEN-04.8", "Failure to obtain a GP session when the user has initiated another attempt to get a GP session via a try again displays a specific service unavailable shutter page to the user")]
    public class LinkedProfilesCreateOnDemandGpSessionFailedTests
    {
        [NhsAppAndroidTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenTryingToAccessLinkedProfilesAndThereIsAFailureCreatingAGpSessionOnDemandAndroid(IAndroidDriverWrapper driver)
        {
            var fo = new EmisPatient()
                .WithName(b => b.GivenName("Fo").FamilyName("Catcha"))
                .WithBehaviour(new NhsLoginReauthorizeSSOBehaviour());
            var brie = new TppPatient()
                .WithName(b => b.GivenName("Brie").FamilyName("Oche"));

            using var patients = Mocks.Patients.Add(fo, brie);

            LoginProcess.LogAndroidPatientIn(driver, fo);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToLinkedProfiles();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(brie);

            AndroidLinkedProfilesTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            AndroidLinkedProfilesUnavailablePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .PageContent.ReportAProblem();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .AssertErrorCode("3c")
                .ReturnToApp();

            driver.PressBackButton();

            AndroidMorePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        [Ignore("Clicking links in the web here seems quite unreliable, ignoring now until we fix this")]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenTryingToAccessLinkedProfilesAndThereIsAFailureCreatingAGpSessionOnDemandIOS(IIOSDriverWrapper driver)
        {
            var anna = new EmisPatient()
                .WithName(b => b.GivenName("Anna").FamilyName("Damma"))
                .WithBehaviour(new NhsLoginReauthorizeSSOBehaviour());
            var crew = new TppPatient()
                .WithName(b => b.GivenName("Crew").FamilyName("Tonn"));

            using var patients = Mocks.Patients.Add(anna, crew);

            LoginProcess.LogIOSPatientIn(driver, anna);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToLinkedProfiles();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(crew);

            IOSLinkedProfilesTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            IOSLinkedProfilesUnavailablePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .PageContent.ReportAProblem();

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .AssertErrorCode("3c")
                .ReturnToApp();

            driver.SwipeBack();

            IOSMorePage
                .AssertOnPage(driver);
        }
    }
}