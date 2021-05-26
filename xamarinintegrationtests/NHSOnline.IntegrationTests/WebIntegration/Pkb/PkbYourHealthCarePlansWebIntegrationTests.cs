using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Pkb
{

    [TestClass]
    public class PkbYourHealthCarePlansWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirPkbCarePlansFromTheYourHealthScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Floyd").FamilyName("Lawton"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToCarePlans();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Care plans")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, "/auth/listPlans.action")
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirPkbCarePlansFromTheYourHealthScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Slade").FamilyName("Wilson"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealth();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToCarePlans();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Care plans")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, "/auth/listPlans.action")
                .AssertNativeHeader();
        }
    }
}