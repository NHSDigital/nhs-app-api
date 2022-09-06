using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.DeviceAndOperatingSystemCompatibility
{
    [TestClass]
    public class LoginWithP9OlderOSVersionTests
    {
        [NhsAppCanaryTest]
        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhoneX, OSVersion = IOSVersion.Eleven)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS11(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientInPreIos13(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhoneXR, OSVersion = IOSVersion.Twelve)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS12(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientInPreIos13(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone11Pro, OSVersion = IOSVersion.Thirteen)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS13(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone12, OSVersion = IOSVersion.Fourteen)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS14(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone13, OSVersion = IOSVersion.Fifteen)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS15(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone12Pro, OSVersion = IOSVersion.SixteenBeta)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS16(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppCanaryTest]
        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.GalaxyS9, OSVersion = AndroidOSVersion.Eight)]
        public void PatientWithProofLevelNineCanSuccessfullyLogInOnAndroid8(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel3, OSVersion = AndroidOSVersion.Nine)]
        public void PatientWithProofLevelNineCanSuccessfullyLogInOnAndroid9(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.GalaxyS20Ultra, OSVersion = AndroidOSVersion.Ten)]
        public void PatientWithProofLevelNineCanSuccessfullyLogInOnAndroid10(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.GalaxyS21, OSVersion = AndroidOSVersion.Eleven)]
        public void PatientWithProofLevelNineCanSuccessfullyLogInOnAndroid11(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel6, OSVersion = AndroidOSVersion.Twelve)]
        public void PatientWithProofLevelNineCanSuccessfullyLogInOnAndroid12(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.Pixel6Pro, OSVersion = AndroidOSVersion.Thirteen)]
        public void PatientWithProofLevelNineCanSuccessfullyLogInOnAndroid13(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}