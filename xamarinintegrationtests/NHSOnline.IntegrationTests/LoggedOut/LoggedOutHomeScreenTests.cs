using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-1.1", "Launching the app displays the logged out home screen")]
    public sealed class LoggedOutHomeScreenTests
    {
        [NhsAppAndroidTest]
        public void APatientSeesTheLoggedOutHomeScreenWhenStartingTheAppAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanUseTheBackButtonToCloseTheAppOnTheLoggedOutHomeScreenAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver);

            driver.AssertRunningInForeground();

            driver.PressBackButton();

            driver.AssertNotRunningInForeground();
        }

        [NhsAppIOSTest]
        public void APatientCanNotSwipeToCloseTheAppOnTheLoggedOutHomeScreenIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientSeesTheLoggedOutHomeScreenWhenStartingTheAppIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}