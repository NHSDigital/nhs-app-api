using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    public class NhsLoginWebViewUserAgentTests
    {

        [NhsAppAndroidTest]
        public void APatientWithProofLevelFiveCanAccessNhsLoginAndTheWebViewUserAgentIsSetAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"))
                .WithProofLevel5();
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.AssertUserAgent(Platform.Android);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelFiveCanAccessNhsLoginAndTheWebViewUserAgentIsSetIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Fred").FamilyName("Jones"))
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
                .PageContent.AssertUserAgent(Platform.Ios);
        }
    }
}