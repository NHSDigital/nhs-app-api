using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    public static class LoginProcess
    {
        // These methods are not to be used when testing the login flow
        public static void LogAndroidPatientIn(IAndroidDriverWrapper driver, Patient patient)
        {
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
        }

        public static void LogIOSPatientIn(IIOSDriverWrapper driver, Patient patient)
        {
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
                .OptInToUserResearch();
        }

        public static void LogIOSPatientInPreIos13(IIOSDriverWrapper driver, Patient patient)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSPreIOS13GettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            var termsAndConditionsPage = IOSTermsAndConditionsPage
                .AssertOnPage(driver);

            termsAndConditionsPage.PageContent.AcceptTermsAndConditionsWithoutContinue();

            TransitoryErrorHandler.HandleSpecificFailure()
                .Alternate(() =>
                    {
                        // The accept action is flakey and doesnt always check the box
                        termsAndConditionsPage.PageContent.VerifyAccepted();
                    },
                    "Expected e.Selected to be true because Checkbox should be selected, but found False.",
                    () => { termsAndConditionsPage.PageContent.AcceptTermsAndConditions(); });

            termsAndConditionsPage.PageContent.Continue();

            TransitoryErrorHandler.HandleSpecificFailure()
                .Alternate(() =>
                    {
                        // The accept action is flakey and doesnt always check the box
                        IOSUserResearchOptInPage
                            .AssertOnPage(driver);
                    },
                    "No IWebElement found matching By.XPath: //h1[normalize-space()='Help improve the NHS App']",
                    () =>
                    {
                        termsAndConditionsPage.AssertPageContent();
                        termsAndConditionsPage.PageContent.AcceptTermsAndConditionsWithoutContinue();
                        termsAndConditionsPage.PageContent.VerifyAccepted();
                        termsAndConditionsPage.PageContent.Continue();
                    });

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();
        }
    }
}
