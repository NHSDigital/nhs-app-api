using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public class AndroidLoginTests
    {
        [NhsAppAndroidTest()]
        public void APatientCanStartTheApp(IAndroidDriverWrapper driver)
        {
            _ = AndroidLoggedOutHomePage.AssertOnPage(driver);
        }

        [NhsAppAndroidTest()]
        public void APatientIsShownTheBeforeYouStartPage(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            _ = AndroidBeforeYouStartPage.AssertOnPage(driver);
        }
    }
}
