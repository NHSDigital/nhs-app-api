using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.Notifications;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-04.3", "Enabling notifications for the first time on the device displays a prompt to grant appropriate permissions")]
    public class EnableNotificationsShowPermissionDialogIOS
    {
        [NhsAppIOSTest]
        public void APatientEnablesNotificationsForTheFirstTimeOnTheDeviceIsShownAPromptToGrantAppropriatePermissionsIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNotifications();

            IOSNotificationsPage
                .AssertOnPage(driver)
                .IOSPageContent.ToggleOnNotifications();

            IOSNotificationsPermissionDialog
                .AssertDisplayed(driver);
        }
    }
}