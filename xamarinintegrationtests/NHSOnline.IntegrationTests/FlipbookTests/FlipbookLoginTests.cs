using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookLoginTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(FlipbookTestName = "A user logs into the app")]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver, screenshot: true)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver,  screenshot: true)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver, screenshot: true)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.OptInToUserResearch();

            AndroidLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .AssertPageDisplayedFor("Wendy House");
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(FlipbookTestName = "A user logs into the app")]
        public void APatientWithProofLevelNineCanSuccessfullyLogInIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver, screenshot: true)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver, screenshot: true)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .AssertPageDisplayedFor("Wendy House");
        }
    }
}