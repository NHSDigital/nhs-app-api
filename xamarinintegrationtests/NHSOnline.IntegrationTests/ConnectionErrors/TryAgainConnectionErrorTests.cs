using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings.LegalAndCookies;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings.LegalAndCookies.ManageCookies;
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

            await driver.ResetNetwork();

            AndroidCloseSlimTryAgainConnectionErrorPage
                .AssertOnPage(driver)
                .TryAgain();

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
                .TryAgain();

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

            await driver.ResetNetwork();

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

            await driver.ResetNetwork();

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