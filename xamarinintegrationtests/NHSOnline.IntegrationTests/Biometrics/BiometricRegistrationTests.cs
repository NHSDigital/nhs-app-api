using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Biometrics
{
    [TestClass]
    [BusinessRule("BR-SET-02.1", "Navigating to biometrics settings screen when a user has registered Touch ID on the device shows the users current biometrics settings")]
    [BusinessRule("BR-SET-02.3", "Navigating to biometrics settings screen when a user has not registered Face ID on the device show the users current biometrics settings")]
    [BusinessRule("BR-SET-02.5", "Navigating to biometrics settings screen when a user has not registered Fingerprint ID on the device show the users current biometrics settings")]
    public class BiometricRegistrationTests
    {
        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel3, OSVersion = AndroidOSVersion.Ten)]
        public void APatientWithBiometricsCanNavigateToTheBiometricRegistrationScreenAndroid(IAndroidDriverWrapper driver)
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
                .PageContent.NavigateToAccountAndSettings();;

            AndroidAccountSettingsPage
                .AssertOnPage(driver)
                .NavigateToFingerprintFaceIrisBiometrics();

            AndroidFingerprintFaceIrisPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone11Pro, OSVersion = IOSVersion.Thirteen)]
        public void APatientWithFaceIdWithCanNavigateToTheBiometricRegistrationScreenIOS(IIOSDriverWrapper driver)
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
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver)
                .AssertFaceIdMenuItemElements()
                .NavigateToFaceIdBiometrics();

            IOSFaceIdRegistrationPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone8, OSVersion = IOSVersion.Thirteen)]
        public void APatientWithTouchIdWithCanNavigateToTheBiometricRegistrationScreenIOS(IIOSDriverWrapper driver)
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
                .NavigateToAccountAndSettings();

            IOSAccountSettingsPage
                .AssertOnPage(driver)
                .AssertTouchIdMenuItemElements()
                .NavigateToTouchIdBiometrics();

            IOSTouchIdRegistrationPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}