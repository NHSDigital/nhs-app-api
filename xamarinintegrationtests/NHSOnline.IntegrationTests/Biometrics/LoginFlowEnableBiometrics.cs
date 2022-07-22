using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Home.Biometrics;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings.Biometrics;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Biometrics
{
    [TestClass]
    public class LoginFlowEnableBiometrics
    {
        [NhsAppAndroidTest]
        public void APatientCanEnableBiometricsDuringTheLoginFlowAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Jack").FamilyName("Potts"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .NoDontSendNotifications()
                .Continue();

            AndroidFingerprintFaceIrisPromptPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone11Pro, OSVersion = IOSVersion.Thirteen)]
        public void APatientCanEnableFaceBiometricsDuringTheLoginFlowIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .NoDontSendNotifications()
                .Continue();

            IOSFacePromptPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhone8, OSVersion = IOSVersion.Thirteen)]
        public void APatientCanEnableTouchBiometricsDuringTheLoginFlowIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.NotificationsPromptEnabled)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSManageNotificationsPromptPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .NoDontSendNotifications()
                .Continue();

            IOSTouchPromptPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}