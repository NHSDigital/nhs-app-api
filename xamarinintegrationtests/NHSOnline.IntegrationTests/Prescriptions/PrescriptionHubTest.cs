using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Prescriptions
{
    [TestClass]
    [BusinessRule("BR-PRE-01", "Show prescriptions hub page")]
    public class PrescriptionsHubTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownThePrescriptionHubPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Pres").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertPkbElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownThePrescriptionHubPageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Pres").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertPkbElements();
        }

        [NhsAppAndroidTest]
        public void APatientIsShownPrescriptionHubAndCanNavigateBackFromLinksViaKeyboardNavigationAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Pres").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigatePrescriptions(patient);

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .KeyboardNavigateToOrderARepeatPrescriptionPage();

            AndroidOrderARepeatPrescriptionPage
                .AssertOnPage(driver)
                .KeyboardNavigateBack();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToViewYourOrders();

            AndroidViewYourOrdersPage
                .AssertOnPage(driver)
                .KeyboardNavigateBack();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToPkbHospitalAndOtherPrescriptions();

            AndroidPkbHospitalAndOtherPrescriptionsPage
                .AssertOnPage(driver)
                .KeyboardNavigateBack();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertPkbElements();
        }
    }
}