using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-9.4", "Invoking native back on the NHS login journey displays the logged out home screen")]
    public class NhsLoginBackTests
    {
        [NhsAppAndroidTest]
        public void APatientCanUseTheBackButtonToGoBackToTheHomePageFromNhsLoginAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanUseTheBackButtonToGoBackToTheHomePageFromNhsLoginIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}