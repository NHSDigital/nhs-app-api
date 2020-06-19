using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public class AndroidLoginTests
    {
        [NhsAppAndroidTest("A patient can start the app on android")]
        public void APatientCanStartTheApp(IAndroidDriverWrapper driver)
        {
            _ = AndroidLoggedOutHomePage.AssertOnPage(driver);
        }
    }
}