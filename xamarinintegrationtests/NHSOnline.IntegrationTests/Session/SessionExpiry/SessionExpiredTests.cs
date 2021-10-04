using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session.SessionExpiry
{
    [TestClass]
    [BusinessRule("BR-LOG-07.5","Log out due to a session timeout displays the logged out home screen with a yellow banner message")]
    public class SessionExpiredTests
    {
        private static readonly TimeSpan SessionExpiredDuration = TimeSpan.FromMinutes(2.5);
        private const string HomeHelpLinkPath = "/";

        [NhsAppAndroidTest(AndroidBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientLeavesTheAppOpenLongEnoughForTheSessionToExpireAndSeesTheLoggedOutHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Des").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);

            Thread.Sleep(SessionExpiredDuration);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }

        [NhsAppIOSTest(IOSBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientLeavesTheAppOpenLongEnoughForTheSessionToExpireAndSeesTheLoggedOutHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Des").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);

            Thread.Sleep(SessionExpiredDuration);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }

        [NhsAppAndroidTest(AndroidBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientOpensABrowserOverlayAndLeavesTheAppOpenLongEnoughForTheSessionToExpireAndSeesTheLoggedOutHomeScreenWhenTheyCloseTheOverlayAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Des").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            var sessionExpiryDelay = new DelayExecutionTimer(SessionExpiredDuration);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToHelp(patient);

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnHelpPageByText(driver, HomeHelpLinkPath);

            sessionExpiryDelay.Wait();

            AndroidAppTab
                .AssertOnHelpPageByText(driver, HomeHelpLinkPath)
                .ReturnToApp();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }

        [NhsAppIOSTest(IOSBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientOpensABrowserOverlayAndLeavesTheAppOpenLongEnoughForTheSessionToExpireAndSeesTheLoggedOutHomeScreenWhenTheyCloseTheOverlayIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Des").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            var sessionExpiryDelay = new DelayExecutionTimer(SessionExpiredDuration);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToHelp();

            IOSAppTab
                .AssertOnHelpPageByText(driver, HomeHelpLinkPath);

            sessionExpiryDelay.Wait();

            IOSAppTab
                .AssertOnHelpPageByText(driver, HomeHelpLinkPath)
                .ReturnToApp();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }

        [NhsAppAndroidTest(AndroidBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientOnTermsAndConditionsLeavesTheAppOpenLongEnoughForTheSessionToExpireAndSeesTheLoggedOutHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Arthur").FamilyName("Sleep"));
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
                .AssertPageContent();

            Thread.Sleep(SessionExpiredDuration);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }

        [NhsAppIOSTest(IOSBrowserStackCapability.ExtendedIdleTimeout)]
        public void APatientOnTermsAndConditionsLeavesTheAppOpenLongEnoughForTheSessionToExpireAndSeesTheLoggedOutHomeScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Arthur").FamilyName("Sleep"));
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
                .AssertPageContent();

            Thread.Sleep(SessionExpiredDuration);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertSessionExpired();
        }
    }
}