using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-02.1", "Receiving a push notification on a device where notifications are enabled and the app is closed displays the push notification on the device")]
    [BusinessRule("BR-NOT-02.2", "Receiving a push notification on a device where notifications are enabled and the app is open displays the push notification on the device")]
    public class ReceivingNotificationsTests
    {
        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientIsShownANotificationWhenTheAppIsClosedAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientIsShownANotificationWhenTheAppIsClosedIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientIsShownANotificationWhenTheAppIsOpenAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientIsShownANotificationWhenTheAppIsOpenIos() { }
    }
}