using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-08.5","Log in when a bad response is received displays an error message")]
    public class CreateSessionBadGatewayErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsBadGatewayAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginTokenBadGatewayBehaviour())
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionBadResponseFromUpstreamSystemErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            AndroidCreateSessionBadResponseFromUpstreamSystemErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToLoggedOutHomeWithTheKeyboardWhenCreateSessionReturnsBadGatewayAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithBehaviour(new NhsLoginTokenBadGatewayBehaviour());
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionBadResponseFromUpstreamSystemErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .KeyboardNavigateToAndActivateContactUs();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            AndroidCreateSessionBadResponseFromUpstreamSystemErrorPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAndActivateBackToHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsBadGatewayIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new NhsLoginTokenBadGatewayBehaviour())
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

            IOSCreateSessionBadResponseFromUpstreamSystemErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            IOSCreateSessionBadResponseFromUpstreamSystemErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}