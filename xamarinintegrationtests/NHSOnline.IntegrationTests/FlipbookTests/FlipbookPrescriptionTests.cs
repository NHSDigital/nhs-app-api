using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookPrescriptionTests
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(FlipbookTestName = "Accessing the prescriptions hub", JourneyId = "P-RE-00")]
        public void APatientWithProofLevelNineCanAccessThePrescriptionsHub(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithBehaviour(new EmisNoNominatedPharmacyBehaviour())
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("2147483640");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver, screenshot: true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs in to the app - iOS",
            FlipbookTestName = "A user can order a repeat prescription")]
        public void APatientWithProofLevelNineCanOrderARepeatPrescriptionIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateToOrderARepeatPrescription();

            IOSOrderARepeatPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.ChooseRepeat()
                .Continue();

            IOSChooseRepeatPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.ChoosePrescription()
                .InsertSpecialRequest()
                .Continue();

            IOSCheckPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.Continue();

            IOSPrescriptionConfirmedPage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}