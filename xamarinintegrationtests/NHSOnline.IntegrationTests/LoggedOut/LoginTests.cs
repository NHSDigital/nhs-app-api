using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
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
            var patient = new P5Patient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"));
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

            IOSGettingStartedPage
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

            IOSGettingStartedPage
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
