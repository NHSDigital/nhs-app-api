using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session.OnDemandGpSession
{
    [TestClass]
    [BusinessRule("BR-GEN-04.6", "Failure to obtain a GP session on the first attempt in the user session displays a service specific 'try again' error to the user")]
    [BusinessRule("BR-GEN-04.8", "Failure to obtain a GP session when the user has initiated another attempt to get a GP session via a try again displays a specific service unavailable shutter page to the user")]
    public class PrescriptionsCreateOnDemandGpSessionFailedTests
    {
        [NhsAppAndroidTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenTryingToAccessPrescriptionsAndThereIsAFailureCreatingAGpSessionOnDemandAndroid(IAndroidDriverWrapper driver)
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
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToOrderARepeatPrescription();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(brie);

            AndroidPrescriptionsTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            AndroidPrescriptionsUnavailablePage
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

            AndroidPrescriptionsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientSeesServiceSpecificGpSessionErrorScreensWhenTryingToAccessPrescriptionsAndThereIsAFailureCreatingAGpSessionOnDemandIOS(IIOSDriverWrapper driver)
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
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToOrderARepeatPrescription();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(crew);

            IOSPrescriptionsTemporaryProblemPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .TryAgain();

            IOSPrescriptionsUnavailablePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .PageContent.ReportAProblem();

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .AssertErrorCode("3c")
                .ReturnToApp();

            driver.SwipeBack();

            IOSPrescriptionsPage
                .AssertOnPage(driver);
        }
    }
}