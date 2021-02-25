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
    [BusinessRule("BR-LOG-08.4", "Log in when an unexpected error occurs displays an error message")]
    public class InternalServerErrorTests
    {
         [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenNhsLoginReturnsAnErrorRedirectAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour());

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

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            AndroidNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackHome();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenNhsLoginReturnsAnErrorRedirectIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithBehaviour(new NhsLoginAuthoriseErrorCodeBehaviour());

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

            IOSNhsLoginErrorPage
                .AssertOnPage(driver)
                .AssertPageElements()
                .ContactUs();

            IOSAppTab
                .AssertOnContactUsPage(driver)
                .ReturnToApp();

            IOSNhsLoginErrorPage
                .AssertOnPage(driver)
                .BackHome();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}