using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session
{
    [TestClass]
    [BusinessRule("BR-LOG-07.2","User is prompted to logout or extend their session when their session reaches predefined time until it expires")]
    [BusinessRule("BR-LOG-07.3","Extending a session when prompted closes the prompt, extends the session and remains on the same screen")]
    [BusinessRule("BR-LOG-07.4","Logging out when prompted when a session is expiring logs the user out")]
    public class SessionExpiryDialogExtendTimeoutTests
    {
        private static readonly TimeSpan SessionExpiryDialogDuration = TimeSpan.FromMinutes(1);

        [NhsAppAndroidTest]
        public void APatientSeesTheSessionExpiryDialogAndChoosesToExtendTheirSessionAndWhenTheySeeTheDialogAgainTheyChooseToLogOutAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Des").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver);

            Thread.Sleep(SessionExpiryDialogDuration);

            AndroidSessionExpiryPrompt
                .AssertDisplayed(driver)
                .ExtendSession();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver);

            Thread.Sleep(SessionExpiryDialogDuration);

            AndroidSessionExpiryPrompt
                .AssertDisplayed(driver)
                .Logout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientSeesTheSessionExpiryDialogAndChoosesToExtendTheirSessionAndWhenTheySeeTheDialogAgainTheyChooseToLogOutIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Des").FamilyName("Krypton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver);

            Thread.Sleep(SessionExpiryDialogDuration);

            IOSSessionExpiryPrompt
                .AssertDisplayed(driver)
                .ExtendSession();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver);

            Thread.Sleep(SessionExpiryDialogDuration);

            IOSSessionExpiryPrompt
                .AssertDisplayed(driver)
                .Logout();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}