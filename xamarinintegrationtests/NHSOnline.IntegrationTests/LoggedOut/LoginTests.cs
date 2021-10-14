using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.BrowserOverlay;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    public class LoginTests
    {
        [NhsAppAndroidTest]
        public void APatientCanFollowInternalAndExternalLinksFromNhsLoginAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.InternalPage();

            AndroidStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.NavigateBack();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.InternalPageNewWindow();

            AndroidStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.NavigateBack();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Covid();

            AndroidBrowserOverlayBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidBrowserOverlayCovidPage
                .AssertOnPage(driver)
                .ReturnToApp();

            AndroidStubbedLoginPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanFollowInternalAndExternalLinksFromNhsLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"))
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
                .PageContent.InternalPage();

            IOSStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.NavigateBack();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Covid();

            IOSBrowserOverlayCovidPage
                .AssertOnPage(driver)
                .ReturnToApp();

            IOSStubbedLoginPage
                .AssertOnPage(driver);
        }

        [Ignore("Clicking links with target='_blank' does not work in BrowserStack")]
        [NhsAppIOSTest]
        public void APatientCanFollowInternalLinkWithTargetBlankFromNhsLoginIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"))
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
                .PageContent.InternalPageNewWindow();

            IOSStubbedLoginInternalPage
                .AssertOnPage(driver)
                .PageContent.NavigateBack();

            IOSStubbedLoginPage
                .AssertOnPage(driver);
        }
    }
}
