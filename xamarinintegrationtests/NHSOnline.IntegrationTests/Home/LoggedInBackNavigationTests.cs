using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-NB-01.2", "Invoking native back on Android on the logged in home screen shows the logout prompt and the user can log out")]
    public class LoggedInBackNavigationTests
    {
        [NhsAppAndroidTest]
        public void APatientPressesTheBackButtonOnTheHomeScreenAndCanThenLogOutAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLogoutPrompt
                .AssertDisplayed(driver)
                .Logout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanKeepPressingTheBackButtonFromANestedRouteAllTheWayBackToTheHomeScreenIOS(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation
                .NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidMorePage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanSwipeBackFromANestedRouteAllTheWayBackToTheHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation
                .NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSMorePage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}