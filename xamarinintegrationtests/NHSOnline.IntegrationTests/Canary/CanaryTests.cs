using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.Canary
{
    [TestClass]
    public class CanaryTests
    {
        [NhsAppAndroidTest]
        [NhsAppCanaryTest]
        public void APatientWithProofLevelFiveCanUseTheMainAreasOfTheAppAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Eobard").FamilyName("Thawne"));
            using var patients = Mocks.Patients.Add(patient);

            driver.CloseApp();

            driver.OpenChromeApp()
                .NavigateToDeepLinkLauncher();

            AndroidDeepLinkLauncherPage
                .AssertOnPage(driver)
                .ClickLink();

            AndroidDeepLinkAppChoice
                .AssertDisplayed(driver)
                .ChooseNhsApp();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .Navigation.NavigateToYourHealth();

            AndroidYourHealthPkbPage
                .AssertOnPage(driver)
                .PageContent.NavigateToRecordSharing();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.RecordSharing)
                .AssertNativeHeader()
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.Logout();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        [NhsAppCanaryTest]
        public void APatientWithProofLevelFiveCanUseTheMainAreasOfTheAppIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Eobard").FamilyName("Thawne"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            IOSHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .AssertNativeHeader()
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .PageContent.Logout();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}
