using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.IntegrationTests.Mongo.TermsAndConditions;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookNominatedPharmacyTests
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "Accessing the prescriptions hub - iOS",
            FlipbookTestName = "View your Nominated Pharmacy when already selected", JourneyId = "P-NP-01")]
        public void APatientWithProofLevelNineCanViewTheirNominatedPharmacyWhenAlreadySelected(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithBehaviour(new EmisExistingNominatedPharmacyBehaviour())
                .WithName(b => b.GivenName("Terry").FamilyName("2147483641"))
                .WithNhsNumber("2147483641");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNominatedPharmacy();

            IOSNominatedPharmacyPage
                .AssertOnPage(driver, screenshot: true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "Accessing the prescriptions hub - iOS",
            FlipbookTestName = "View your Nominated Pharmacy when none currently nominated", JourneyId = "P-NP-02")]
        public void APatientWithProofLevelNineCanViewNominatePharmacyInterruptScreenWhenNoneCurrentlyNominated(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("2147483642"))
                .WithNhsNumber("2147483642")
                .WithBehaviour(new EmisNoNominatedPharmacyBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.AssertNominateANewPharmacy()
                .NavigateToNominateANewPharmacy();

            IOSNominateANewPharmacyPage
                .StepStart(driver, continueClick: false);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "View your Nominated Pharmacy when none currently nominated - iOS",
            FlipbookTestName = "Nominate a high street pharmacy if no pharmacy currently nominated", JourneyId = "P-NP-03")]
        public void APatientWithProofLevelNineCanNominateAHighStreetPharmacyWhenNoneCurrentlyNominated(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("2147483643"))
                .WithNhsNumber("2147483643")
                .WithBehaviour(new EmisNoNominatedPharmacyBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.AssertNominateANewPharmacy()
                .NavigateToNominateANewPharmacy();

            var page = IOSNominateANewPharmacyPage
                .StepStart(driver, continueClick: true)
                .StepChoosePharmacy(isOnlinePharmacy: false)
                .StepFindAHighStreetPharmacy()
                .StepSelectAHighStreetPharmacy();

            // PDS search now needs to return that patient has a nominated Pharmacy
            patient.WithBehaviour(new EmisExistingNominatedPharmacyBehaviour());

            page.StepConfirmNominatedPharmacy()
                .StepHighStreetPharmacyFinalConfirmation();
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "View your Nominated Pharmacy when none currently nominated - iOS",
            FlipbookTestName = "Attempt to Nominate an online pharmacy if no pharmacy currently nominated", JourneyId = "P-NP-04")]
        public void APatientWithProofLevelNineCanAttemptToNominateAnOnlinePharmacyWhenNoneCurrentlyNominated(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("2147483644"))
                .WithNhsNumber("2147483644")
                .WithBehaviour(new EmisNoNominatedPharmacyBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.AssertNominateANewPharmacy()
                .NavigateToNominateANewPharmacy();

            IOSNominateANewPharmacyPage
                .StepStart(driver, continueClick: true)
                .StepChoosePharmacy(isOnlinePharmacy: true)
                .StepOnlinePharmacyFinalShutter();
        }
    }
}