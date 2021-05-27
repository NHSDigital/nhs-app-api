using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Advice;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.WebIntegration;
using NHSOnline.IntegrationTests.Pages.IOS.Advice;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.WebIntegration;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.WebIntegration
{

    [TestClass]
    public class OneOneOneWebIntegrationTests
    {
        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanAccessTheOneOneOneScreenFromAdviceAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Otis").FamilyName("Ocean"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AdviceIcon.Click();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToOneOneOne();

            AndroidOneOneOnePage
                .AssertOnPage(driver);
        }

        [NhsAppAndroidTest]
        public void APatientWithProofLevelNineCanKeyboardNavigateToAccessTheOneOneOneScreenFromAdviceAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new KeyboardPatient()
                .WithName(b => b.GivenName("Otis").FamilyName("Ocean"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .KeyboardNavigateToAdvice();

            AndroidAdvicePage
                .AssertOnPage(driver)
                .KeyboardNavigateToOneOneOne();

            AndroidOneOneOnePage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientWithProofLevelNineCanAccessTheOneOneOneScreenFromAdviceIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Otis").FamilyName("Ocean"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.AdviceIcon.Click();

            IOSAdvicePage
                .AssertOnPage(driver)
                .PageContent.NavigateToOneOneOne();

            IOSOneOneOnePage
                .AssertOnPage(driver);
        }
    }
}