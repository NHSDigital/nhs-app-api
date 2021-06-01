using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.2", "Navigating to a service via the web the correct nav icon is highlighted")]
    public class CorrectIconIsSelectedOnWebNavigationTest
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateToRepeatPrescriptionsUsingThePopularServicesAndSeeThePrescriptionsBottomNavIconHighlightedAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.PrescriptionsMenuItem.Click();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AssertPrescriptionsSelected();
        }

        [NhsAppIOSTest]
        public void APatientCanNavigateToRepeatPrescriptionsUsingThePopularServicesAndSeeThePrescriptionsBottomNavIconHighlightedIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.PrescriptionsMenuItem.Click();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.AssertPrescriptionsSelected();
        }
    }
}