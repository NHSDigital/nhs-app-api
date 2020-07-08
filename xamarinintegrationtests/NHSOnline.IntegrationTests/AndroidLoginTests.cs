using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Web;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public class AndroidLoginTests
    {
        [NhsAppAndroidTest]
        public void APatientCanStartTheApp(IAndroidDriverWrapper driver)
        {
            _ = AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertCovidLinkOnPage()
                .AssertHelpIconOnPage();
        }

        [NhsAppAndroidTest]
        public void APatientCanClickToViewCovidConditions(IAndroidDriverWrapper driver)
        {
            var loginPage = AndroidLoggedOutHomePage.AssertOnPage(driver);
            loginPage.ClickCovidBanner();
            AndroidAppTab.AssertFirstAppTabServiceByHeader(driver, "Covid Conditions");
        }

        [NhsAppAndroidTest]
        public void APatientCanClickToGetHelpWithLoggingIn(IAndroidDriverWrapper driver)
        {
            var loginPage = AndroidLoggedOutHomePage.AssertOnPage(driver);
            loginPage.ClickHelpIcon();
            AndroidAppTab.AssertFirstAppTabServiceByHeader(driver, "Login Help");
        }

        [NhsAppAndroidTest]
        public void APatientIsShownTheBeforeYouStartPage(IAndroidDriverWrapper driver)
        {
            _ = NavigateToBeforeYouStartPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanLogin(IAndroidDriverWrapper driver)
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

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanLogin(IAndroidDriverWrapper driver)
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

        [NhsAppAndroidTest()]
        public void APatientCanClickTheLinksAndIsTakenToTheCorrectPages(IAndroidDriverWrapper driver)
        {
            var beforeYouStartPage = NavigateToBeforeYouStartPage(driver);

            beforeYouStartPage.TriggerCovidLinkClick();
            AndroidAppTab.AssertFirstAppTabServiceByHeader(driver, "Covid");
            beforeYouStartPage.TriggerConditionsLinkClick();
            AndroidAppTab.AssertSubsequentAppTabServiceByHeader(driver, "Conditions");
            beforeYouStartPage.TriggerOneOneOneLinkClick();
            AndroidAppTab.AssertSubsequentAppTabServiceByHeader(driver, "111");
        }

        [NhsAppAndroidTest()]
        public void APatientCanUseTheExpander(IAndroidDriverWrapper driver)
        {
            var beforeYouStartPage = NavigateToBeforeYouStartPage(driver);
            beforeYouStartPage.AssertExpanderPresent();
        }

        private static AndroidBeforeYouStartPage NavigateToBeforeYouStartPage(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            var beforeYouStartPage = AndroidBeforeYouStartPage.AssertOnPage(driver);
            return beforeYouStartPage;
        }
    }
}
