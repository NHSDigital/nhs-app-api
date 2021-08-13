using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.TermsAndConditions
{
    [TestClass]
    [BusinessRule("BR-LOG-06.4", "Invoking native back on the conditions of use screen screen has no associated action")]
    [BusinessRule("BR-LOG-05.3", "Log in when a user has not previously accepted the NHS App conditions of use displays the conditions of use screen")]
    public class TermsAndConditionsBackTests
    {
        [NhsAppAndroidTest]
        public void APatientNavigatingBackFromTermsAndConditionsRemainsOnTheSamePageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidGettingStartedPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent.Login(patient);

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver)
                .AssertPageContent();

            driver
                .PressBackButton()
                .WaitForBackAction();

            driver.AssertRunningInForeground();

            AndroidTermsAndConditionsPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientNavigatingBackFromTermsAndConditionsRemainsOnTheSamePageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
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

            IOSTermsAndConditionsPage
                .AssertOnPage(driver)
                .AssertPageContent();

            driver
                .SwipeBack()
                .WaitForBackAction();

            driver.AssertRunningInForeground();

            IOSTermsAndConditionsPage
                .AssertOnPage(driver);
        }
    }
}