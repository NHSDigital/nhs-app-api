using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    public class LoginTests
    {
        [NhsAppAndroidTest]
        [Ignore("Logged in home page is less friendly")]
        public void APatientWithProofLevelFiveCanLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
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
                .PageContent.Continue();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Fred Jones");
        }

        [NhsAppIOSTest]
        [Ignore("Logged in home page is less friendly")]
        public void APatientWithProofLevelFiveCanLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Williams"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
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
                .AssertPageDisplayedFor("Fred Williams");
        }

        [NhsAppAndroidTest]
        [Ignore("Logged in home page is less friendly")]
        public void APatientWithProofLevelNineCanLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.Title("Mr").GivenName("Jack").FamilyName("Flash"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            AndroidUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Jack Flash");
        }

        [NhsAppIOSTest]
        [Ignore("Logged in home page is less friendly")]
        public void APatientWithProofLevelNineCanLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.Title("Mr").GivenName("Jack").FamilyName("Flash"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
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

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Jack Flash");
        }

        [NhsAppAndroidTest]
        [Ignore("Link appears to be focused but isn't clicked")]
        public void APatientCanFollowInternalAndExternalLinksFromNhsLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.InternalPage();

            AndroidStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.Back();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.InternalPageNewWindow();

            AndroidStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.Back();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Covid();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            AndroidStubbedLoginPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanFollowInternalAndExternalLinksFromNhsLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.InternalPage();

            IOSStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.Back();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Covid();
            
            IOSAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            IOSStubbedLoginPage
                .AssertOnPage(driver);
        }

        [Ignore("Clicking links with target='_blank' does not work in BrowserStack")]
        [NhsAppIOSTest]
        public void APatientCanFollowInternalLinkWithTargetBlankFromNhsLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.InternalPageNewWindow();

            IOSStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.Back();

            IOSStubbedLoginPage
                .AssertOnPage(driver);
        }
    }
}
