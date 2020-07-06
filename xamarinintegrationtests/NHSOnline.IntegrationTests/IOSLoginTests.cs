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
            var beforeYouStartPage = NavigateToBeforeYouStartPage(driver);

            beforeYouStartPage.AssertAllLinkArePresent();
            beforeYouStartPage.TriggerConditionsLinkClick();
            IOSAppTab.AssertAppTabServiceByHeader(driver, "Conditions Service");
            beforeYouStartPage.TriggerCovidLinkClick();
            IOSAppTab.AssertAppTabServiceByHeader(driver, "Covid Service");
            beforeYouStartPage.TriggerOneOneOneLinkClick();
            IOSAppTab.AssertAppTabServiceByHeader(driver, "111 Service");
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

            using (var webInteractor = driver.Web())
            {
                StubbedLoginPage
                    .AssertOnPage(webInteractor)
                    .Login(patient);
            }

            _ = IOSLoggedInHomePage.AssertOnPage(driver);
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