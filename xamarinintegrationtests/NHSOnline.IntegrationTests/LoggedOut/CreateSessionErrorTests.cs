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
    [BusinessRule("BR-LOG-08.4", "Log in when an unexpected error occurs displays an error message")]
    public class CreateSessionErrorTests
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

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanNavigateToLoggedOutHomeWithTheKeyboardWhenCreateSessionReturnsUnexpectedErrorAndroid(IAndroidDriverWrapper driver)
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

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .KeyboardNavigateToAndActivateContactUs();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAndActivateBackToHome();

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

            IOSAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            IOSNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}