using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.DeepLinks
{
    [TestClass]
    public class DeepLinksTests
    {
        [NhsAppAndroidTest]
        [NhsAppCanaryTest]
        public void BackgroundAppDeepLink(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);

            driver.BackgroundApp();

            driver.OpenChromeApp()
                .NavigateToDeepLinkLauncher();

            AndroidDeepLinkLauncherPage
                .AssertOnPage(driver)
                .ClickLink();

            AndroidDeepLinkAppChoice
                .AssertDisplayed(driver)
                .ChooseNhsApp();

            AndroidAppointmentsPage.AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        [NhsAppCanaryTest]
        public void ClosedAppDeepLink(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            driver.CloseApp();

            driver.OpenChromeApp()
                .NavigateToDeepLinkLauncher();

            AndroidDeepLinkLauncherPage
                .AssertOnPage(driver)
                .ClickLink();

            AndroidDeepLinkAppChoice
                .AssertDisplayed(driver)
                .ChooseNhsApp();

            LoginProcess.LogAndroidPatientIn(driver, patient);
            AndroidAppointmentsPage.AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel3, OSVersion = AndroidOSVersion.Nine)]
        public void DeepLinkToSamePage(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage.AssertOnPage(driver);

            driver.BackgroundApp();

            driver.OpenChromeApp()
                .NavigateToDeepLinkWebIntegrationLauncher();

            AndroidDeepLinkWebIntegrationLauncherPage
                .AssertOnPage(driver)
                .ClickLink();

            AndroidDeepLinkAppChoice
                .AssertDisplayed(driver)
                .ChooseNhsApp();

            AndroidRedirectorPage.AssertOnPage(driver);

            AndroidErsPage.AssertOnPage(driver);

            driver.BackgroundApp();

            // We just need to click the link again as the DeepLinkLauncher page is already
            // open in the Chrome App
            driver.OpenChromeApp()
                .ClickLink();

            AndroidDeepLinkAppChoice
                .AssertDisplayed(driver)
                .ChooseNhsApp();

            AndroidRedirectorPage.AssertOnPage(driver);

            AndroidErsPage.AssertOnPage(driver);
        }
    }
}