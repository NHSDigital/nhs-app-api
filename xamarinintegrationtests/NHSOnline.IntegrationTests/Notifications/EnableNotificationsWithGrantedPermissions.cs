using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.6", "Enabling notifications when permissions have previously been granted on the device successfully enables notifications for the device")]
    public class EnableNotificationsWithGrantedPermissions
    {
        [NhsAppManualTest("NHSO-14098", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientEnablesNotificationsWhenPermissionHasBeenPreviouslyGrantedIOS() { }
    }
}