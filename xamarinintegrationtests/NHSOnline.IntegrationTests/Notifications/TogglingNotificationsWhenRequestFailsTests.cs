using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.12", "Enabling or disabling the notifications setting for the device when the request fails displays an error")]
    public class TogglingNotificationsWhenRequestFailsTests
    {
        [NhsAppManualTest("NHSO-14102", "BrowserStack requires Enterprise Certificate signing to enable notifications on iOS")]
        public void APatientTogglesNotificationsWhenRequestFailsIsShownErrorIOS() { }
    }
}