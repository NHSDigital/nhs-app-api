using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.DeviceAndOperatingSystemCompatibility
{
    [TestClass]
    public class LoginWithTabletTests
    {
        [NhsAppCanaryTest]
        [NhsAppIOSTest(IOSDevice = IOSDevice.iPadAir4, OSVersion = IOSVersion.Fourteen)]
        public void APatientCanSuccessfullyLogInOnAnIpadAirIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnTabletPage(driver);
        }

        [NhsAppCanaryTest]
        [NhsAppAndroidTest(AndroidDevice = AndroidDevice.GalaxyTabS8, OSVersion = AndroidOSVersion.Twelve)]
        public void APatientCanSuccessfullyLogInOnAGalaxyTabAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Buzz").FamilyName("Aldrin"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnTabletPage(driver);
        }
    }
}