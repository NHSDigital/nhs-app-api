using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.GpHealthRecord);

            AndroidGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.OrganDonation);

            AndroidOrganDonationPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.Ndop);

            AndroidNdopPage
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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.CovidPass);

            AndroidStubbedNetCompanyInternalPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.VaccineRecord);

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
                .KeyboardNavigateToYourHealth();

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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthGncrPage
                .AssertOnPage(driver)
                .KeyboardNavigateToGncr();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other healthcare letters")
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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.PkbTestResults);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.PkbCarePlans);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.PkbTrackYourHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.PkbSharedHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.PkbRecordSharing);

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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.CieTestResults);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.CieCarePlans);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.CieTrackYourHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.CieSharedHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthCiePage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.CieRecordSharing);

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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.SecondaryCareViewTestResults);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.SecondaryCareViewCarePlans);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.SecondaryCareViewTrackYourHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.SecondaryCareViewSharedHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthSecondaryCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.SecondaryCareViewRecordSharing);

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
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .KeyboardNavigateTo(YourHealthPages.MyCareViewTestResults);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TestResultsPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.MyCareViewCarePlans);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, CarePlansPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.MyCareViewTrackYourHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, TrackYourHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.MyCareViewSharedHealth);

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, SharedHealthPageTitle)
                .KeyboardNavigateBack();

            AndroidYourHealthMyCareViewPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateTo(YourHealthPages.MyCareViewRecordSharing);

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