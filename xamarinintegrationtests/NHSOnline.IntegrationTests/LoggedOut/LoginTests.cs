using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.Web;
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
                .WithName(b => b.GivenName("Fred").FamilyName("Bloggs"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            using (var webInteractor = driver.Web(WebViewContext.NhsLogin))
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

            using (var webInteractor = driver.Web(WebViewContext.NhsApp))
            {
                TermsAndConditionsPage
                    .AssertOnPage(webInteractor)
                    .AcceptTermsAndConditions();

                UserResearchOptInPage
                    .AssertOnPage(webInteractor)
                    .OptInToUserResearch();

                LoggedInHomePage
                    .AssertOnPage(webInteractor)
                    .AssertWelcomeMessageDisplayedFor("Fred Bloggs");
            }
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Bloggs"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            using (var webInteractor = driver.Web(WebViewContext.NhsLogin))
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

            using (var webInteractor = driver.Web(WebViewContext.NhsApp))
            {
                TermsAndConditionsPage
                    .AssertOnPage(webInteractor)
                    .AcceptTermsAndConditions();

                UserResearchOptInPage
                    .AssertOnPage(webInteractor)
                    .OptInToUserResearch();

                LoggedInHomePage
                    .AssertOnPage(webInteractor)
                    .AssertWelcomeMessageDisplayedFor("Fred Bloggs");
            }
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

            using (var webInteractor = driver.Web(WebViewContext.NhsLogin))
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

            using (var webInteractor = driver.Web(WebViewContext.NhsApp))
            {
                TermsAndConditionsPage
                    .AssertOnPage(webInteractor)
                    .AcceptTermsAndConditions();

                UserResearchOptInPage
                    .AssertOnPage(webInteractor)
                    .OptInToUserResearch();

                LoggedInHomePage
                    .AssertOnPage(webInteractor)
                    .AssertWelcomeMessageDisplayedFor("Mr Jack Flash");
            }
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

            using (var webInteractor = driver.Web(WebViewContext.NhsLogin))
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

            using (var webInteractor = driver.Web(WebViewContext.NhsApp))
            {
                TermsAndConditionsPage
                    .AssertOnPage(webInteractor)
                    .AcceptTermsAndConditions();

                UserResearchOptInPage
                    .AssertOnPage(webInteractor)
                    .OptInToUserResearch();

                LoggedInHomePage
                    .AssertOnPage(webInteractor)
                    .AssertWelcomeMessageDisplayedFor("Mr Jack Flash");
            }
        }
    }
}