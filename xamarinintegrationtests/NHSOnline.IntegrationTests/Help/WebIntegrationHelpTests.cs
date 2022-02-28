using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.Help
{
    [TestClass]
    [BusinessRule("BR-NAV-01.4", "Clicking the help icon opens a contextual help link in a browser overlay")]
    public class WebIntegrationHelpTests
    {
        private const string ThirdPartyHelpLinkPath = "health-records-in-the-nhs-app/third-party-services/";

        [NhsAppAndroidTest]
        [NhsAppFlakyTest]
        public void APkbPatientCanAccessContextualHelpFromWebIntegrationScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = LoadPkbPatient();

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines");

            driver.PressBackButton();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions)
                .Navigation.NavigateToHelp();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, ThirdPartyHelpLinkPath)
                .ReturnToApp();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions);
        }

        [NhsAppIOSTest]
        [NhsAppFlakyTest]
        public void APkbPatientCanAccessContextualHelpFromWebIntegrationScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = LoadPkbPatient();

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines");

            driver.SwipeBack();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other medicines")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions)
                .Navigation.NavigateToHelp();

            IOSBrowserOverlayNhsAppHelpPage
                .AssertOnPage(driver, ThirdPartyHelpLinkPath)
                .ReturnToApp();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions);
        }

        private static PkbPatient LoadPkbPatient()
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Arthur").FamilyName("Curry"));

            Mocks.Patients.Add(patient);

            return patient;
        }
    }
}
