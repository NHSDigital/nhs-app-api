using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
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

            Login(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.HospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.BookOrCancelYourReferralAppointment();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.InternalPage();

            // Internal Page (same domain) should load in WebView
            AndroidErsInternalPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NhsLogin();

            // NHS Login should load in WebView
            AndroidErsNhsLoginPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.Covid();

            // Other pages should load in App Browser Tab
            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            AndroidErsPage
                .AssertOnPage(driver)
                .PageContent.NhsAppAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanFollowLinksInAWebIntegrationIos(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("David").FamilyName("June"));
            using var patients = Mocks.Patients.Add(patient);

            Login(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.HospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.BookOrCancelYourReferralAppointment();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.InternalPage();

            // Internal Page (same domain) should load in WebView
            IOSErsInternalPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NhsLogin();

            // NHS Login should load in WebView
            IOSErsNhsLoginPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.Back();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.Covid();

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

            Login(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Appointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.HospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.BookOrCancelYourReferralAppointment();

            IOSErsPage
                .AssertOnPage(driver)
                .PageContent.NhsAppAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver);
        }

        private static void Login(IAndroidDriverWrapper driver, Patient patient)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
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

        private static void Login(IIOSDriverWrapper driver, Patient patient)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
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
