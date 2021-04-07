using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-SET-04.7", "Enabling notifications on a device where another user of the same device has currently got notifications enabled, enables notifications for the current user and disables notifications for the other user")]
    public class EnableNotificationsWhenAnotherUserHasEnabledOnDevice
    {
        [NhsAppManualTest("NHSO-14099", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientEnabledNotificationsWhenAnotherUserHasAlreadyGrantedPermissionAndEnabledNotificationsOnTheDeviceIOS() { }
    }
}