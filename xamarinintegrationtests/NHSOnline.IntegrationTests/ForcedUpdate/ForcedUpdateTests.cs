using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.ForcedUpdate
{
    [TestClass]
    [NhsAppUpgradeTest]
    [BusinessRule("BR-LOG-13.2", "Continuing with NHS login when app is below the min required app version displays a shutter page")]
    [BusinessRule("BR-LOG-13.6", "Continuing with NHS login when request for min app version has failed displays an error")]
    public class ForcedUpdateTests
    {
        [NhsAppAndroidTest(AndroidBrowserStackCapability.SignInToGoogle)]
        public void APatientWithAnOutDatedAppSeesTheForcedUpgradePageWhenLoggingInAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidForcedUpdatePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Upgrade();

            AndroidNhsAppPlayStorePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .InstallAvailable();
        }

        [NhsAppIOSTest]
        public void APatientWithAnOutDatedAppSeesTheForcedUpgradePageWhenLoggingInIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSForcedUpdatePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Upgrade();

            IOSNhsAppAppStorePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidBrowserStackCapability.NoNetwork)]
        public void APatientSeesTheForcedUpgradeErrorPageWhenLoggingInWithNoNetworkInAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .BackToHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSBrowserStackCapability.NoNetwork)]
        public void APatientSeesTheForcedUpgradeErrorPageWhenLoggingInWithNoNetworkInIOS(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .BackToHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}