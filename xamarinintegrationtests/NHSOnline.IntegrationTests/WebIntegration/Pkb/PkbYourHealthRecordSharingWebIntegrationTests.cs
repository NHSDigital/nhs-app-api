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
    public class PkbYourHealthRecordSharingWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirPkbRecordSharingFromTheYourHealthScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Alfred").FamilyName("Pennyworth"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            AndroidYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToRecordSharing();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Record Sharing")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.RecordSharing)
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirPkbRecordSharingFromTheYourHealthScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Jim").FamilyName("Gordon"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.YourHealthIcon.Click();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .PageContent.NavigateToRecordSharing();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Record Sharing")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.RecordSharing)
                .AssertNativeHeader();
        }
    }
}