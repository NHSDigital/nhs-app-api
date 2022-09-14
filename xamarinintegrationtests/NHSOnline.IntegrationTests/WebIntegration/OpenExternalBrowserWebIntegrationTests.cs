using AngleSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;
using NHSOnline.IntegrationTests.UI.Drivers.Native.IOS;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    public class OpenExternalBrowserWebIntegrationTests
    {
        private const string TestUrl = "http://stubs.local.bitraft.io:8080/nhsuk/111";
        private const string PageHeader = "111";
        private const string UrlText = "stubs.local.bitraft.io:8080/nhsuk/111";

        // Setting to S10 and os 9 as the new default exposed an issue on differing versions
        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.GalaxyS10, OSVersion = AndroidOSVersion.Nine)]
        public void APatientCanAccessAWebIntegrationAndCallOpenExternalBrowserAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            NavigateToOpenExternalBrowserViaPkbHospitalAppointmentsAndroid(driver);

            AndroidOpenExternalBrowserPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .EnterExternalUrl(driver, new Url(TestUrl))
                .ClickExternalBrowserButton(driver);

           AndroidChromeApp.VerifyUrl(driver, UrlText);
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAWebIntegrationAndCallOpenExternalBrowserIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            NavigateToOpenExternalBrowserViaPkbHospitalAppointmentsIOS(driver);

            IOSOpenExternalBrowserPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .EnterExternalURL(driver, new Url(TestUrl))
                .ClickExtenalBrowserButton(driver);

            IOSSafariApp.VerifyUrl(driver, PageHeader);
        }

        private static void NavigateToOpenExternalBrowserViaPkbHospitalAppointmentsAndroid(
            IAndroidDriverWrapper driver)
        {

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

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
                .AssertNativeHeader()
                .NavigateToOpenExternalBrowser();
        }

        private static void NavigateToOpenExternalBrowserViaPkbHospitalAppointmentsIOS(IIOSDriverWrapper driver)
        {
            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .NavigateToOpenExternalBrowser();
        }
    }
}