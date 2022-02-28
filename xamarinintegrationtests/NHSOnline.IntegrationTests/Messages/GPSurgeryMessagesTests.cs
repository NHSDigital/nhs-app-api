using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Messages
{
    [TestClass]
    public class GpSurgeryMessagesTests
    {
        [NhsAppAndroidTest]
        public void APatientIsShownAnErrorIfTheyDoNotHaveAccessToGpSurgeryMessagingAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Garry").FamilyName("Messi"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .PageContent.NavigateToGPSurgeryMessages();

            AndroidGpSurgeryMessagesPage
                .AssertOnPage(driver);

            driver.PressBackButton();

            AndroidMessagesPage
                .AssertOnPage(driver);
        }

        [NhsAppIOSTest]
        public void APatientIsShownAnErrorIfTheyDoNotHaveAccessToGpSurgeryMessagingIOS(IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Garry").FamilyName("Messi"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToMessages();

            IOSMessagesPage
                .AssertOnPage(driver)
                .PageContent
                .NavigateToGPSurgeryMessages();

            IOSGpSurgeryMessagesPage
                .AssertOnPage(driver);

            driver.SwipeBack();

            IOSMessagesPage
                .AssertOnPage(driver);
        }
    }
}