using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-8.7", "Log in when a user is under 13 displays a shutter screen")]
    public class PatientUnder13Tests
    {
        [NhsAppAndroidTest]
        public void AnErrorIsDisplayedWhenAPatientIsUnder13Android(IAndroidDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithAge(12, 300);
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidCreateSessionFailedAgeRequirementErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }

        [NhsAppIOSTest]
        public void AnErrorIsDisplayedWhenAPatientIsUnder13Ios(IIOSDriverWrapper driver)
        {
            var patient = new P5Patient()
                .WithAge(12, 300);
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            IOSCreateSessionFailedAgeRequirementErrorPage
                .AssertOnPage(driver)
                .AssertPageElements();
        }
    }
}