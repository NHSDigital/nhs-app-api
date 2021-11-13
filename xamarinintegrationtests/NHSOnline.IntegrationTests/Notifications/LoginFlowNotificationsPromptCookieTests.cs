using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-03.8", "Log in on a device when the notifications prompt has been viewed on the device previously does not display the notifications prompt")]
    public class LoginFlowNotificationsPromptCookieTests
    {
        [NhsAppAndroidTest]
        public void APatientDoesNotSeeTheNotificationsPromptWhenTheAppIsRelaunchedAfterDismissingItAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Norman").FamilyName("Price"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptOutOfUserResearch();

            AndroidManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .NoDontSendNotifications()
                .Continue();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);

            driver.CloseApp();
            driver.LaunchApp();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientDoesNotSeeTheNotificationsPromptWhenTheAppIsRelaunchedAfterDismissingItIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Norman").FamilyName("Price"));
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
                .PageContent.OptOutOfUserResearch();

            IOSManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .NoDontSendNotifications()
                .Continue();

            IOSLoggedInHomePage
                .AssertOnPage(driver);

            driver.CloseApp();
            driver.LaunchApp();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}