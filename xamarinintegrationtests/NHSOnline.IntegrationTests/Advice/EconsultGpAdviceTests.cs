using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Advice
{
    [TestClass]
    [BusinessRule("BR-GI-01.1", "Navigating to GP Advice journey displays the eConsult GP Advice journey")]
    [BusinessRule("BR-GI-01.2", "Sharing demographic data in an eConsult journey pre-populates relevant fields for the user")]
    [BusinessRule("BR-GI-01.3", "Clicking the link to find out more information about online consultations navigates the user to the Online Consultation Privacy Policy on NHS.uk in a browser overlay")]
    [BusinessRule("BR-GI-01.4", "Clicking the link for the NHS App privacy policy navigates the user to the Privacy Policy on NHS.uk in a browser overlay")]
    public class EconsultGpAdviceTests
    {
        [NhsAppAndroidTest]
        public void APatientCanAccessEngageGpAdviceJourneyAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Electra").FamilyName("Consult"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpForAdviceEngage();

            AndroidAskYourGpForAdviceStartPage
                .AssertOnPage(driver)
                .PageContent.SelectFindOutMore();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayOnlineConsultationPrivacyPolicyPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidAskYourGpForAdviceStartPage
                .AssertOnPage(driver)
                .PageContent.SelectPrivacyPolicy();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayNhsAppPrivacyPolicyPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidAskYourGpForAdviceStartPage
                .AssertOnPage(driver)
                .PageContent.AssertPageContent()
                .ClickDemographicsCheckbox()
                .Continue();

            AndroidAskYourGpForAdviceTermsPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidAskYourGpForAdviceTermsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanAccessEngageGpAdviceJourneyIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Electra").FamilyName("Consult"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAdvice();

            IOSAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToAskYourGpForAdviceEngage();

            IOSAskYourGpForAdviceStartPage
                .AssertOnPage(driver)
                .PageContent.SelectFindOutMore();

            IOSBrowserOverlayOnlineConsultationPrivacyPolicyPage
                .AssertOnPage(driver)
                .ReturnToApp();

            IOSAskYourGpForAdviceStartPage
                .AssertOnPage(driver)
                .PageContent.AssertPageContent()
                .ClickDemographicsCheckbox()
                .Continue();

            IOSAskYourGpForAdviceTermsPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSAskYourGpForAdviceTermsPage
                .AssertOnPage(driver);
        }

        [NhsAppManualTest("NHSO-15774", "OLC Journey is not fully stubbed yet")]
        public void APatientThatAllowsDemographicDataHasTheirNameAutoPopulatedOnTheGpAdviceJourney() { }
    }
}