using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-03.1", "Continuing with NHS login displays NHS login when the before you start screen has previously been acknowledged")]
    public class NhsLoginGettingStartedAcknowledgedTests
    {
        [NhsAppAndroidTest]
        public void APatientIsNotShownTheGettingStartedPageIfItHasAlreadyBeenAcknowledgedAndroid(IAndroidDriverWrapper driver)
        {
            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .Navigation.Close();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver);
        }
    }
}