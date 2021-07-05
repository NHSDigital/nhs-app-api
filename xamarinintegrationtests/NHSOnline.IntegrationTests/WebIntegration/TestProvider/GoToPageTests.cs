using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.TestProvider
{
    [TestClass]
    public class GoToPageTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanNavigateToTheHomePageUsingGoToPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToHomePage();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNavigateToHealthRecordsUsingGoToPageIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToYourHealthPage();

            IOSYourHealthPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanNavigateToThePrescriptionsPageUsingGoToPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNavigateToAdviceUsingGoToPageIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToYourAdvicePage();

            IOSAdvicePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanNavigateToTheMessagesPageUsingGoToPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNavigateToMoreUsingGoToPageIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToMorePage();

            IOSMorePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineSeesTheLoggedInHomePageWhenGoToPageCalledWithInvalidPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToInvalidPage();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNavigateToUpliftUsingGoToPageIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToUpliftPage();

            IOSUpliftShutterPage
                .Continue(driver);

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanNavigateToAppointmentsUsingGoToPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNavigateToSettingsUsingGoToPageIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToSettingsPage();

            IOSMorePage
                .AssertOnPage(driver);
        }
    }
}