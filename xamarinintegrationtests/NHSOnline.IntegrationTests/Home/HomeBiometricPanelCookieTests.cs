using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [BusinessRule("BR-HOM-01.2", "Biometrics call out is not displayed when it has been previously dismissed")]
    [BusinessRule("BR-LOG-03.1", "Continuing with NHS login displays NHS login when the before you start screen has previously been acknowledged")]
    [TestClass]
    public class HomeBiometricPanelCookieTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveDoesNotSeeTheBiometricsPanelWhenTheAppIsRelaunchedAfterDismissingItAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            var homePage = AndroidLoggedInHomePage
                .AssertOnPage(driver);
            homePage.PageContent.AssertBiometricPanelVisible();
            homePage.PageContent.DismissBiometricPanel();
            homePage.PageContent.AssertBiometricPanelNotVisible();

            driver.CloseApp();

            driver.LaunchApp();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertBiometricPanelNotVisible();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveDoesNotSeeTheBiometricsPanelWhenTheAppIsRelaunchedAfterDismissingItIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            var homePage = IOSLoggedInHomePage
                .AssertOnPage(driver);
            homePage.PageContent.AssertBiometricPanelVisible();
            homePage.PageContent.DismissBiometricPanel();
            homePage.PageContent.AssertBiometricPanelNotVisible();

            driver.CloseApp();

            driver.LaunchApp();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertBiometricPanelNotVisible();
        }
    }
}
