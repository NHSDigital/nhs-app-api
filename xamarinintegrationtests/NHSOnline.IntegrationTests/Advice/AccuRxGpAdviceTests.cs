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
    public class AccuRxGpAdviceTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessAccuRxGpAdviceJourneyAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AccuRx)
                .WithName(b => b.GivenName("Electra").FamilyName("Consult"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpForAdviceAccuRx();

            AndroidWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP for medical advice");
        }

        [NhsAppIOSTest]
        public void APatientCanAccessAccuRxGpAdviceJourneyIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AccuRx)
                .WithName(b => b.GivenName("Electra").FamilyName("Consult"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpForAdviceAccuRx();

            IOSWebIntegrationWarningPanelPage
                .AssertOnPage(driver, "Ask your GP for medical advice");
        }
    }
}