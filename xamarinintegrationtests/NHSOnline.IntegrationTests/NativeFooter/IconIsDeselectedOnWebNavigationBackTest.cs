using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.YourHealth;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.3", "Navigating back via the web deselects the icon")]
    public class IconIsDeselectedOnWebNavigationBackTest
    {
        [NhsAppAndroidTest]
        public void APatientNavigatingBackToHomeUsingABreadcrumbSeesNoBottomNavIconHighlightedAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.GpHealthMenuItem.Click();

            AndroidGpMedicalRecordPage
                .AssertOnPage(driver)
                .Navigation.AssertYourHealthSelected();

            AndroidGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent.BackBreadcrumb.Click();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AssertNoIconsSelected();
        }

        [NhsAppIOSTest]
        public void APatientNavigatingBackToHomeUsingABreadcrumbSeesNoBottomNavIconHighlightedIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.GpHealthMenuItem.Click();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .Navigation.AssertYourHealthSelected();

            IOSGpMedicalRecordPage
                .AssertOnPage(driver)
                .PageContent.BackBreadcrumb.Click();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AssertNoIconsSelected();
        }
    }
}