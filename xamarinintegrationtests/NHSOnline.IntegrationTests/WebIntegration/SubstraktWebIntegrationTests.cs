using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{

    [TestClass]
    public class SubstraktWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessSubstraktAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new SubstraktPatient()
                .WithName(b => b.GivenName("Stephen").FamilyName("Strange"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            AndroidYourHealthSubstraktPage
                .AssertOnPage(driver)
                .PageContent.NavigateToUpdateYourPersonalDetails();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Update your personal details")
                .PageContent.NavigateToNextPage();

            AndroidSubstraktPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToExternalDomain();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertInBrowserAppTab(driver)
                .ReturnToApp();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessSubstraktIOS(IIOSDriverWrapper driver)
        {
            var patient = new SubstraktPatient()
                .WithName(b => b.GivenName("Stephen").FamilyName("Strange"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToYourHealth();

            IOSYourHealthSubstraktPage
                .AssertOnPage(driver)
                .PageContent.NavigateToUpdateYourPersonalDetails();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Update your personal details")
                .PageContent.NavigateToNextPage();

            IOSSubstraktPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToExternalDomain();

            IOSAppTab
                .AssertInBrowserAppTab(driver)
                .ReturnToApp();
        }
    }
}