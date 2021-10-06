using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.Android.More;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-01.1", "Launching the app displays the logged out home screen")]
    public class VersionNumberTests
    {
        [NhsAppAndroidTest]
        public void APatientCanSeeTheCorrectVersionNumberWhenLoggedOutAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertCorrectVersionText();
        }

        [NhsAppIOSTest]
        public void APatientCanSeeTheCorrectVersionNumberWhenLoggedOutIOS(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .AssertCorrectVersionText();
        }
    }
}
