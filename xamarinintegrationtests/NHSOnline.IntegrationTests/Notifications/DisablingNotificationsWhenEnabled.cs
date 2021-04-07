using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-SET-04.8", "Disabling notifications successfully disables notifications on the device")]
    public class DisablingNotificationsWhenEnabled
    {
        [NhsAppManualTest("NHSO-14100", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientDisablesNotificationsWhenAlreadyEnabledIOS() { }
    }
}