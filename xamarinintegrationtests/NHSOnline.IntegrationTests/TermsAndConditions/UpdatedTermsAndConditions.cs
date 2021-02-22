using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Mongo.TermsAndConditions;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.LoggedOut;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LoggedOut;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.TermsAndConditions
{
    [TestClass]
    [BusinessRule("BR-LOG-05.4", "Log in when the conditions have changed since the user last accepted them displays the updated conditions of use screen")]
    public class UpdatedTermsAndConditions
    {
        [NhsAppAndroidTest]
        public void ShowUpdatedTermsAndConditionsPageAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));

            ConsentCollection.Add(patient.ToConsent() with { DateOfConsent = "2018-11-11T00:00:00+00:00" });

            using var patients = Mocks.Patients.Add(patient);

            AndroidLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            AndroidBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            AndroidStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .Login(patient);

            AndroidUpdatedTermsAndConditionsPage
                .AssertOnPage(driver)
                .AssertPageContent();
        }

        [NhsAppIOSTest]
        public void ShowUpdatedTermsAndConditionsPageIos(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));

            ConsentCollection.Add(patient.ToConsent() with { DateOfConsent = "2018-11-11T00:00:00+00:00" });

            using var patients = Mocks.Patients.Add(patient);

            IOSLoggedOutHomePage
                .AssertOnPage(driver)
                .ContinueWithNhsLogin();

            IOSBeforeYouStartPage
                .AssertOnPage(driver)
                .Continue();

            IOSStubbedLoginPage
                .AssertOnPage(driver)
                .PageContent
                .Login(patient);

            IOSUpdatedTermsAndConditionsPage
                .AssertOnPage(driver)
                .AssertPageContent();
        }
    }
}