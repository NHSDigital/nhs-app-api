using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public class IOSLoginTests
    {
        [NhsAppIOSTest()]
        public void APatientCanStartTheApp(IIOSDriverWrapper driver)
        {
            _ = IOSLoggedOutHomePage.AssertOnPage(driver);
        }

        [NhsAppIOSTest()]
        public void APatientIsShownTheBeforeYouStartPage(IIOSDriverWrapper driver)
        {
            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            _ = IOSBeforeYouStartPage.AssertOnPage(driver);
        }
    }
}