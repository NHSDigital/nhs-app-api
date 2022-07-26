using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.More
{
    [TestClass]
    [BusinessRule("BR-LOG-07.1", "Logging out of the app displays the logged out home screen")]
    public class LogoutTests
    {
        [NhsAppCanaryTest]
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanLogOutFromMoreScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.Logout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppCanaryTest]
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineUsingKeyboardCanLogOutFromMoreScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Alan").FamilyName("Shepard"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMore(patient);

            AndroidMorePage
                .AssertOnPage(driver)
                .KeyboardNavigateToLogout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public async Task APatientWithProofLevelNineAndDeviceInAirplaneModeCanLogOutFromMoreScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new VisionPatient()
                .WithName(b => b.GivenName("Bob").FamilyName("Behnken"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            await driver.EnableAirplaneMode();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.Logout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanLogOutFromMoreScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Doug").FamilyName("Hurley"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .PageContent.Logout();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        [Ignore("Disabling network has no effect on iOS currently. Raised with Browserstack")]
        public async Task APatientWithProofLevelNineAndDeviceNetworkIsDisabledCanLogOutFromMoreScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new VisionPatient()
                .WithName(b => b.GivenName("Scott").FamilyName("Kelly"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            await driver.DisableNetwork();

            IOSMorePage
                .AssertOnPage(driver)
                .PageContent.Logout();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}