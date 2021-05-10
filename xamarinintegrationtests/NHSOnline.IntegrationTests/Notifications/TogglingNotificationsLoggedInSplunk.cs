using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.15", "Enabling or disabling notifications settings on a device writes appropriate logs in Splunk for analytic purposes")]
    public class TogglingNotificationsLoggedInSplunk
    {
        [NhsAppManualTest("NHSO-14110", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientEnablesOrDisablesNotificationsSettingsOnADeviceWritesLogToSplunkForAnalyticsIOS() { }
    }
}