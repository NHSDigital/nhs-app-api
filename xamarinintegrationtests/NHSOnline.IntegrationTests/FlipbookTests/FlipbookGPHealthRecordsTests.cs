using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.HttpMocks.GpMedicalRecord;
using NHSOnline.HttpMocks.Tpp.Models;
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
            FlipbookTestName = "View your GP Health Records")]
        public void APatientWithProofLevelNineCanViewHealthPage(IIOSDriverWrapper driver)
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
                .AssertOnPage(driver, screenshot: true)
                .PageContent
                .NavigateToGPHealthRecord();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver, true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "View your GP Health Records - iOS",
            FlipbookTestName = "View your GP health record - no access")]
        public void APatientWithProofLevelNineHasNoAccessToHealthRecordIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithBehaviour(new EmisRecordsForbiddenBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation
                .NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToGPHealthRecord();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent
                .Continue();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver, true, true);
        }


        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "View your GP Health Records - iOS",
            FlipbookTestName = "Viewing a health record which cannot be downloaded to  maximum file size exceeded (TPP)")]
        public void APatientWithProofLevelNineCannotViewLargeFileFromHealthRecordIOSTpp(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithBehaviour(new TppRecordTooLargeBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation
                .NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToGPHealthRecord();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent
                .Continue();

            IOSGpMedicalRecordListPage
                .AssertOnPage(driver, true, CareRecordLevel.Detailed)
                .DocumentsClick();

            IOSGpMedicalRecordDocumentsPage
                .AssertOnPage(driver, true)
                .DocumentDetailLink.Touch();

            IOSGpMedicalRecordDocumentsPage
                .AssertOnPage(driver, true, true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "View your GP Health Records - iOS",
            FlipbookTestName = "View your GP health record - service unavailable")]
        public void APatientWithProofLevelNineCannotViewHealthRecordWithServiceUnavailableIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithBehaviour(new EmisCreateSessionFailureBehaviour())
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation
                .NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToGPHealthRecord();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent
                .Continue();

            IOSGpMedicalRecordErrorPage
                .AssertOnPage(driver, true)
                .TryAgain();

            IOSGpMedicalRecordErrorPage
                 .AssertOnPage(driver, true, postTryAgain: true);
        }
    }
}

