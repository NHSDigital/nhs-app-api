using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.CitizenId;
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
    [BusinessRule("BR-LOG-08.6", "Log in when a timeout response is received displays and error message")]
    public class GatewayTimeoutErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenNhsLoginGetProfileTimesOutAndroid(IAndroidDriverWrapper driver)
        {
            using var delayBehaviour = new NhsLoginGetUserProfileDelayBehaviour();
            var patient = new P5Patient()
                .WithBehaviour(delayBehaviour);
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

            using (ExtendedTimeout.FromSeconds(15))
            {
                AndroidCreateSessionUpstreamSystemTimeoutErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .ContactUs();
            }

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            AndroidCreateSessionUpstreamSystemTimeoutErrorPage
                .AssertOnPage(driver)
                .BackHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenNhsLoginGetProfileTimesOutIos(IIOSDriverWrapper driver)
        {
            using var delayBehaviour = new NhsLoginGetUserProfileDelayBehaviour();
            var patient = new P5Patient()
                .WithBehaviour(delayBehaviour);
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

            using (ExtendedTimeout.FromSeconds(15))
            {
                IOSCreateSessionUpstreamSystemTimeoutErrorPage
                    .AssertOnPage(driver)
                    .AssertPageElements()
                    .ContactUs();
            }

            IOSAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            IOSCreateSessionUpstreamSystemTimeoutErrorPage
                .AssertOnPage(driver)
                .BackHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}