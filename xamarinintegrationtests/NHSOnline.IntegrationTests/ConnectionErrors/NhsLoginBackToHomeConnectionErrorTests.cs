using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Errors;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.ConnectionErrors
{
    [TestClass]
    [BusinessRule("BR-IC-01.4", "While within any journey that engages Web Integration, if Internet connectivity is lost on a device, error screen with back to home link is displayed which enables going back to Home view for the User")]
    [BusinessRule("BR-IC-02.2", "When native back button is chosen while Internet connectivity error screen without try again button is displayed, back to NHS App view action is triggered to redirect the User to logged in Home view")]
    public class NhsLoginBackToHomeConnectionErrorTests
    {
        [NhsAppAndroidTest]
        public async Task APatientCanGoBackToTheLoggedOutHomePageWhenThereIsAConnectionErrorDuringNhsLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Dewey").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidCloseSlimBackToHomeConnectionErrorPage
                        .AssertOnPage(driver)
                        .BackToHome();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidTermsAndConditionsPage
                        .AssertOnPage(driver);
                });

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public async Task APatientCanGoBackToTheLoggedOutHomePageWhenThereIsAConnectionErrorDuringNhsLoginIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Reese").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSCloseSlimBackToHomeConnectionErrorPage
                        .AssertOnPage(driver)
                        .BackToHome();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSTermsAndConditionsPage
                        .AssertOnPage(driver);
                });

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public async Task APatientCanUseNativeBackToGoBackToTheLoggedOutHomePageWhenThereIsAConnectionErrorDuringNhsLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Dewey").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidCloseSlimBackToHomeConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidTermsAndConditionsPage
                        .AssertOnPage(driver);
                });

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public async Task APatientCanUseNativeBackToGoBackToTheLoggedOutHomePageWhenThereIsAConnectionErrorDuringNhsLoginIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Reese").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSCloseSlimBackToHomeConnectionErrorPage
                        .AssertOnPage(driver);
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSTermsAndConditionsPage
                        .AssertOnPage(driver);
                });

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public async Task APatientCanCloseTheConnectionErrorScreenToGoBackToTheLoggedOutHomePageWhenThereIsAConnectionErrorDuringNhsLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Dewey").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidCloseSlimBackToHomeConnectionErrorPage
                        .AssertOnPage(driver)
                        .Navigation.Close();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidTermsAndConditionsPage
                        .AssertOnPage(driver);
                });

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public async Task APatientCanCloseTheConnectionErrorScreenToGoBackToTheLoggedOutHomePageWhenThereIsAConnectionErrorDuringNhsLoginIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Reese").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSCloseSlimBackToHomeConnectionErrorPage
                        .AssertOnPage(driver)
                        .Navigation.Close();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSTermsAndConditionsPage
                        .AssertOnPage(driver);
                });

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}