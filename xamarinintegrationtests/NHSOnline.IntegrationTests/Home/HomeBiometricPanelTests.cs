using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    [BusinessRule("BR-HOM-01.3", "Biometrics call out is not displayed when the device is not compatible")]
    public class HomeBiometricPanelTests
    {
        [NhsAppManualTest("NHSO-15687", "Unable to test with a device below Android 6")]
        public void APatientThatHasADeviceNotCapableOfBiometricsDoesNotSeeTheBiometricPanelOnHomePageAndroid() { }
    }
}