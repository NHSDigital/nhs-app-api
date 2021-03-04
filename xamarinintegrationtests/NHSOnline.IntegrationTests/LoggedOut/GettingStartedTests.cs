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
    [BusinessRule("BR-LOG-02.1", "Continuing with NHS login displays the getting started screen")]
    public sealed class GettingStartedTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownTheGettingStartedPageAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToGettingStartedPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheGettingStartedPageIos(IIOSDriverWrapper driver)
        {
            NavigateToGettingStartedPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientOnTheGettingStartedPageCanClickToViewTheCovidAppAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .GoToCovidApp();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidAppPage(driver)
                .ReturnToApp();

            AndroidGettingStartedPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientOnTheGettingStartedPageCanClickToViewTheCovidAppIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .GoToCovidApp();

            IOSAppTab
                .AssertOnCovidAppPage(driver)
                .ReturnToApp();

            IOSGettingStartedPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanUseTheKeyboardToActivateTheLinksOnTheGettingStartedPageAndBeTakenToTheCorrectPagesAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToGettingStartedPage(driver)
                .TabToGetCovidApp()
                .PressEnterKey();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidAppPage(driver)
                .ReturnToApp();

            AndroidGettingStartedPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanUseTheKeyboardToTabThroughTheControlsOnTheGettingStartedPageAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToGettingStartedPage(driver)
                .AssertTabFocusOrder();
        }

        private static AndroidGettingStartedPage NavigateToGettingStartedPage(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            return AndroidGettingStartedPage.AssertOnPage(driver);
        }

        private static IOSGettingStartedPage NavigateToGettingStartedPage(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            return IOSGettingStartedPage.AssertOnPage(driver);
        }
    }
}