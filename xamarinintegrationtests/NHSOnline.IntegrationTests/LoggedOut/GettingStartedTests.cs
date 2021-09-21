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
    [BusinessRule("BR-LOG-02.3", "Navigating back from before you start screen displays the Logged out home screen")]
    [BusinessRule("BR-LOG-09.3", "Invoking native back on the getting started screen displays the logged out home screen")]
    public sealed class GettingStartedTests
    {
        private const string WhatToDoThirteenToFifteenLinkPath = "who-can-use-the-nhs-app/";

        [NhsAppAndroidTest]
        public void APatientIsShownTheGettingStartedPageAndCanNavigateBackAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToGettingStartedPage(driver)
                .AssertPageElements()
                .GoToWhatToDoThirteenToFifteenLink();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome());

            AndroidAppTab
                .AssertOnHelpPageByText(driver, WhatToDoThirteenToFifteenLinkPath)
                .ReturnToApp();

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheGettingStartedPageAndCanNavigateBackIos(IIOSDriverWrapper driver)
        {
            NavigateToGettingStartedPage(driver)
                .AssertPageElements()
                .GoToWhatToDoThirteenToFifteenLink();

            IOSAppTab
                .AssertOnHelpPageByText(driver, WhatToDoThirteenToFifteenLinkPath)
                .ReturnToApp();

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
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