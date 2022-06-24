using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Advice
{
    [TestClass]
    public class AccurxGpAdviceTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessAccurxGpAdviceJourneyAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Accurx)
                .WithName(b => b.GivenName("Electra").FamilyName("Consult"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpForAdviceAccurx();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP for medical advice");
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAccurxGpAdviceJourneyIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.Accurx)
                .WithName(b => b.GivenName("Electra").FamilyName("Consult"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpForAdviceAccurx();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP for medical advice");
        }
    }
}