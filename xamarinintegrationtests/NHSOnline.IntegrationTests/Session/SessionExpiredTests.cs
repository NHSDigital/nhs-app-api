using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session
{
    [TestClass]
    [BusinessRule("BR-LOG-07.5","Log out due to a session timeout displays the logged out home screen with a yellow banner message")]
    public class SessionExpiredTests
    {
        private static readonly TimeSpan SessionExpiredDuration = TimeSpan.FromMinutes(2.5);

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
    }
}