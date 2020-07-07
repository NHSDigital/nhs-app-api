using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
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
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
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
                    .AssertWelcomeMessageDisplayedFor("Fred Jones");
            }
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
                    .AssertWelcomeMessageDisplayedFor("Fred Williams");
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
                    .AssertWelcomeMessageDisplayedFor("Jack Flash");
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
                    .AssertWelcomeMessageDisplayedFor("Jack Flash");
            }
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

            using (var webInteractor = driver.Web(WebViewContext.NhsLogin))
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

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

            using (var webInteractor = driver.Web(WebViewContext.NhsLogin))
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

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
    }
}
