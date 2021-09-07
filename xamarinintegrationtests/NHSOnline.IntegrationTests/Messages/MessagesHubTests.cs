using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Messages
{
    [TestClass]
    [BusinessRule("BR-MES-01.1", "Show 'Health information and updates' panel on messages hub page")]
    [BusinessRule("BR-MES-01.2", "Show 'GP surgery messages' panel on messages hub page")]
    [BusinessRule("BR-MES-01.3", "Show 'Ask your GP surgery a question' panel on messages hub page")]
    [BusinessRule("BR-MES-01.4", "Show 'Messages and consultations with a doctor or health professional' panel on messages hub page")]
    [BusinessRule("BR-MES-01.5", "Show 'Consultations, events and messages' panel on messages hub page")]
    [BusinessRule("BR-MES-01.6", "Show 'See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form' panel on messages hub page")]
    [BusinessRule("BR-MES-01.7", "Show 'Online consultations' panel on messages hub page")]
    public class MessagesHubTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownTheMessagesHubPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Garry").FamilyName("Messi"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertEngageElements()
                .AssertSubstraktElements()
                .AssertPkbElements()
                .AssertCieElements()
                .AssertMyCareViewElements()
                .AssertSecondaryCareViewElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheMessagesHubPageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Garry").FamilyName("Messi"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertEngageElements()
                .AssertSubstraktElements()
                .AssertPkbElements()
                .AssertCieElements()
                .AssertMyCareViewElements()
                .AssertSecondaryCareViewElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanKeyboardNavigateToTheMessagesHubPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Garry").FamilyName("Messi"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToGpSurgeryMessages();

            AndroidGpSurgeryMessagesPage
                .AssertOnPage(driver)
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToGncr();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Online consultations")
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToSubstrakt();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP surgery a question")
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToPkb();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Messages and online consultations")
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToCie();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Consultations, events and messages")
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToMyCareView();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Consultations, events and messages")
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToSecondaryCareView();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Consultations, events and messages")
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .TabIntoFocus()
                .KeyboardNavigateToHealthInfoAndUpdates();

            AndroidHealthInformationAndUpdatesPage
                .AssertOnPage(driver)
                .KeyboardNavigateBack();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageElements()
                .AssertEngageElements()
                .AssertSubstraktElements()
                .AssertPkbElements()
                .AssertCieElements()
                .AssertMyCareViewElements()
                .AssertSecondaryCareViewElements();
        }
    }
}