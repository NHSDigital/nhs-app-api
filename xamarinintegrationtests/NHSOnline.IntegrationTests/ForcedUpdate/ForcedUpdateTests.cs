using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
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
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidForcedUpdatePage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidForcedUpdatePage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Upgrade();

            var androidNhsAppPlayStorePage = AndroidNhsAppPlayStorePage
                .AssertOnPage(driver);

            androidNhsAppPlayStorePage.AssertPageElements();

            TransitoryErrorHandler.HandleSpecificFailure()
                .Alternate(() =>
                    {
                        // Play store app has now changed layout and Button no longer has the text property.
                        androidNhsAppPlayStorePage.InstallAvailableFallback();
                    },
                    "A button with text: 'Install' should be present.",
                    () =>
                    {
                        // Old layout is now the fallback - may still be applicable on some devices
                        androidNhsAppPlayStorePage.InstallAvailable();
                    });
        }

        [NhsAppIOSTest]
        public void APatientWithAnOutDatedAppSeesTheForcedUpgradePageWhenLoggingInIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSForcedUpdatePage
                .AssertOnPage(driver);

            driver.SwipeBack();

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
        public async Task APatientSeesTheForcedUpgradeErrorPageWhenLoggingInWithNoNetworkCanGoTo111AndCanNavigateBackToLogInAndroid(IAndroidDriverWrapper driver)
        {
            using var extendedTimeout = ExtendedTimeout.FromSeconds(30);

            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidForcedUpdateErrorPage
                        .AssertOnPage(driver)
                        .AssertPageElements()
                        .BackToLogin();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidGettingStartedPage
                        .AssertOnPage(driver);
                });

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Navigation.Close();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();

            await driver.ResetNetwork();

            AndroidForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .GoTo111();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            // Need to reuse the same page instance in the known issue fallback assertion
            // as creating a new one will result in a new context being grabbed.
            AndroidBrowserOverlay111Page? browserOverlay = null;

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    browserOverlay = AndroidBrowserOverlay111Page.AssertInBrowserOverlay(driver);
                    browserOverlay
                        .AssertOnPage()
                        .ReturnToApp();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    browserOverlay?.AssertNoInternet();
                });

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSBrowserStackCapability.NoNetwork)]
        public async Task APatientSeesTheForcedUpgradeErrorPageWhenLoggingInWithNoNetworkCanGoTo111AndCanNavigateBackToLogInIOS(IIOSDriverWrapper driver)
        {
            using var extendedTimeout = ExtendedTimeout.FromSeconds(30);

            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSForcedUpdateErrorPage
                        .AssertOnPage(driver)
                        .AssertPageElements()
                        .BackToLogin();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSGettingStartedPage
                        .AssertOnPage(driver);
                });

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .Navigation.Close();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();

            await driver.ResetNetwork();

            IOSForcedUpdateErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .GoTo111();

            // Need to reuse the same page instance in the known issue fallback assertion
            // as creating a new one will result in a new context being grabbed.
            IOSBrowserOverlay111Page? browserOverlay = null;

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    browserOverlay = IOSBrowserOverlay111Page.AssertInBrowserOverlay(driver);
                    browserOverlay
                        .AssertOnPage()
                        .ReturnToApp();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    browserOverlay?.AssertNoInternet();
                });

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}