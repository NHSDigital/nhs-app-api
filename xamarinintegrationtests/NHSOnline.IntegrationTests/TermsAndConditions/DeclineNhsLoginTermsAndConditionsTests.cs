using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.TermsAndConditions
{
    [TestClass]
    [BusinessRule("BR-LOG-08.8", "Log in when the user has declined the NHS login terms of use displays a message")]
    public class DeclineNhsLoginTermsAndConditionsTests
    {
        [NhsAppAndroidTest]
        public void APatientDecliningNhsLoginTermsOfUseIsInformedTheyHaveToAcceptToUseTheAppAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            var androidStubbedLoginPageSlimHeader = AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver);

            androidStubbedLoginPageSlimHeader
                .PageContent.LoginWithLoginTermsAndConditions(patient);

            TransitoryErrorHandler.HandleSpecificFailure()
                .ResetAndRetry(() =>
                    {
                        AndroidStubbedLoginTermsAndConditionsPage
                            .AssertOnPage(driver)
                            .PageContent.DeclineTermsAndConditions();

                        AndroidNeedToAcceptNhsTermsOfUsePage
                            .AssertOnPage(driver)
                            .AssertPageContent()
                            .BackToLogin();

                        AndroidLoggedOutHomePage
                            .AssertOnPage(driver);
                    },
                    "No IWebElement found matching By.XPath: //h1[normalize-space()='NHS Login - Terms and Conditions']",
                    // Intermittent issue where the on-screen keyboard appears. Clicking 'Login' again will continue test.
                    () => { androidStubbedLoginPageSlimHeader.PageContent.LoginButtonClick(); });
        }

        [NhsAppIOSTest]
        public void APatientDecliningNhsLoginTermsOfUseIsInformedTheyHaveToAcceptToUseTheAppIos(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.LoginWithLoginTermsAndConditions(patient);

            IOSStubbedLoginTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.DeclineTermsAndConditions();

            IOSNeedToAcceptNhsTermsOfUsePage
                .AssertOnPage(driver)
                .AssertPageContent()
                .BackToLogin();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}