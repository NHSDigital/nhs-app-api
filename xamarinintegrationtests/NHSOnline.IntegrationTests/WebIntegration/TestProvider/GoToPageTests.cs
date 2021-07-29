using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.Prescriptions;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.YourHealth;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.TestProvider
{
    [TestClass]
    public class GoToPageTests
    {
        [NhsAppAndroidTest]
        public void APatientCanNavigateOnThirdPartiesThatUseTheGoToPageMethodAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AssertOnHomePageAndNavigateToGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToHomePage();

            AssertOnHomePageAndNavigateToGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHome();

            AssertOnHomePageAndNavigateToGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToPrescriptions();

            AndroidPrescriptionsPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHome();

            AssertOnHomePageAndNavigateToGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.NavigateToHome();

            AssertOnHomePageAndNavigateToGoToPageAndroid(driver);

            AndroidGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .TabIntoFocus()
                .KeyboardNavigateToGoToInvalidPage();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanNavigateOnThirdPartiesThatUseTheGoToPageMethodIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AssertOnHomeMessagesPageAndNavigateToGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToYourHealthPage();

            IOSYourHealthPage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AssertOnHomeMessagesPageAndNavigateToGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToYourAdvicePage();

            IOSAdvicePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AssertOnHomeMessagesPageAndNavigateToGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToMorePage();

            IOSMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AssertOnHomeMessagesPageAndNavigateToGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToSettingsPage();

            IOSMorePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AssertOnHomeMessagesPageAndNavigateToGoToPageIOS(driver);

            IOSGoToPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.GoToUpliftPage();

            IOSUpliftShutterPage
                .Continue(driver);

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }

        private static void AssertOnHomePageAndNavigateToGoToPageAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();
        }

        private static void AssertOnHomeMessagesPageAndNavigateToGoToPageIOS(IIOSDriverWrapper driver)
        {
            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .AssertNativeHeader()
                .PageContent.NavigateToGoToPage();
        }
    }
}