using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookGpHealthRecordsTests
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - iOS",
            FlipbookTestName = "View your GP health record - no access")]
        public void APatientWithProofLevelNineCanViewHealthRecordWithNoAccessIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .Navigation
                .NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver, true)
                .PageContent
                .NavigateToGPHealthRecord();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver, true)
                .PageContent
                .Continue();

            IOSSessionExpiryPrompt.ExtendIfDisplayed(driver);

            IOSGpMedicalRecordPage
                .AssertOnPage(driver, true);
        }
    }
}

