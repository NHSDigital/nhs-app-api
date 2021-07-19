using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Appointments
{
    [TestClass]
    [BusinessRule("BR-GI-01.54",
        "Navigating away from an in progress eConsultation prompts the user to confirm they want to exit the consultation")]
    [BusinessRule("BR-GI-01.55",
        "Selecting to leave an eConsultation when prompted ends the consultation and navigates the user to the relevant destination")]
    [BusinessRule("BR-GI-01.56",
        "Selecting to stay in an eConsult when prompted does not end the consultation and remains on the page")]
    public class AdditionalGpServicesTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownTheOlcLeaveDialogChoosesToStayThenChoosesToLeaveAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Anne").FamilyName("Teak"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToAdditionalGpServices();

            AndroidAdditionalGpServicesStartPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions()
                .Continue();

            AndroidAdditionalGpServicesPrivacyPage
                .AssertOnPage(driver)
                .PageContent.AcceptPrivacyPolicy()
                .Continue();

            AndroidAdditionalGpServicesConditionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidLeavePrompt
                .AssertDisplayed(driver)
                .Cancel();

            AndroidAdditionalGpServicesConditionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidLeavePrompt
                .AssertDisplayed(driver)
                .Leave();

            AndroidPrescriptionsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheOlcLeaveDialogChoosesToStayThenChoosesToLeaveIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Anne").FamilyName("Teak"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToAdditionalGpServices();

            IOSAdditionalGpServicesStartPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions()
                .Continue();

            IOSAdditionalGpServicesPrivacyPage
                .AssertOnPage(driver)
                .PageContent.AcceptPrivacyPolicy()
                .Continue();

           IOSAdditionalGpServicesConditionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSLeavePrompt
                .AssertDisplayed(driver)
                .Cancel();

            IOSAdditionalGpServicesConditionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSLeavePrompt
                .AssertDisplayed(driver)
                .Leave();

            IOSPrescriptionsPage
                .AssertOnPage(driver);
        }
    }
}