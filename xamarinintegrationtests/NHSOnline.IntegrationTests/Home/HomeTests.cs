using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    public class HomeTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanAccessMoreFromHomeScreenAndGoBackHomeAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.More();

            AndroidMorePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Navigation.Home();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanAccessMoreFromHomeScreenAndGoBackHomeIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.More();

            IOSMorePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Navigation.Home();

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanAccessAdviceFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Advice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanAccessAdviceFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Advice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanAccessHelpFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Help();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome().Always());

            AndroidAppTab
                .AssertOnHomeHelpPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanAccessHelpFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Help();

            IOSAppTab
                .AssertOnHomeHelpPage(driver);
        }


        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessAppointmentsFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessAppointmentsFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessPrescriptionsFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Prescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessPrescriptionsFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Prescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessYourHealthFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessYourHealthFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealth();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessMessagesFromHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Messages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessMessagesFromHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Messages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        private static void LogAndroidPatientIn(IAndroidDriverWrapper driver, Patient patient)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent.Continue();
        }

        private static void LogIOSPatientIn(IIOSDriverWrapper driver, Patient patient)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent.Continue();
        }
    }
}
