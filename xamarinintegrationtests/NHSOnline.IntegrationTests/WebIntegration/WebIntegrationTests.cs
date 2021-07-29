using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    public class WebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanFollowLinksInAWebIntegrationAndReturnUsingJsApiAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("David").FamilyName("April"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToBookOrCancelYourReferralAppointment();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToInternalPage();

            // Internal Page (same domain) should load in WebView
            AndroidErsInternalPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            // NHS Login should load in WebView
            AndroidErsNhsLoginPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToCovid();

            // Other pages should load in App Browser Tab
            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsAppAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientInAWebIntegrationCanUseTheirKeyboardToReturnToTheAppViaTheBottomNavigation(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Valencia").FamilyName("Sanguinelli"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            AndroidNhsLoginSettingsPage
                .AssertOnPage(driver)
                .KeyboardNavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToOneOneOne();

            AndroidOneOneOnePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .KeyboardNavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAToZ();

            AndroidAToZPage
                .AssertOnPage(driver)
                .KeyboardNavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth();

            AndroidYourHealthPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanFollowLinksInAWebIntegrationAndReturnUsingJsApiIos(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("David").FamilyName("June"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToBookOrCancelYourReferralAppointment();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToInternalPage();

            // Internal Page (same domain) should load in WebView
            IOSErsInternalPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            // NHS Login should load in WebView
            IOSErsNhsLoginPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToCovid();

            // Other pages should load in App Browser Tab
            IOSAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsAppAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver);
        }
    }
}
