using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-SET-01.11", "Navigating to NHS login displays the Manage your NHS login screen")]
    public class NhsLoginSettingsWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessNhsLoginSettingsFromMoreScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Norman").FamilyName("Bates"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAccountAndSettings();

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            AndroidNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanKeyboardNavigateToAccessNhsLoginSettingsFromMoreScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Norman").FamilyName("Long"));
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
                .PageContent.NavigateToNhsLogin();

            AndroidNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessNhsLoginSettingsFromMoreScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Nigel").FamilyName("Linger"));
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
                .NavigateToNhsLoginSettings();

            IOSNhsLoginSettingsPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }
    }
}