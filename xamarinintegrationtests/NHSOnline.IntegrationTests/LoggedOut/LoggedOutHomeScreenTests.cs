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

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .ChooseChrome()
                .JustOnce()
                .AssertOnCovidConditionsPage();
        }

        [NhsAppIOSTest]
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

            AndroidAppTab
                .AssertOnBrowserChoice(driver)
                .ChooseChrome()
                .JustOnce()
                .AssertOnLoginHelpPage();
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