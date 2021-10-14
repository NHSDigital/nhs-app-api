using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Help
{
    [TestClass]
    [BusinessRule("BR-LOG-01.11", "Following the link for Help on the logged out home screen navigates the user to the appropriate location")]
    [BusinessRule("BR-NAV-01.4", "Clicking the help icon opens a contextual help link in a browser overlay")]
    [BusinessRule("BR-NAV-02.7", "When closing a browser overlay the relevant icon stays selected")]
    public class HelpTests
    {
        private const string HomeHelpLinkPath = "/";
        private const string AdviceHelpLinkPath = "/";
        private const string AppointmentsHelpLinkPath = "appointments-and-online-consultations-in-the-nhs-app/gp-surgery-appointments/";
        private const string PrescriptionsHelpLinkPath = "prescriptions-in-the-nhs-app/ordering-a-prescription/";
        private const string YourHealthHelpLinkPath = "health-records-in-the-nhs-app/gp-health-record/";
        private const string MessagesHelpLinkPath = "messaging-in-the-nhs-app/";
        private const string NhsLoginSettingsHelpLinkPath = "nhs-app-account-and-settings/managing-your-nhs-app-account/";
        private const string LoggedOutHelpLinkPath = "logging-in-to-the-nhs-app/";


        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromLoggedOutHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .GetHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, LoggedOutHelpLinkPath)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromLoggedOutHomeScreenIOS(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .GetHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, LoggedOutHelpLinkPath)
                .ReturnToApp();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, HomeHelpLinkPath)
                .ReturnToApp();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AssertNoNavigationIconsSelected();
        }


        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToContextualHelpAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToHelp(patient);

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, HomeHelpLinkPath);
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, HomeHelpLinkPath)
                .ReturnToApp();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AssertNoNavigationIconsSelected();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromAdviceScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, AdviceHelpLinkPath)
                .ReturnToApp();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .Navigation.AssertAdviceSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromAdviceScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, AdviceHelpLinkPath)
                .ReturnToApp();

            IOSAdvicePage
                .AssertOnPage(driver)
                .Navigation.AssertAdviceSelected();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromAppointmentsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, AppointmentsHelpLinkPath)
                .ReturnToApp();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.AssertAppointmentsSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromAppointmentsScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, AppointmentsHelpLinkPath)
                .ReturnToApp();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.AssertAppointmentsSelected();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromPrescriptionsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, PrescriptionsHelpLinkPath)
                .ReturnToApp();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AssertPrescriptionsSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromPrescriptionsScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, PrescriptionsHelpLinkPath)
                .ReturnToApp();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AssertPrescriptionsSelected();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromYourHealthScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            AndroidGpMedicalRecordPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, YourHealthHelpLinkPath)
                .ReturnToApp();

            AndroidGpMedicalRecordPage
                .AssertOnPage(driver)
                .Navigation.AssertYourHealthSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromYourHealthScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, YourHealthHelpLinkPath)
                .ReturnToApp();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .Navigation.AssertYourHealthSelected();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromMessagesScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, MessagesHelpLinkPath)
                .ReturnToApp();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.AssertMessagesSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromMessagesScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, MessagesHelpLinkPath)
                .ReturnToApp();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.AssertMessagesSelected();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessContextualHelpFromNhsLoginSettingsAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            AndroidNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, NhsLoginSettingsHelpLinkPath)
                .ReturnToApp();

            AndroidNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessContextualHelpFromNhsLoginSettingsIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadEmisPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver)
                .NavigateToNhsLoginSettings();

            IOSNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, NhsLoginSettingsHelpLinkPath)
                .ReturnToApp();

            IOSNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        private static EmisPatient LoadEmisPatient()
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();

            Mocks.Patients.Add(patient);

            return patient;
        }
    }
}
