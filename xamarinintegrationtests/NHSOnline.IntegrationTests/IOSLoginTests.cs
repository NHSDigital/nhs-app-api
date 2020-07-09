using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.Web;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public class IOSLoginTests
    {
        [NhsAppIOSTest]
        public void APatientCanStartTheApp(IIOSDriverWrapper driver)
        {
            _ = IOSLoggedOutHomePage.AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheBeforeYouStartPage(IIOSDriverWrapper driver)
        {
            _ = NavigateToBeforeYouStartPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanClickAllTheLinksAndGoToCorrectPages(IIOSDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertAllLinkArePresent()
                .SearchConditionsAndTreatments();

            IOSAppTab
                .AssertOnConditionsPage(driver)
                .ReturnToApp();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .CheckCoronavirusSymptoms();

            IOSAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .UseNhs111Online();

            IOSAppTab
                .AssertOn111Page(driver)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void APatientCanUseTheExpander(IIOSDriverWrapper driver)
        {
            var beforeYouStartPage = NavigateToBeforeYouStartPage(driver);

            beforeYouStartPage.AssertExpanderPresent();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanLogin(IIOSDriverWrapper driver)
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

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanLogin(IIOSDriverWrapper driver)
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

        private static IOSBeforeYouStartPage NavigateToBeforeYouStartPage(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            var beforeYouStartPage = IOSBeforeYouStartPage.AssertOnPage(driver);
            return beforeYouStartPage;
        }
    }
}