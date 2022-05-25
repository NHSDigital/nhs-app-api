using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings.LegalAndCookies;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings.LegalAndCookies.ManageCookies;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Errors;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.LegalAndCookies;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.LegalAndCookies.ManageCookies;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.ConnectionErrors
{
    [TestClass]
    [BusinessRule("BR-IC-01.1", "While within any journey that engages either native view or web view, if Internet connectivity is lost on a device, error screen with try again link/button is displayed which enables retrying to call the view for the User")]
    [BusinessRule("BR-IC-01.3", "When try again link/button is chosen by the User on Internet connectivity error screen, try again action is triggered to retrieve last screen before Internet connectivity was lost.	")]
    [BusinessRule("BR-IC-02.1", "When native back button is chosen while Internet connectivity error screen with try again link/button is displayed, try again action is triggered to retrieve last screen before Internet connectivity was lost")]
    public class TryAgainConnectionErrorTests
    {
        [NhsAppFlakyTest]
        [NhsAppAndroidTest]
        public async Task APatientCanTryAgainWhenThereIsAConnectionErrorWhenLoadingTheNhsAppWebAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Cynthia").FamilyName("Sanders"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore(patient);

            AndroidMorePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCookiesSettings();

            AndroidLegalAndCookiesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToManageCookies();

            AndroidManageCookiesPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidManageCookiesPage
                .AssertOnPage(driver)
                .ToggleOptionalCookies();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidManageCookiesPage
                        .AssertOnPage(driver);
                });

            // For some reason this test consistently fails when there is no delay after resetting the network
            await driver.ResetNetworkAndWait(TimeSpan.FromSeconds(5));

            AndroidCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
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

            AndroidCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
                .TryAgain();

            AndroidManageCookiesPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public async Task APatientCanTryAgainWhenThereIsAConnectionErrorWhenLoadingTheNhsAppWebIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Francis").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver)
                .NavigateToLegalAndCookies();

            IOSLegalAndCookiesPage
                .AssertOnPage(driver)
                .NavigateToManageCookies();

            IOSManageCookiesPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSManageCookiesPage
                .AssertOnPage(driver)
                .ToggleCookies();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSManageCookiesPage
                        .AssertOnPage(driver);
                });

            await driver.ResetNetwork();

            IOSCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
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

            IOSCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
                .TryAgain();

            IOSManageCookiesPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public async Task APatientCanTryAgainByClosingTheErrorWhenThereIsAConnectionErrorWhenLoadingTheNhsAppWebAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Cynthia").FamilyName("Sanders"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore(patient);

            AndroidMorePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCookiesSettings();

            AndroidLegalAndCookiesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToManageCookies();

            AndroidManageCookiesPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidManageCookiesPage
                .AssertOnPage(driver)
                .ToggleOptionalCookies();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidManageCookiesPage
                        .AssertOnPage(driver);
                });

            await driver.ResetNetwork();

            AndroidCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
                .Navigation.Close();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidManageCookiesPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                });
        }

        [NhsAppIOSTest]
        public async Task APatientCanTryAgainByClosingTheErrorWhenThereIsAConnectionErrorWhenLoadingTheNhsAppWebIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Francis").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver)
                .NavigateToLegalAndCookies();

            IOSLegalAndCookiesPage
                .AssertOnPage(driver)
                .NavigateToManageCookies();

            IOSManageCookiesPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSManageCookiesPage
                .AssertOnPage(driver)
                .ToggleCookies();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSManageCookiesPage
                        .AssertOnPage(driver);
                });

            // The web uses the navigator.online property to determine
            // internet connection issues, which is not immediately updated
            // when the network comes back online in Safari.
            await driver.ResetNetworkAndWait(TimeSpan.FromSeconds(5));

            IOSCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
                .Navigation.Close();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSManageCookiesPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                });
        }
        
        [NhsAppFlakyTest]
        [NhsAppAndroidTest]
        public async Task APatientCanUseNativeBackToTryAgainWhenThereIsAConnectionErrorWhenLoadingTheNhsAppWebAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Cynthia").FamilyName("Sanders"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore(patient);

            AndroidMorePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCookiesSettings();

            AndroidLegalAndCookiesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToManageCookies();

            AndroidManageCookiesPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidManageCookiesPage
                .AssertOnPage(driver)
                .ToggleOptionalCookies();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidManageCookiesPage
                        .AssertOnPage(driver);
                });

            await driver.ResetNetwork();

            driver.PressBackButton();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidManageCookiesPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                });
        }

        [NhsAppIOSTest]
        public async Task APatientCanUseNativeBackToTryAgainWhenThereIsAConnectionErrorWhenLoadingTheNhsAppWebIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Francis").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver)
                .NavigateToLegalAndCookies();

            IOSLegalAndCookiesPage
                .AssertOnPage(driver)
                .NavigateToManageCookies();

            IOSManageCookiesPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSManageCookiesPage
                .AssertOnPage(driver)
                .ToggleCookies();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSManageCookiesPage
                        .AssertOnPage(driver);
                });

            // The web uses the navigator.online property to determine
            // internet connection issues, which is not immediately updated
            // when the network comes back online in Safari.
            await driver.ResetNetworkAndWait(TimeSpan.FromSeconds(5));

            driver.SwipeBack();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSManageCookiesPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSCloseSlimTryAgainConnectionErrorPage
                        .AssertOnPage(driver);
                });
        }
    }
}