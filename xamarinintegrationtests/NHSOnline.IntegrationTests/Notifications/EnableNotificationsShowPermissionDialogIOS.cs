using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Settings;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-SET-04.3", "Enabling notifications for the first time on the device displays a prompt to grant appropriate permissions")]
    public class EnableNotificationsShowPermissionDialogIOS
    {
        [NhsAppIOSTest]
        public void APatientEnablesNotificationsForTheFirstTimeOnTheDeviceIsShownAPromptToGrantAppropriatePermissionsIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent.Continue();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Settings();

            IOSSettingsPage
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