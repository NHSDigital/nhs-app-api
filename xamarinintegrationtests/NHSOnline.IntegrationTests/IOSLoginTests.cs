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
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            _ = IOSBeforeYouStartPage.AssertOnPage(driver);
        }

        [Ignore("Issue raised with BrowserStack regarding accessing the api through the browser stack local process")]
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

            _ = IOSCreateSessionPage.AssertOnPage(driver);
        }
    }
}