using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    public class LocationServicesWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanNotAccessLocationServicesWithinTheAppWhenTheyDoNotAcceptPermissionsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToLocationServices();

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .ShowLocation();

            AndroidLocationServicesPermissionsDialog
                .AssertDisplayed(driver)
                .Deny();

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertErrorTextPresented();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessLocationServicesWithinTheAppWhenTheyAcceptPermissionsIos(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToLocationServices();

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.ShowLocation();

            IOSLocationServicesPermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSLocationServicesSitePermissionDialog
                .AssertDisplayed(driver)
                .Allow();

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertLocationPresented();
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanNotAccessLocationServicesWithinTheAppWhenTheyDoNotAcceptPermissionsIos(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToLocationServices();

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.ShowLocation();

            IOSLocationServicesPermissionDialog
                .AssertDisplayed(driver)
                .Deny();

            IOSLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertErrorTextPresented();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessLocationServicesWithinTheAppWhenTheyAcceptPermissionsAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToLocationServices();

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .ShowLocation();

            AndroidLocationServicesPermissionsDialog
                .AssertDisplayed(driver)
                .Allow();

            AndroidLocationServicesPage
                .AssertOnPage(driver)
                .PageContent.AssertLocationPresented();
        }
    }
}