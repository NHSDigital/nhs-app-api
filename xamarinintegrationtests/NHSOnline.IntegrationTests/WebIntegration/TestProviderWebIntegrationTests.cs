using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    public class TestProviderWebIntegrationTests
    {
        private const int ValidStartTime = 1893589200; // Wednesday, 2 January 2030 13:00:00
        private const int ValidEndTime = 1893589800; // Wednesday, 2 January 2030 13:10:00
        private const int InvalidEndTime = 1893589100; // Wednesday, 2 January 2030 12:58:20

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessTheTestProviderFromMessagesScreenAndroid(
            IAndroidDriverWrapper driver)
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
                .AssertNativeHeader();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanUploadTheirFileToTheTestProviderFileUploadScreenAndroid(
            IAndroidDriverWrapper driver)
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
                .PageContent.NavigateToFileUpload();

            AndroidFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .UploadTestFile()
                .PageContent
                .AssertFileNotSelected()
                .UploadFile();

            AndroidStoragePage
                .AssertOnPage(driver)
                .SelectFile();

            AndroidFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.AssertFileSelected();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAddAnEventToTheCalendarOnTheTestProviderCalendarScreenAndroid(
            IAndroidDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            AndroidCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, ValidEndTime)
                .AddCalendarEvent();

            AndroidGoogleCalendarsApp
                .AssertOnPage(driver)
                .NavigateThroughOverview()
                .ConfirmGotIt()
                .AssertDetailsArePassed();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAddAnEventToTheCalendarOnTheTestProviderCalendarScreenIOS(
            IIOSDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, ValidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSCalendarsApp
                .AssertOnPage(driver)
                .AssertDetailsArePassed();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnTheTestProviderCalendarScreenIsShownAndErrorDialogCanBeDismissedAndroid(
            IAndroidDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            AndroidCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .AddCalendarEvent();

            AndroidCalendarErrorDialog
                .AssertDisplayed(driver)
                .Ok();

            AndroidCalendarPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnTheTestProviderCalendarScreenIsShownAndErrorDialogCanBeDismissedIOS(
            IIOSDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarValidationErrorDialog
                .AssertDisplayed(driver)
                .Ok();

            IOSCalendarPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnTheTestProviderCalendarScreenIsShownAnErrorDialogAndCanAddedManuallyAndroid(
            IAndroidDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            AndroidCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .AddCalendarEvent();

            AndroidCalendarErrorDialog
                .AssertDisplayed(driver)
                .AddEventManually();

            AndroidGoogleCalendarsApp
                .AssertOnPage(driver)
                .NavigateThroughOverview()
                .ConfirmGotIt();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineTryingToCallAnInvalidDateRangeOnTheTestProviderCalendarScreenIsShownAnErrorDialogAndCanAddedManuallyIOS(
            IIOSDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, InvalidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarValidationErrorDialog
                .AssertDisplayed(driver)
                .AddEventManually();

            IOSCalendarPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSCalendarsApp
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineTryingToAddToCalendarWithValidDetailsDeniesAccessAndIsShownTheRelevantWarningIOS(
            IIOSDriverWrapper driver)
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
                .PageContent.NavigateToCalendar();

            IOSCalendarPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .AddCalendarDetails(ValidStartTime, ValidEndTime)
                .PageContent.AddCalendarEvent();

            IOSCalendarPermissionDialog
                .AssertDisplayed(driver)
                .Deny();

            IOSCalendarPermissionErrorDialog
                .AssertDisplayed(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanKeyboardNavigateToAccessTheTestProviderFromMessagesScreenAndroid(
            IAndroidDriverWrapper driver)
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
                .KeyboardNavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessTheTestProviderFromMessagesScreenIOS(IIOSDriverWrapper driver)
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
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanUploadTheirFileToTheTestProviderFileUploadScreenIOS(
            IIOSDriverWrapper driver)
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
                .PageContent.NavigateToFileUpload();

            IOSFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .UploadTestFile()
                .PageContent.UploadFile();

            IOSFileSourceDialog
                .GetPanel(driver)
                .SelectBrowse();

            IOSStoragePage
                .AssertOnPage(driver)
                .SearchForText()
                .SelectFile();

            IOSFileUploadPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.AssertFileSelected();
        }
    }
}