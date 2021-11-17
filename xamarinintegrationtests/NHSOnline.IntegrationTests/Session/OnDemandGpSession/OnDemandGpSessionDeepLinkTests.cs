using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Session.OnDemandGpSession
{
    [TestClass]
    public class OnDemandGpSessionDeepLinkTests
    {
        [NhsAppAndroidTest]
        public void APatientAccessingAGpServiceClicksADeepLinkDuringSuccessfulSingleSignOnIsRedirectedToTheDeepLinkAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Dora").FamilyName("Yaki"))
                .WithBehaviour(new NhsLoginReauthorizeSSOBehaviour());

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToOrderARepeatPrescription();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver);

            driver.BackgroundApp();

            driver.OpenChromeApp()
                .NavigateToDeepLinkLauncher();

            AndroidDeepLinkLauncherPage
                .AssertOnPage(driver)
                .ClickLink();

            AndroidDeepLinkAppChoice
                .AssertDisplayed(driver)
                .ChooseNhsApp();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidAppointmentsPage.AssertOnPage(driver);
        }
    }
}