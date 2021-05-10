using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.9", "Navigating to notification settings when notifications have been disabled for the device in the device settings displays an error message")]
    public class NavigateToNotificationsWhenDisabledOnDevice
    {
        [NhsAppManualTest("NHSO-14101", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientNavigatesToNotificationsWhenDisabledOnDeviceIOS() { }
    }
}