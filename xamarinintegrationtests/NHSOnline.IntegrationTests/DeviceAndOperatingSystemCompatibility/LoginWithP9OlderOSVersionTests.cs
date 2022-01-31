using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.DeviceAndOperatingSystemCompatibility
{
    [TestClass]
    public class LoginWithP9OlderOSVersionTests
    {
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
    }
}