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
    public class PkbYourHealthTestResultsWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirPkbTestResultsFromTheYourHealthScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Dick").FamilyName("Grayson"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestResults();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Test results")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, "/test/myTests.action")
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirPkbTestResultsFromTheYourHealthScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Victor").FamilyName("Stone"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestResults();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Test results")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, "/test/myTests.action")
                .AssertNativeHeader();
        }
    }
}