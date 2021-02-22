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
    public sealed class BeforeYouStartTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownTheBeforeYouStartPageAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void APatientIsShownTheBeforeYouStartPageIos(IIOSDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanClickTheLinksAndIsTakenToTheCorrectPagesAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .CheckCoronavirusSymptoms();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .SearchConditionsAndTreatments();

            AndroidAppTab
                .AssertOnConditionsPage(driver)
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .UseNhs111Online();

            AndroidAppTab
                .AssertOn111Page(driver)
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanClickAllTheLinksAndGoToCorrectPagesIos(IIOSDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .SearchConditionsAndTreatments();

            IOSAppTab
                .AssertOnConditionsPage(driver)
                .ReturnToApp();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .CheckCoronavirusSymptoms();

            IOSAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .UseNhs111Online();

            IOSAppTab
                .AssertOn111Page(driver)
                .ReturnToApp();

            IOSBeforeYouStartPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanUseTheKeyboardToActivateTheLinksAndBeTakenToTheCorrectPagesAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .TabToCheckCoronavirusSymptoms()
                .PressEnterKey();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidPage(driver)
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .TabToSearchConditionsAndTreatments()
                .PressEnterKey();

            AndroidAppTab
                .AssertOnConditionsPage(driver)
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .TabToUseNhs111Online()
                .PressEnterKey();

            AndroidAppTab
                .AssertOn111Page(driver)
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanViewTheGuidanceForAges13To15Android(IAndroidDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertCanShowAndHideGuidanceForAges13To15();
        }

        [NhsAppIOSTest]
        public void APatientCanViewTheGuidanceForAges13To15Ios(IIOSDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertCanShowAndHideGuidanceForAges13To15();
        }

        [NhsAppAndroidTest]
        public void APatientCanUseTheKeyboardToTabThroughTheControlsOnTheBeforeYouStartScreenPageAndroid(IAndroidDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertTabFocusOrder();
        }

        private static AndroidBeforeYouStartPage NavigateToBeforeYouStartPage(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            return AndroidBeforeYouStartPage.AssertOnPage(driver);
        }

        private static IOSBeforeYouStartPage NavigateToBeforeYouStartPage(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            return IOSBeforeYouStartPage.AssertOnPage(driver);
        }
    }
}