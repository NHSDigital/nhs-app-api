using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration.Uplift
{
    [TestClass]
    [BusinessRule("BR-LOG-12.3", "Using the native back when the user has navigated to the NHS login uplift journey from the logged in home screen displays the logged in home screen")]
    public class NhsLoginUpliftFromHomeBackTests
    {
        [NhsAppAndroidTest]
        public void PressingTheBackButtonOnUpliftJourneyLaunchedFromTheHomepageShowsTheHomepageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            AndroidStubbedLoginUpliftPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidLoggedInHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void SwipingBackOnTheUpliftJourneyLaunchedFromTheHomepageShowsTheHomepageIos(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient();
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.ProveYourIdentityContinue();

            IOSStubbedLoginUpliftPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSLoggedInHomePage
                .AssertOnPage(driver);
        }
    }
}