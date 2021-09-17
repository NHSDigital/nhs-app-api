using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Uplift
{
    [TestClass]
    [BusinessRule("BR-LOG-12.13", "Finalising successful P9 uplift via POL, when uplift was entered from logged in home screen, takes the uplifted User to logged in home screen again with all P9 User options available")]
    [BusinessRule("BR-LOG-12.15", "Returning from failed POL P9 uplift, when uplift was entered from logged in home screen, takes the P5 User to logged in home screen again with still only P5 User options available")]
    public class NhsLoginPolUpliftToP9Tests
    {
        [NhsAppAndroidTest]
        public void PolUpliftOfP5PatientIsSuccessfulReturnsToHomeScreenWithP9OptionsAvailableAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Ron").FamilyName("ManagerSuccessAndroid"))
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .UpliftSuccess();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Ron ManagerSuccessAndroid")
                .PageContent.AssertUpliftPanelNotVisible();
        }

        [NhsAppIOSTest]
        public void PolUpliftOfP5PatientIsSuccessfulReturnsToHomeScreenWithP9OptionsAvailableIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Ron").FamilyName("ManagerSuccessIos"))
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .UpliftSuccess();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Ron ManagerSuccessIos")
                .PageContent.AssertUpliftPanelNotVisible();
        }

        [NhsAppAndroidTest]
        public void PolUpliftOfP5PatientWhereNhsLoginTriggersTermsAndConditionsDeclinedReturnsToTermsAndConditionsScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Ron").FamilyName("ManagerTsAndCsAndroid"))
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .UpliftTsAndCsDeclined();

            AndroidAcceptNhsTermsOfUsePage
                .AssertOnPage(driver)
                .AssertPageContent()
                .BackToHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void PolUpliftOfP5PatientWhereNhsLoginTriggersTermsAndConditionsDeclinedReturnsToTermsAndConditionsScreenIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Ron").FamilyName("ManagerTsAndCsIos"))
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .UpliftTsAndCsDeclined();

            IOSAcceptNhsTermsOfUsePage
                .AssertOnPage(driver)
                .AssertPageContent()
                .BackToHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void PolUpliftOfP5PatientWhereNhsLoginEncountersErrorReturnsToLoginErrorScreenAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Ron").FamilyName("ManagerErrorAndroid"))
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .UpliftError();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void PolUpliftOfP5PatientWhereNhsLoginEncountersErrorReturnsToLoginErrorScreenIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Ron").FamilyName("ManagerErrorIos"))
                .WithProofLevel5();

            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .UpliftError();

            IOSNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackToHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}