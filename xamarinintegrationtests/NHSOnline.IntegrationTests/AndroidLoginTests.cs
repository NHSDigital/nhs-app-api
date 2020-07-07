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
            _ = AndroidLoggedOutHomePage.AssertOnPage(driver);
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

            using (var webInteractor = driver.Web())
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

            _ = AndroidLoggedInHomePage.AssertOnPage(driver);
        }

        [NhsAppAndroidTest()]
        public void APatientCanClickTheLinksAndIsTakenToTheCorrectPages(IAndroidDriverWrapper driver)
        {
            var beforeYouStartPage = NavigateToBeforeYouStartPage(driver);

            beforeYouStartPage.TriggerCovidLinkClick();
            AndroidAppTab.AssertFirstAppTabServiceByHeader(driver, "Covid Service");
            beforeYouStartPage.TriggerConditionsLinkClick();
            AndroidAppTab.AssertSubsequentAppTabServiceByHeader(driver, "Conditions Service");
            beforeYouStartPage.TriggerOneOneOneLinkClick();
            AndroidAppTab.AssertSubsequentAppTabServiceByHeader(driver, "111 Service");
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
