using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests
{
    [TestClass]
    public class IOSLoginTests
    {
        [NhsAppIOSTest("A patient can start the app on iOS")]
        public void APatientCanStartTheApp(IIOSDriverWrapper driver)
        {
            _ = IOSLoggedOutHomePage.AssertOnPage(driver);
        }
    }
}