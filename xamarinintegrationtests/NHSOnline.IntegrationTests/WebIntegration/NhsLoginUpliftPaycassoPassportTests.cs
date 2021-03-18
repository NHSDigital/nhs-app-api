using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-LOG-11.3", "Invoking Paycasso in the NHS login uplift journey to capture an image of a passport displays Paycasso")]
    public class NhsLoginUpliftPaycassoPassportTests
    {
        [NhsAppIOSTest]
        [Ignore("NHSO-13762: Ignoring test until we decide whether Paycasso should stay or go")]
        public void APatientWithProofLevelFiveCanStartPaycassoToTakeAPhotoOfTheirPassportIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient();
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
                .PageContent.Continue();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver)
                .PageContent.Paycasso();

            IOSStubbedLoginPaycassoPage
                .AssertOnPage(driver)
                .PageContent.UseStubs().Start();

            IOSPermissionsDialog
                .AssertDisplayedForCamera(driver)
                .Okay();

            IOSPaycassoPassportPage
                .AssertOnPage(driver)
                .AssertPageContent();
        }
    }
}