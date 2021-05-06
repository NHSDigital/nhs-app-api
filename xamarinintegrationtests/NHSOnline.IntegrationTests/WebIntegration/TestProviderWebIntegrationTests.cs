using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{

    [TestClass]
    public class TestProviderWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessTheTestProviderFromMessagesScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Messages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanKeyboardNavigateToAccessTheTestProviderFromMessagesScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .KeyboardNavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessTheTestProviderFromMessagesScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Messages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }
    }
}