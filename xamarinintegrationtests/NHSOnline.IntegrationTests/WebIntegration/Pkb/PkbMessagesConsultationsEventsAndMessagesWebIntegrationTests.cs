using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Pkb
{

    [TestClass]
    public class PkbMessagesConsultationsEventsAndMessagesWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirPkbConsultationsEventsAndMessagesFromTheMessagesScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Eobard").FamilyName("Thawne"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToConsultationsEventsAndMessages();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Messages and online consultations")
                .PageContent.NavigateToNextPage();

            AndroidPkbPage
                .AssertOnPage(driver, "/auth/getInbox.action?tab=messages")
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirPkbConsultationsEventsAndMessagesFromTheMessagesScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new PkbPatient()
                .WithName(b => b.GivenName("Hunter").FamilyName("Zolomon"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToConsultationsEventsAndMessages();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Messages and online consultations")
                .PageContent.NavigateToNextPage();

            IOSPkbPage
                .AssertOnPage(driver, "/auth/getInbox.action?tab=messages")
                .AssertNativeHeader();
        }
    }
}