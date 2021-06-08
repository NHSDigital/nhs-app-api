using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Home
{
    [TestClass]
    public class NhsAppWebViewUserAgentTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndTheWebViewUserAgentIsSetAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertUserAgent(Platform.Android);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanSuccessfullyLogInAndTheWebViewUserAgentIsSetIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .PageContent.AssertUserAgent(Platform.Ios);
        }
    }
}