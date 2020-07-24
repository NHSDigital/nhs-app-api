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
                .SearchConditionsAndTreatments();

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .ChooseChrome()
                .JustOnce()
                .AssertOnConditionsPage()
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .CheckCoronavirusSymptoms();

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .JustOnce()
                .AssertOnCovidPage()
                .ReturnToApp();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .UseNhs111Online();

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .JustOnce()
                .AssertOn111Page()
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
        public void APatientCanViewTheGuidanceForAges13To15Android(IAndroidDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertCanShowAndHideGuidanceForAges13To15();
        }

        [NhsAppIOSTest]
        public void APatientCanUseTheExpanderIos(IIOSDriverWrapper driver)
        {
            NavigateToBeforeYouStartPage(driver)
                .AssertExpanderPresent();
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