using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.4", "Navigating to More via the top menu deselects the currently highlighted icon")]
    public class NavigatingToMoreClearsTheSelectedIconTest
    {
        [NhsAppAndroidTest]
        public void APatientNavigatingToMoreUsingTheTopNavSeesTheHighlightedBottomNavIconDeselectedAndroid(
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
                .Navigation.More();

            AndroidMorePage
                .AssertOnPage(driver).
                Navigation.AppointmentsIcon.AssertNotSelected();
        }

        [NhsAppIOSTest]
        public void APatientNavigatingToMoreUsingTheTopNavSeesTheHighlightedBottomNavIconDeselectedIOS(
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
                .Navigation.More();

            IOSMorePage
                .AssertOnPage(driver).
                Navigation.AppointmentsIcon.AssertNotSelected();
        }
    }
}