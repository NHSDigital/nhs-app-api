using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth.Ndop;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth.Ndop;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Ndop
{
    [TestClass]
    [BusinessRule("BR-NS-05.2", "Navigating to the NDOP journey from the jump off in the NHS App (mobile App) initiates the NDOP journey within the NHS App.")]
    [BusinessRule("BR-NS-05.3", "When entering NDOP journey several jump offs (information screens) are available to the User as links")]
    [BusinessRule("BR-NS-05.4", "User can navigate between available  information screens either by choosing dedicated named links or by choosing navigation elements (Next and Previous)")]
    [BusinessRule("BR-NS-05.5", "NDOP journey screen presenting detailed information about making choice for data sharing enables Start now button")]
    [BusinessRule("BR-NS-05.6", "Start now button launches NDOP Service in NHS App, when clicked")]
    [BusinessRule("BR-NS-05.7", "While launching NDOP Service from NHS App Single Sign On (SSO) applies for the User so there is no need to additionally login to NDOP Service")]
    [BusinessRule("BR-NS-05.8", "After entering NDOP Service NDOP journey takes place for the User")]
    public class NdopIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateToNdopAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Pkb)
                .WithName(b => b.GivenName("Julian").FamilyName("Assange"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToYourHealth(patient);

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNdop();

            AndroidNdopOverviewPage
                .AssertOnPage(driver)
                .PageContent.ClickMakeYourChoiceLink();

            AndroidNdopMakeYourChoicePage
                .AssertOnPage(driver)
                .PageContent.AssertElements()
                .ClickStartButton();

            AndroidNdopWebIntegrationPage
                .AssertOnPage(driver)
                .PageContent.AssertNhsNumberDisplayedFor(patient.NhsNumber.StringValue);
        }

        [NhsAppIOSTest]
        public void APatientCanNavigateToNdopIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Julian").FamilyName("Assange"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToNdop();

            IOSNdopPage
                .AssertOnPage(driver)
                .PageContent.ClickMakeYourChoiceLink();

            IOSNdopMakeYourChoicePage
                .AssertOnPage(driver)
                .PageContent.AssertElements()
                .ClickStartButton();

            IOSNdopWebIntegrationPage
                .AssertOnPage(driver)
                .PageContent.AssertNhsNumberDisplayedFor(patient.NhsNumber.StringValue);
        }

        [NhsAppManualTest("NHSO-16574",
            "We can mock losing the connection in the mocks, however the browserstack proxy masks this in the tests")]
        public void APatientSeesAnErrorWhenNdopIsUnavailableAndroid()
        {
        }

        [NhsAppManualTest("NHSO-16571",
            "We can mock losing the connection in the mocks, however the browserstack proxy masks this in the tests")]
        public void APatientSeesAnErrorWhenNdopIsUnavailableIOS()
        {
        }
    }
}