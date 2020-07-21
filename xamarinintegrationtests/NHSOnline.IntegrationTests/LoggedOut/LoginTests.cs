using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    public class LoginTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Fred Jones");
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Williams"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Fred Williams");
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.Title("Mr").GivenName("Jack").FamilyName("Flash"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Jack Flash");
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.Title("Mr").GivenName("Jack").FamilyName("Flash"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);


            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Jack Flash");
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenNhsLoginReturnsAnErrorRedirectAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .ChooseChrome()
                .JustOnce()
                .AssertOnContactUsPage()
                .ReturnToApp();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenNhsLoginReturnsAnErrorRedirectIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
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
                .BackHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsForbiddenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new EmisCreateSessionForbiddenBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionForbiddenErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .ChooseChrome()
                .JustOnce()
                .AssertOnContactUsPage()
                .ReturnToApp();

            AndroidCreateSessionForbiddenErrorPage
                .AssertOnPage(driver)
                .BackHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsForbiddenIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithBehaviour(new EmisCreateSessionForbiddenBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionForbiddenErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            IOSCreateSessionForbiddenErrorPage
                .AssertOnPage(driver)
                .BackHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsBadRequestAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .ChooseChrome()
                .JustOnce()
                .AssertOnContactUsPage()
                .ReturnToApp();

            AndroidCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .BackHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsBadRequestIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithBehaviour(new NhsLoginAuthoriseBlankCodeBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            IOSCreateSessionBadRequestErrorPage
                .AssertOnPage(driver)
                .BackHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsFailedAgeRequirementAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithAge(12, 300);
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionFailedAgeRequirementErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenCreateSessionReturnsFailedAgeRequirementIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithAge(12, 300);
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionFailedAgeRequirementErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}
