using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
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

            LoginProcess.LogAndroidPatientIn(driver, patient);

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

            LoginProcess.LogIOSPatientIn(driver, patient);

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

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AdviceIcon.Click();

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

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AdviceIcon.Click();

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

            LoginProcess.LogAndroidPatientIn(driver, patient);

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

            LoginProcess.LogIOSPatientIn(driver, patient);

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

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AppointmentsIcon.Click();

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

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AppointmentsIcon.Click();

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

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.Click();

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

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.Click();

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

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

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

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

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

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

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

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            IOSMessagesPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToAdviceAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToAppointmentsAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToPrescriptionsAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigatePrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToYourHealthAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToMessagesAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateFromHomeScreenAndGoBackHomeAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .KeyboardNavigateToHome();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToHelpAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToHelp();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome().Always());

            AndroidAppTab
                .AssertOnHomeHelpPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUseTheKeyboardToNavigateToMoreAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}
