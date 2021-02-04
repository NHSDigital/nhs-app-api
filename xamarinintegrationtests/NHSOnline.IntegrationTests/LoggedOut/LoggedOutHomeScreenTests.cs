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
    public sealed class LoggedOutHomeScreenTests
    {
        [NhsAppAndroidTest]
        public void APatientSeesTheLoggedOutHomeScreenWhenStartingTheAppAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        [Ignore("Covid banner missing from iOS page source")]
        public void APatientSeesTheLoggedOutHomeScreenWhenStartingTheAppIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppAndroidTest]
        public void APatientCanClickToViewCovidConditionsAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .GetInformationAboutCoronavirus();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnCovidConditionsPage(driver);
        }

        [NhsAppIOSTest]
        [Ignore("Covid banner missing from iOS page source")]
        public void APatientCanClickToViewCovidConditionsIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .GetInformationAboutCoronavirus();

            IOSAppTab
                .AssertOnCovidConditionsPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientCanClickToGetHelpWithLoggingInAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .GetHelp();

            AndroidAppTabBrowserChoice
                .AssertDisplayed(driver)
                .ChooseChrome()
                .Always();

            AndroidAppTab
                .AssertOnLoginHelpPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientCanClickToGetHelpWithLoggingInIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .GetHelp();

            IOSAppTab
                .AssertOnLoginHelpPage(driver);
        }
    }
}