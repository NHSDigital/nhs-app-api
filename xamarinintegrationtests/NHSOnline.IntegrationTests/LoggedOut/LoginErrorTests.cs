using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Logs;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    public class LoginErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenNhsLoginReturnsAnErrorRedirectAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour())
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            var timing = TimedTestExecutor.Execute(() =>
            {
                AndroidStubbedLoginPageSlimHeader
                    .AssertOnPage(driver)
                    .PageContent.Login(patient);

                AndroidNhsLoginErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .ContactUs();
            });

            var clientLogs = new ClientLoggerLogs(timing.StartTime, timing.StopTime);
            clientLogs.AssertClientLogContainsRegex(@"Error reference 3w?(\S{4}) generated. NHS login redirect error");

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidStubbedLoginPageSlimHeader
                    .AssertOnPage(driver)
                    .PageContent.Login(patient);

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenNhsLoginReturnsAnErrorRedirectIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour())
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

            IOSNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            IOSNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackToLogin();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSNhsLoginErrorPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToLoggedOutHomeWithTheKeyboardWhenNhsLoginReturnsUnexpectedErrorAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour());
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

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .KeyboardNavigateToAndActivateContactUs();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayContactUsPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAndActivateBackToLogin();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}