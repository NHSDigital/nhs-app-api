using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.1", "Navigating to a service via the native footer the nav icon is highlighted")]
    public class CorrectIconIsSelectedOnClick
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateToEachHubUsingTheBottomNavAndCanSeeTheRelevantBottomNavIconHighlightedAndroid(
            IAndroidDriverWrapper driver)
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
                .Navigation.AppointmentsIcon.AssertSelected();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.Click();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.AssertSelected();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.AssertSelected();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.AssertSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanNavigateToEachHubUsingTheBottomNavAndCanSeeTheRelevantBottomNavIconHighlightedIOS(
            IIOSDriverWrapper driver)
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
                .Navigation.AppointmentsIcon.AssertSelected();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.Click();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.PrescriptionsIcon.AssertSelected();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.AssertSelected();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.AssertSelected();
        }
    }
}