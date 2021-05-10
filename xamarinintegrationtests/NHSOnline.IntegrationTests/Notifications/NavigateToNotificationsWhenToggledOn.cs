using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.01", "Navigating to manage notifications from the settings menu when the notifications for the device are enabled displays the manage notifications screen with the current registration status for the device toggled to on")]
    public class NavigateToNotificationsWhenToggledOn
    {
        [NhsAppManualTest("NHSO-14095", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientNavigatesToNotificationsWhenToggledOnIOS() { }
    }
}