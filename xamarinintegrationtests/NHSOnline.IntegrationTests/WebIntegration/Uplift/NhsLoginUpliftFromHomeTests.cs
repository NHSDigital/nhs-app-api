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
    public class NhsLoginUpliftFromHomeTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanStartTheUpliftJourneyFromTheHomepageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanStartTheUpliftJourneyFromTheHomepageIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }
    }
}