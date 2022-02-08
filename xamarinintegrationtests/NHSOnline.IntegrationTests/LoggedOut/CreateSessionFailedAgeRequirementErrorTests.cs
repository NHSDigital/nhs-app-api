using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-08.7", "Log in when a user is under 13 displays a shutter screen")]
    public class CreateSessionFailedAgeRequirementErrorTests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenAPatientIsUnder13Android(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithAge(12, 300)
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPageSlimHeader
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionFailedAgeRequirementErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();

            driver.PressBackButton();

            AndroidLoggedOutHomePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenAPatientIsUnder13Ios(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithAge(12, 300)
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionFailedAgeRequirementErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();

            driver.SwipeBack();

            IOSLoggedOutHomePage
                .AssertOnPage(driver);
        }
    }
}