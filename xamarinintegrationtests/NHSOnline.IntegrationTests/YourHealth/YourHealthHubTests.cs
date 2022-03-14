using System;
using System.Threading;
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
        private const string TestResultsAndImagingPageTitle = "Test results and imaging";
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

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToOrganDonation();

            // Due to GP On-demand, it can take a few seconds for the page to load with properly
            Thread.Sleep(TimeSpan.FromSeconds(5));

            AndroidOrganDonationPage
                .AssertOnPage(driver)
                .PageContent.ClickBackBreadcrumb();

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
                .AssertOnPage(driver, TestResultsAndImagingPageTitle)
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
    }
}