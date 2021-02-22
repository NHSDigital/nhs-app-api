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
    [BusinessRule("BR-LOG-01.10", "Following the link for Coronavirus when the link is available on the logged out home screen navigates the user to the appropriate location")]
    public sealed class CovidBannerTests
    {
        [NhsAppAndroidTest]
        public void APatientCanViewCovidBannerAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertCovidBannerPresent();
        }

        [NhsAppIOSTest]
        public void APatientCanViewCovidBannerIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertCovidBannerPresent();
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
        public void APatientCanClickToViewCovidConditionsIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .GetInformationAboutCoronavirus();

            IOSAppTab
                .AssertOnCovidConditionsPage(driver);
        }
    }
}