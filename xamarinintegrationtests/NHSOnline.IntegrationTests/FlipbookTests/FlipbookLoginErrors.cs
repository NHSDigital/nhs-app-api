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
    public class FlipbookLoginErrorTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(FlipbookTestName = "A user logs into the app seeing validation errors")]
        public void APatientWithProofLevelNineCanSuccessfullyLogInSeeingErrorsAndroid(IAndroidDriverWrapper driver)
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

            var termsAndConditions = AndroidTermsAndConditionsPage
                .AssertOnPage(driver, screenshot: true);

            termsAndConditions.PageContent.ContinueWithoutAccepting();

            termsAndConditions.ScreenshotError();
            termsAndConditions.ScrollToContinueAndScreenshot();

            termsAndConditions.PageContent.AcceptTermsAndConditions();

            var androidUserResearchPage = AndroidUserResearchOptInPage
                .AssertOnPage(driver, screenshot: true);

            androidUserResearchPage.PageContent.ContinueWithoutOption();
            androidUserResearchPage.ScreenshotError();
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(FlipbookTestName = "A user logs into the app seeing validation errors")]
        public void APatientWithProofLevelNineCanSuccessfullyLogInSeeingErrorsIOS(IIOSDriverWrapper driver)
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

            var termsAndConditions = IOSTermsAndConditionsPage
                .AssertOnPage(driver, screenshot: true);

            termsAndConditions.PageContent.ContinueWithoutAccepting();

            termsAndConditions.ScreenshotError();
            termsAndConditions.ScrollToContinueAndScreenshot();

            termsAndConditions.PageContent.AcceptTermsAndConditions();

            var iosUserResearchPage = IOSUserResearchOptInPage
                .AssertOnPage(driver, screenshot: true);

            iosUserResearchPage.PageContent.ContinueWithoutOption();
            iosUserResearchPage.ScreenshotError();
        }
    }
}