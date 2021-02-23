using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-9.3", "Invoking native back on the before you start screen displays the logged out home screen")]
    public class BeforeYouStartBackTests
    {
        [NhsAppAndroidTest]
        public void APatientNavigatingBackFromBeforeYouStartIsShownTheLoggedOutHomePageAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientNavigatingBackFromBeforeYouStartIsShownTheLoggedOutHomePageIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}