using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Substrakt
{

    [TestClass]
    public class SubstraktAskGpAQuestionWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessTheirSubstraktAskYourGpSurgeryAQuestionFromTheMessagesScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new SubstraktPatient()
                .WithName(b => b.GivenName("Wally").FamilyName("West"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Messages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpSurgeryAQuestion();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP surgery a question")
                .PageContent.NavigateToNextPage();

            AndroidSubstraktPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }

        [NhsAppIOSTest]
        public void APatientCanAccessTheirSubstraktAskYourGpSurgeryAQuestionFromTheMessagesScreenIOS(IIOSDriverWrapper driver)
        {
            var patient = new SubstraktPatient()
                .WithName(b => b.GivenName("Jay").FamilyName("Garrick"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.Messages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpSurgeryAQuestion();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP surgery a question")
                .PageContent.NavigateToNextPage();

            IOSSubstraktPage
                .AssertOnPage(driver)
                .AssertNativeHeader();
        }
    }
}