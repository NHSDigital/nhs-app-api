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