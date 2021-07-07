using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.18", "Notification settings that have been enabled should be persisted through an update of the app")]
    [BusinessRule("BR-NOT-04.19", "Notification settings that have been disabled should be persisted through an update of the app")]
    [BusinessRule("BR-NOT-04.20", "Notification settings that have been enabled in the legacy app should be persisted through to the Xamarin app")]
    [BusinessRule("BR-NOT-04.21", "Notification settings that have been disabled in the legacy app should be persisted through to the Xamarin app")]
    public class PersistingNotificationDecisionTests
    {
        [NhsAppManualTest("NHSO-14226", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsEnabledHasTheirNotificationDecisionPersistThroughAXamarinAppUpdateAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsEnabledHasTheirNotificationDecisionPersistThroughAXamarinAppUpdateIos() { }

        [NhsAppManualTest("NHSO-14226", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsDisabledHasTheirNotificationDecisionPersistThroughAXamarinAppUpdateAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsDisabledHasTheirNotificationDecisionPersistThroughAXamarinAppUpdateIos() { }

        [NhsAppManualTest("NHSO-14226", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsEnabledHasTheirNotificationDecisionPersistThroughALegacyAppUpdateAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsEnabledHasTheirNotificationDecisionPersistThroughALegacyAppUpdateIos() { }

        [NhsAppManualTest("NHSO-14226", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsDisabledHasTheirNotificationDecisionPersistThroughALegacyAppUpdateAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Upgrading the app is not feasible for browserstack tests")]
        public void APatientWithNotificationsDisabledHasTheirNotificationDecisionPersistThroughALegacyAppUpdateIos() { }
    }
}