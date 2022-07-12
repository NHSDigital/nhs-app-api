using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookPrescriptionErrorsTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs in to the app - Android",
            FlipbookTestName = "A user can order a repeat prescription seeing validation errors")]
        public void APatientWithProofLevelNineCanOrderARepeatPrescriptionSeeingErrorsAndroid(IAndroidDriverWrapper driver)
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

            var androidOrderARepeatPrescriptionPage =
                AndroidOrderARepeatPrescriptionPage.AssertOnPage(driver, screenshot: true);

            androidOrderARepeatPrescriptionPage.PageContent.Continue();
            androidOrderARepeatPrescriptionPage.ScreenshotError();

            androidOrderARepeatPrescriptionPage.PageContent
                .ChooseRepeat()
                .Continue();

            var androidChooseRepeatPrescriptionPage =
                AndroidChooseRepeatPrescriptionPage.AssertOnPage(driver, screenshot: true);

            androidChooseRepeatPrescriptionPage.PageContent.Continue();
            androidChooseRepeatPrescriptionPage.ScreenshotError();
        }

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

            iosOrderARepeatPrescriptionPage.PageContent.Continue();
            iosOrderARepeatPrescriptionPage.ScreenshotError();

            iosOrderARepeatPrescriptionPage
                .PageContent
                .ChooseRepeat()
                .Continue();

            var iosChooseRepeatPrescriptionPage =
                IOSChooseRepeatPrescriptionPage.AssertOnPage(driver, screenshot: true);

            iosChooseRepeatPrescriptionPage.PageContent.Continue();
            iosChooseRepeatPrescriptionPage.ScreenshotError();
        }
    }
}