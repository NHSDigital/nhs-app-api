using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.Prescriptions;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.WebIntegration.Pkb;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.8", "Accessing a web integration from within a a service with a highlighted nav icon navigates to the web integration and the nav icon remains highlighted")]
    public class NavigatingToAWebIntegrationNavItemsRemainSelectedTests
    {
        [NhsAppAndroidTest]
        public void APatientSeesTheNavigationFooterRemainSelectedWhenNavigatingToAWebIntegrationAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Alan").FamilyName("Titchmarsh"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToOneOneOne();

            AndroidOneOneOnePage
                .AssertOnPage(driver)
                .Navigation.AssertAdviceSelected();

            AndroidOneOneOnePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherAppointments();

            AndroidHospitalAndOtherAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToViewAppointments();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "View appointments")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .Navigation.AssertAppointmentsSelected();

            AndroidPkbPage
                .AssertOnPage(driver, PhrPath.ViewAppointments)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            AndroidTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .Navigation.AssertMessagesSelected();
        }

        [NhsAppIOSTest]
        public void APatientSeesTheNavigationFooterRemainSelectedWhenNavigatingToAWebIntegrationIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Tommy").FamilyName("Walsh"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToOneOneOne();

            IOSOneOneOnePage
                .AssertOnPage(driver)
                .Navigation.AssertAdviceSelected();

            IOSOneOneOnePage
                .AssertOnPage(driver)
                .Navigation.NavigateToPrescriptions();

            IOSPrescriptionsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToHospitalAndOtherPrescriptions();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Hospital and other prescriptions")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions)
                .Navigation.AssertPrescriptionsSelected();

            IOSPkbPage
                .AssertOnPage(driver, PhrPath.HospitalAndOtherPrescriptions)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToTestProvider();

            IOSTestWebIntegrationProviderPage
                .AssertOnPage(driver)
                .Navigation.AssertMessagesSelected();
        }
    }
}