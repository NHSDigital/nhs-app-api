using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.DeviceAndOperatingSystemCompatibility
{
    [TestClass]
    public class LoginWithP9OlderOSVersionTests
    {
        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhoneX, OSVersion = IOSVersion.Eleven)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS11(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOS11GettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            IOS11TermsAndConditionsPage
                .AssertOnPage(driver)
                .AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Wendy House");
        }


        [NhsAppIOSTest(IOSDevice = IOSDevice.iPhoneXR, OSVersion = IOSVersion.Twelve)]
        public void APatientWithProofLevelNineCanSuccessfullyLogInOnIOS12(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Wendy").FamilyName("House"));
            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOS11GettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .AssertVectorOfTrust()
                .Login(patient);

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .PageContent.AcceptTermsAndConditions();

            IOSUserResearchOptInPage
                .AssertOnPage(driver)
                .PageContent.OptInToUserResearch();

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .AssertPageDisplayedFor("Wendy House");
        }
    }
}