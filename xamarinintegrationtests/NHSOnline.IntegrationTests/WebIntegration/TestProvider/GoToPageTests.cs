using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration.TestProvider
{
    [TestClass]
    public class GoToPageTests
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateOnThirdPartiesThatUseTheGoToPageMethodAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Brie").FamilyName("Oche"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver, true);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToHealthRecords();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToSettings();

            AndroidMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToUplift();

            AndroidUpliftShutterPage
                .AssertOnPage(driver)
                .NavigateToAppointments();
        }

        [NhsAppIOSTest]
        public void APatientCanNavigateOnThirdPartiesThatUseTheGoToPageMethodIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Fo").FamilyName("Catcha"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver, true);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToHome();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToHealthRecords();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToSettings();

            IOSMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToInvalidPage();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToUplift();

            IOSUpliftShutterPage
                .AssertOnPage(driver);
        }

        private static void NavigateToHospitalAppointmentsGoToPageAndroid(
            IAndroidDriverWrapper driver,
            bool assertWarningPanel = false)
        {
            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            if (assertWarningPanel)
            {
                AndroidWebIntegrationWarningPanelPage
                    .AssertOnPage(driver, "View appointments")
                    .PageContent.NavigateToNextPage();
            }

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToGoToPage();
        }

        private static void NavigateToHospitalAppointmentsGoToPageIOS(
            IIOSDriverWrapper driver,
            bool assertWarningPanel = false)
        {
            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            if (assertWarningPanel)
            {
                IOSWebIntegrationWarningPanelPage
                    .AssertOnPage(driver, "View appointments")
                    .PageContent.NavigateToNextPage();
            }

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToGoToPage();
        }
    }
}