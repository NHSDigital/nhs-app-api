using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-LOG-05.1", "Log in for a P5 user logs them into the app")]
    [BusinessRule("BR-HOM-02.1", "User registered to P5 level cannot see their NHS number")]
    public class LogInWithP5Tests
    {
        [NhsAppCanaryTest]
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanSuccessfullyLogInAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
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
                .PageContent
                    .AssertVectorOfTrust()
                    .Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Terry Tibbs")
                .PageContent.AssertNhsNumberNotVisible();
        }

        [NhsAppCanaryTest]
        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanSuccessfullyLogInIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
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
                .PageContent
                    .AssertVectorOfTrust()
                    .Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Terry Tibbs")
                .PageContent.AssertNhsNumberNotVisible();
        }
    }
}