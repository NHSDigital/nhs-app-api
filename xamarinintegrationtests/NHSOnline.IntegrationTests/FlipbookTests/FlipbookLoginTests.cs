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
        [NhsAppFlipbookTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            driver.Screenshot("LoggedOutHomePage");

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            driver.Screenshot("GettingStartedPage");

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            driver.Screenshot("TermsAndConditions");

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            driver.Screenshot("UserResearch");

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Wendy House");

            driver.Screenshot("LoggedInHome");
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            driver.Screenshot("LoggedOutHome");

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            driver.Screenshot("GettingStarted");

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            driver.Screenshot("TermsAndConditions");

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            driver.Screenshot("UserResearch");

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Wendy House");

            driver.Screenshot("LoggedInHome");
        }
    }
}