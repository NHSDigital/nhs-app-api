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
    [BusinessRule("BR-LOG-08.6", "Log in when a timeout response is received displays and error message")]
    public class CreateSessionUpstreamTimeoutErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenNhsLoginGetProfileTimesOutAndroid(IAndroidDriverWrapper driver)
        {
            using var delayBehaviour = new NhsLoginGetUserProfileDelayBehaviour();
            var patient = new EmisPatient()
                .WithBehaviour(delayBehaviour)
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

            using (ExtendedTimeout.FromSeconds(15))
            {
                AndroidCreateSessionUpstreamSystemTimeoutErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .ContactUs();
            }

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidCreateSessionUpstreamSystemTimeoutErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToLoggedOutHomeWithTheKeyboardWhenUpstreamTimeoutCreatingSessionAndroid(IAndroidDriverWrapper driver)
        {
            using var delayBehaviour = new NhsLoginGetUserProfileDelayBehaviour();
            var patient = new EmisPatient()
                .WithBehaviour(delayBehaviour)
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

            using (ExtendedTimeout.FromSeconds(15))
            {
                AndroidCreateSessionUpstreamSystemTimeoutErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .KeyboardNavigateToAndActivateContactUs();
            }

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidCreateSessionUpstreamSystemTimeoutErrorPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAndActivateBackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenNhsLoginGetProfileTimesOutIos(IIOSDriverWrapper driver)
        {
            using var delayBehaviour = new NhsLoginGetUserProfileDelayBehaviour();
            var patient = new EmisPatient()
                .WithBehaviour(delayBehaviour)
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

            using (ExtendedTimeout.FromSeconds(15))
            {
                IOSCreateSessionUpstreamSystemTimeoutErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .ContactUs();
            }

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            IOSCreateSessionUpstreamSystemTimeoutErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}