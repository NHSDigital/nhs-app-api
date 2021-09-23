using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Errors;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Errors;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.ConnectionErrors
{
    [TestClass]
    public class NhsLoginUpliftBackToHomeConnectionErrorTests
    {
        [NhsAppAndroidTest]
        public async Task APatientCanGoBackToHomeWhenThereIsAConnectionErrorUpliftingAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithProofLevel5()
                .WithName(b => b.GivenName("Lois").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver);

            await driver.EnableAirplaneMode();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .NavigateToInternalPage();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    AndroidBackToHomeConnectionErrorPage
                        .AssertOnPage(driver)
                        .BackToHome();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    AndroidStubbedLoginInternalPage
                        .AssertOnPage(driver);
                });

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public async Task APatientCanGoBackToHomeWhenThereIsAConnectionErrorUpliftingIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithProofLevel5()
                .WithName(b => b.GivenName("Hal").FamilyName("Wilkerson"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver);

            await driver.DisableNetwork();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .NavigateToInternalPage();

            KnownIssue.BrowserStackNetworkChangeFailed()
                .ShouldExpect(() =>
                {
                    IOSBackToHomeConnectionErrorPage
                        .AssertOnPage(driver)
                        .BackToHome();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    IOSStubbedLoginInternalPage
                        .AssertOnPage(driver);
                });

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}