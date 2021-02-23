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
    [BusinessRule("BR-LOG-01.11", "Following the link for Help on the logged out home screen navigates the user to the appropriate location")]
    public class HelpIconTests
    {
        [NhsAppAndroidTest]
        public void APatientCanViewHelpIconAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertHelpIconPresent();
        }

        [NhsAppIOSTest]
        public void APatientCanViewHelpIconIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertHelpIconPresent();
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