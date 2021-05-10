using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.4", "Granting appropriate permissions when enabling notifications enables notifications on the device (iOS)")]
    public class GrantingNotificationPermissionsIOS
    {
        [NhsAppManualTest("NHSO-14088", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientGrantsPermissionWhenEnablingNotificationsIOS() { }
    }
}