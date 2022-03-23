using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookPrescriptionTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs in to the app - Android",
            FlipbookTestName = "A user can order a repeat prescription")]
        public void APatientWithProofLevelNineCanOrderARepeatPrescriptionAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .KeyboardNavigatePrescriptions(patient);

            AndroidPrescriptionsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateToOrderARepeatPrescription();

            AndroidOrderARepeatPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.ChooseRepeat()
                .Continue();

            AndroidChooseRepeatPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.ClickPrescription()
                .InsertSpecialRequest()
                .Continue();

            AndroidCheckPrescriptionPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.Continue();

            AndroidPrescriptionConfirmedPage
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
                .PageContent.ClickPrescription()
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