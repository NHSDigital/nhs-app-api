using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-WI-01.14", "Following a link from within the web integration to a valid NHS App location navigates the user to the specified location")]
    [BusinessRule("BR-WI-01.17", "Navigating to Covid Pass service displays the Covid Pass journey")]
    public class CovidPassP5Tests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanSuccessfullyAccessTheirCovidPassAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.GetYourCovidPass();

            AndroidStubbedNetCompanyInternalPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .Uplift();

            AndroidUpliftShutterPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanSuccessfullyAccessTheirCovidPassIos(IIOSDriverWrapper driver)
        {
            var patient = new TppPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent
                .GetYourCovidPass();

            IOSStubbedNetCompanyInternalPage
                .AssertOnPage(driver)
                .PageContent
                .AssertPageContent()
                .Uplift();

            IOSUpliftShutterPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver);
        }
    }
}