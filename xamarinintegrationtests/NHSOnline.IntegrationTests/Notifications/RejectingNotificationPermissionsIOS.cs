using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-SET-04.5", "Rejecting request for permissions when turning on notifications on iOS displays an error message")]
    public class RejectingNotificationPermissionsIOS
    {
        [NhsAppManualTest("NHSO-14097", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientRejectsPermissionWhenEnablingNotificationsIOS() { }
    }
}