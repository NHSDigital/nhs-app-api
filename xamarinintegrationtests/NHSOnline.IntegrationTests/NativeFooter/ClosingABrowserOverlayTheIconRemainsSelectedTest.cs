using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.Android;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Messages;
using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.Messages;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.NativeFooter
{
    [TestClass]
    [BusinessRule("BR-NAV-02.7", "When closing a browser overlay the relevant icon stays selected")]
    public class ClosingABrowserOverlayTheIconRemainsSelectedTest
    {
        [NhsAppAndroidTest]
        public void APatientOnTheMessagingHubCanOpenAndCloseABrowserOverlayAndTheMessagingBottomNavIconIsStillHighlightedAndroid(
            IAndroidDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.Help();

            AndroidAppTabBrowserChoice
                .IfDisplayed(driver, choice => choice.ChooseChrome().Always());

            AndroidAppTab
                .AssertOnHomeHelpPage(driver)
                .ReturnToApp();

            AndroidMessagesPage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.AssertSelected();
        }

        [NhsAppIOSTest]
        public void APatientOnTheMessagingHubCanOpenAndCloseABrowserOverlayAndTheMessagingBottomNavIconIsStillHighlightedIOS(
            IIOSDriverWrapper driver)
        {
            var patient = new EmisPatient()
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"));
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.Click();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.Help();

            IOSAppTab
                .AssertOnHomeHelpPage(driver)
                .ReturnToApp();

            IOSMessagesPage
                .AssertOnPage(driver)
                .Navigation.MessagesIcon.AssertSelected();
        }
    }
}