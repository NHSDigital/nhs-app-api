using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

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
                .Navigation.Appointments();

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
                .IfDisplayed(driver, choice => choice.ChooseChrome().Always());

            AndroidAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsAppAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanFollowLinksInAWebIntegrationIos(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("David").FamilyName("June"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

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
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAWebIntegrationAndReturnUsingJsApiIos(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("David").FamilyName("June"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToBookOrCancelYourReferralAppointment();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsAppAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver);
        }
    }
}
