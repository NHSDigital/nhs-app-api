using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Uplift
{
    [TestClass]
    [BusinessRule("BR-LOG-12.1", "Navigating to the uplift journey from the logged in home screen displays NHS login uplift journey with the slim blue header")]
    [BusinessRule("BR-LOG-12.2", "Closing the slim blue header when the user has navigated to the NHS login uplift journey from the logged in home screen displays the logged in home screen")]
    public class NhsLoginUpliftFromHomeCloseTests
    {
        [NhsAppAndroidTest]
        public void ClosingTheUpliftJourneyLaunchedFromTheHomepageShowsTheHomepageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .Navigation.Close();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void ClosingTheUpliftJourneyLaunchedFromTheHomepageShowsTheHomepageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .Navigation.Close();

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}