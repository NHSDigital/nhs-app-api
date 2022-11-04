using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookPrescriptionErrorsTests
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs in to the app - iOS",
            FlipbookTestName = "A user can order a repeat prescription seeing validation errors")]
        public void APatientWithProofLevelNineCanOrderARepeatPrescriptionSeeingErrorsIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot:true)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver,screenshot:true)
                .PageContent.NavigateToOrderARepeatPrescription();

            var iosOrderARepeatPrescriptionPage =
                IOSOrderARepeatPrescriptionPage.AssertOnPage(driver, screenshot: true);

            iosOrderARepeatPrescriptionPage.Continue();
            iosOrderARepeatPrescriptionPage.ScreenshotError();

            iosOrderARepeatPrescriptionPage.ChooseRepeat();

            iosOrderARepeatPrescriptionPage.ScrollToContinueAndScreenshot();
            iosOrderARepeatPrescriptionPage.Continue();

            var iosChooseRepeatPrescriptionPage =
                IOSChooseRepeatPrescriptionPage.AssertOnPage(driver, screenshot: true);

            iosChooseRepeatPrescriptionPage.PageContent.Continue();
            iosChooseRepeatPrescriptionPage.ScreenshotError();
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "Accessing the prescriptions hub - iOS",
            FlipbookTestName = "Repeat Prescriptions not available ", JourneyId = "P-RE-01")]
        public void RepeatPrescriptionServiceNotOfferedByEmisGpShowsShutterScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithBehaviour(new EmisPrescriptionsForbiddenBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewYourOrders();

            IOSRepeatPrescriptionsUnavailablePage
                .AssertOnPage(driver, screenshot: true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "Accessing the prescriptions hub - iOS",
            FlipbookTestName = "Error submitting prescription order", JourneyId = "P-RE-04")]
        public void APatientWithProofLevelNineCanOrderARepeatPrescriptionAndSeeErrorsIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithNhsNumber("2147483647")
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToOrderARepeatPrescription();

            IOSOrderARepeatPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .ChooseRepeat()
                .Continue();

            IOSChooseRepeatPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.ChoosePrescription()
                .InsertPotentiallyDangerousSpecialRequest()
                .Continue();

            IOSCheckPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.Continue();

            IOSPrescriptionConfirmedErrorPage
                .AssertOnPage(driver, screenshot: true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "Accessing the prescriptions hub - iOS",
            FlipbookTestName = "Attempting to order prescription when GP Services down", JourneyId = "P-RE-07")]
        public void APatientWithProofLevelNineAttemptsToOrderPrescriptionWhenGPServicesAreDownIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithBehaviour(new EmisCreateSessionFailureBehaviour())
                .WithNhsNumber("2147483647")
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToOrderARepeatPrescription();

            IOSOrderARepeatPrescriptionErrorPage.AssertOnPage(driver, screenshot: true)
                .ClickTryAgainButton();

            IOSPrescriptionsUnavailablePage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}