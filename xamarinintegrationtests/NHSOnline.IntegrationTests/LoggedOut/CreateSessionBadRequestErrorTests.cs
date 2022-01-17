using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-08.2","Log in when OAuth details are incomplete or invalid displays an error message")]
    public class CreateSessionBadRequestErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsBadRequestAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour())
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToLoggedOutHomeWithTheKeyboardWhenCreateSessionReturnsBadRequestAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .KeyboardNavigateToAndActivateContactUs();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAndActivateBackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsBadRequestIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour())
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            IOSCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}