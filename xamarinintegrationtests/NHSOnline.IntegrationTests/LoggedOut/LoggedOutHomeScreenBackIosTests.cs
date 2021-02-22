using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-09.2","Invoking native back on iOS on the logged out home screen has no action")]
    public class LoggedOutHomeScreenBackIosTests
    {

        [NhsAppIOSTest]
        public void APatientCanNotSwipeToCloseTheAppOnTheLoggedOutHomeScreenIos(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}