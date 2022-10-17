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
    [BusinessRule("BR-WI-01.14", "Following a link from within the web integration to a valid NHS App location navigates the user to the specified location")]
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
                .AssertFullNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeaderHome()
                .TabIntoFocus()
                .KeyboardNavigateToGoToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeaderHome()
                .TabIntoFocus()
                .KeyboardNavigateToGoToHealthRecords();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeaderHome()
                .TabIntoFocus()
                .KeyboardNavigateToGoToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeaderHome()
                .TabIntoFocus()
                .KeyboardNavigateToGoToSettings();

            AndroidMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeaderHome()
                .TabIntoFocus()
                .KeyboardNavigateToGoToUplift();

            AndroidUpliftShutterPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanNavigateOnThirdPartiesThatUseTheGoToPageMethodIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Fo").FamilyName("Catcha"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            var iosLoggedInHomePage = IOSLoggedInHomePage
                .AssertOnPage(driver);

            iosLoggedInHomePage
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver, true);

            var iosGoToPage = IOSGoToPage
                .AssertOnPage(driver);

            iosGoToPage
                .AssertNativeHeader()
                .PageContent.GoToHome();

            iosLoggedInHomePage
                .AssertOnPage(iosLoggedInHomePage, driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToHealthRecords();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToSettings();

            IOSMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
                .PageContent.GoToInvalidPage();

            iosLoggedInHomePage
                .AssertOnPage(iosLoggedInHomePage, driver)
                .Navigation.NavigateToAppointments();

            NavigateToHospitalAppointmentsGoToPageIOS(driver);

            iosGoToPage
                .AssertOnPage(iosGoToPage)
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
                .NavigateToGoToPage();
        }
    }
}