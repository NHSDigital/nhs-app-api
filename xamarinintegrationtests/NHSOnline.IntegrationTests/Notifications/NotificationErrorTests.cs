using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.10", "Failure to enable notifications due to failure to obtain PNS code displays an error")]
    [BusinessRule("BR-NOT-04.11", "Navigating to manage notification when the users current notification registration fails to retrieve displays an error")]
    [BusinessRule("BR-NOT-04.12", "Enabling or disabling the notifications setting for the device when the request fails displays an error")]
    public class NotificationErrorTests
    {
        [NhsAppManualTest("NHSO-14226", "Unable to create error scenario for browserstack tests")]
        public void APatientFailingToObtainApnsCodeIsShownAnErrorAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Unable to create error scenario for browserstack tests")]
        public void APatientFailingToObtainApnsCodeIsShownAnErrorIos() { }

        [NhsAppManualTest("NHSO-14226", "Unable to create error scenario for browserstack tests")]
        public void APatientFailingToObtainNotificationRegistrationIsShownAnErrorAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Unable to create error scenario for browserstack tests")]
        public void APatientFailingToObtainNotificationRegistrationIsShownAnErrorIos() { }

        [NhsAppManualTest("NHSO-14226", "Unable to create error scenario for browserstack tests")]
        public void APatientEnablingNotificationsWhenTheRequestFailsIsShownAnErrorAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Unable to create error scenario for browserstack tests")]
        public void APatientEnablingNotificationsWhenTheRequestFailsIsShownAnErrorIos() { }
    }
}