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
            var patient = new P9Patient();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.LoginWithLoginTermsAndConditions(patient);

            AndroidStubbedLoginTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.DeclineTermsAndConditions();

            AndroidAcceptNhsTermsOfUsePageContent
                .AssertOnPage(driver)
                .AssertPageContent();
        }

        [NhsAppIOSTest]
        public void APatientDecliningNhsLoginTermsOfUseIsInformedTheyHaveToAcceptToUseTheAppIos(
            IIOSDriverWrapper driver)
        {
            var patient = new P9Patient();
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

            IOSAcceptNhsTermsOfUsePageContent
                .AssertOnPage(driver)
                .AssertPageContent();
        }
    }
}