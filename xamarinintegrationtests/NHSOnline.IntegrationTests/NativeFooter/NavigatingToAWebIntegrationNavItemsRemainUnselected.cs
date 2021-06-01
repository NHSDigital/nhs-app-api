using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.9", "Accessing a web integration from a a service that does not have a highlighted nav icon navigates to the web integration and no nav icon is highlighted")]
    public class NavigatingToAWebIntegrationServiceTheIconStaysHighlightedTest
    {
        [NhsAppAndroidTest]
        public void APatientSeesTheNavigationFooterRemainDeselectedWhenNavigatingToAWebIntegrationAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Charlie").FamilyName("Dimmock"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            AndroidMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            AndroidNhsLoginSettingsPage
                .AssertOnPage(driver)
                .Navigation.AssertNoIconsSelected();
        }

        [NhsAppIOSTest]
        public void APatientSeesTheNavigationFooterRemainDeselectedWhenNavigatingToAWebIntegrationIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Alan").FamilyName("Titchmarsh"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver)
                .PageContent.NavigateToNhsLogin();

            IOSNhsLoginSettingsPage
                .AssertOnPage(driver)
                .Navigation.AssertNoIconsSelected();
        }
    }
}