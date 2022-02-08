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
    /// <summary>
    /// BR-LOG-08.4 corresponds to a HTTP 500 Error code during Create Session.
    /// </summary>
    [TestClass]
    [BusinessRule("BR-LOG-08.4", "Log in when an unexpected error occurs displays an error message")]
    public class CreateSessionErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsAnInternalServerErrorAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginGetUserProfileExplodingSuccessCodeBehaviour())
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

            AndroidCreateSessionInternalServerErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidCreateSessionInternalServerErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionInternalServerErrorPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsAnInternalServerErrorIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginGetUserProfileExplodingSuccessCodeBehaviour())
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

            IOSCreateSessionInternalServerErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            IOSCreateSessionInternalServerErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionInternalServerErrorPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToLoggedOutHomeWithTheKeyboardWhenCreateSessionReturnsInternalServerErrorAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginGetUserProfileExplodingSuccessCodeBehaviour());
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

            AndroidCreateSessionInternalServerErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .KeyboardNavigateToAndActivateContactUs();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidCreateSessionInternalServerErrorPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAndActivateBackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}