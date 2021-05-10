using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.02", "Navigating to manage notifications from the settings menu when the notifications for the device are disabled displays the manage notifications screen with the current registration status for the device toggled to off")]
    public class NavigateToNotificationsWhenToggledOff
    {
        [NhsAppManualTest("NHSO-14096", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientNavigatesToNotificationsWhenToggledOffIOS() { }
    }
}