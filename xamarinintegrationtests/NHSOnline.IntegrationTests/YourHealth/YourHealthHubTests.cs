using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth.Ndop;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.YourHealth
{
    [TestClass]
    [BusinessRule("BR-HRE-01.1", "Show Health Records hub page")]
    public class YourHealthHubTests
    {
        private const string TestResultsPageTitle = "Test results";
        private const string CarePlansPageTitle = "Care plans";
        private const string TrackYourHealthPageTitle = "Track your health";
        private const string SharedHealthPageTitle = "Shared health links";
        private const string RecordSharingPageTitle = "Record Sharing";

        [NhsAppAndroidTest]
        public void APatientIsShownTheYourHealthHubPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertCovidPassElements()
                .AssertVaccineRecordElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheYourHealthHubPageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertCovidPassElements()
                .AssertVaccineRecordElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessYourHealthLinksKeyboardNavigationAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Pkb)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateToGpHealthRecord();

            AndroidGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            // Step removed to prevent flaky test failures see https://nhsd-jira.digital.nhs.uk/browse/NHSO-16514 for details
            // AndroidYourHealthPkbPage
            //     .AssertOnPage(driver)
            //     .TabIntoFocus()
            //     .KeyboardNavigateToOrganDonation();

            // AndroidOrganDonationPage
            //     .AssertOnPage(driver)
            //     .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToNdop();

            AndroidNdopOverviewPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessCovidLinksOnYourHealthAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Pkb)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateToCovidPass();

            AndroidStubbedNetCompanyInternalPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateToVaccineRecord();

            AndroidStubbedVaccineRecordPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .PageContent
                .AssertCovidPassElements()
                .AssertVaccineRecordElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessSubstraktOnYourHealthAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Substrakt)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthSubstraktPage
                .AssertOnPage(driver)
                .KeyboardNavigateToSubstrakt();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Update your personal details")
                .KeyboardNavigateBack();

            AndroidYourHealthSubstraktPage
                .AssertOnPage(driver)
                .PageContent
                .AssertElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanAccessGncrOnYourHealthAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Gncr)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthGncrPage
                .AssertOnPage(driver)
                .KeyboardNavigateToGncr();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other healthcare documents")
                .KeyboardNavigateBack();

            AndroidYourHealthGncrPage
                .AssertOnPage(driver)
                .PageContent
                .AssertElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanKeyboardNavigatePkbLinksAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Pkb)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateToTestResults();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCarePlans();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToTrackYourHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToSharedHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToRecordSharing();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, RecordSharingPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .PageContent
                .AssertElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanKeyboardNavigateCieLinksAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Cie)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .KeyboardNavigateToTestResults();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCarePlans();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToTrackYourHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToSharedHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToRecordSharing();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, RecordSharingPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .PageContent.AssertElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanKeyboardNavigateSecondaryCareViewLinksAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.SecondaryCareView)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .KeyboardNavigateToTestResults();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCarePlans();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToTrackYourHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToSharedHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToRecordSharing();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, RecordSharingPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .PageContent
                .AssertElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanKeyboardNavigateMyCareViewLinksAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.MyCareView)
                .WithName(b => b.GivenName("Uri").FamilyName("Health"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .KeyboardNavigateToTestResults();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCarePlans();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToTrackYourHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToSharedHealth();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToRecordSharing();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, RecordSharingPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .PageContent
                .AssertElements();
        }
    }
}