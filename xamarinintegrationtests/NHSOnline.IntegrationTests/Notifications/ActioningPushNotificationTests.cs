using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.Notifications
{
    [TestClass]
    [BusinessRule("BR-NOT-01.1","Actioning a push notification with a link to an area of the app when the app is closed opens the app to the logged-out home screen and redirects the user to the relevant screen once logged in")]
    [BusinessRule("BR-NOT-01.2","Actioning a push notification with a link to an area of the app when the app is open and logged-out remains on the same screen and redirects the user to the relevant screen once logged in")]
    [BusinessRule("BR-NOT-01.3","Actioning a push notification with a link to an area of the app when the app is open and on NHS login journey redirects the user to the relevant screen once logged in is completed")]
    [BusinessRule("BR-NOT-01.4","Actioning a push notification with a link to an area of the app when the app is open and logged in redirects the user to the relevant screen")]
    [BusinessRule("BR-NOT-01.5","Actioning a push notification on an Android device with a link to an area of the app when the app is open and logged in on a browser overlay closes the browser overlay and redirects to the relevant screen")]
    [BusinessRule("BR-NOT-01.6","Actioning a push notification with a link to an area of the app when the app is open and logged in on a web integration redirects the user to the relevant screen")]
    [BusinessRule("BR-NOT-01.7","Actioning a push notification with a link to a web integrated service when the app is closed opens the app to the logged-out home screen and redirects the user to the web integrated service once logged in")]
    [BusinessRule("BR-NOT-01.8","Actioning a push notification with a link to a web integrated service when the app  is open and logged out remains on the same screen and redirects the user to the web integrated service once logged in")]
    [BusinessRule("BR-NOT-01.9","Actioning a push notification with a link to a web integrated service when the app is open and on NHS login journey redirects the user to the web integrated service once log in is complete")]
    [BusinessRule("BR-NOT-01.10","Actioning a push notification with a link to a web integrated service when the app is open and logged in redirects the user to the web integrated service")]
    [BusinessRule("BR-NOT-01.11","Actioning a push notification on an Android device with a link to a web integrated service when the app is open and logged in on a browser overlay closes the browser overlay and redirects to the relevant screen")]
    [BusinessRule("BR-NOT-01.12","Actioning a push notification with a link to a web integrated service when the app is open and logged in on a web integration redirects the user to the web integrated service")]
    [BusinessRule("BR-NOT-01.13","Actioning a push notification with a link to an unsupported service when the app is closed opens the app and displays the logged in home screen once logged in")]
    [BusinessRule("BR-NOT-01.14","Actioning a push notification with a link to an unsupported service when the app is open and logged out remains on the same screen and displays the logged in home screen once logged in")]
    [BusinessRule("BR-NOT-01.15","Actioning a push notification with a link to an unsupported service when the app is open and on NHS login journey displays the logged in home screen once logged in")]
    [BusinessRule("BR-NOT-01.16","Actioning a push notification with a link to an unsupported service when the app is open and logged in stays on or redirects to the logged in home screen")]
    [BusinessRule("BR-NOT-01.17","Actioning a push notification on an Android device with a link to an unsupported service when the app is open and logged in on a browser overlay closes the browser overlay and redirects to the home screen")]
    [BusinessRule("BR-NOT-01.18","Actioning a push notification with a link to an unsupported service when the app is open and logged in on a web integration redirects to the home screen")]
    [BusinessRule("BR-NOT-01.19","Actioning a push notification with a link in an invalid format when the app is closed opens the app on the logged out home page and displays the logged in home page once logged in	")]
    [BusinessRule("BR-NOT-01.20","Actioning a push notification with a link in an invalid format when the app is open and logged out remains on the same screen and displays the logged in home screen once logged in")]
    [BusinessRule("BR-NOT-01.21","Actioning a push notification with a link in an invalid format when the app is open and on NHS login journey displays the logged in home screen once logged in")]
    [BusinessRule("BR-NOT-01.22","Actioning a push notification with a link in an invalid format when the app is open and logged in remains on the home screen")]
    [BusinessRule("BR-NOT-01.23","Actioning a push notification on an Android device with a link in an invalid format when the app is open and logged in on a browser overlay remains on the same screen")]
    [BusinessRule("BR-NOT-01.24","Actioning a push notification with a link in an invalid format when the app is open and logged in on a web integration redirects to the logged in home screen")]
    [BusinessRule("BR-NOT-01.25","Actioning a push notification without a link when the app is closed opens the app")]
    [BusinessRule("BR-NOT-01.26","Actioning a push notification without a link when the app is open and logged out remains on the same screen")]
    [BusinessRule("BR-NOT-01.27","Actioning a push notification without a link when the app is open and on the NHS login journey returns the user to the logged out home screen")]
    [BusinessRule("BR-NOT-01.28","Actioning a push notification without a link when the app is open and logged in remains on the same screen")]
    [BusinessRule("BR-NOT-01.30","Actioning a push notification without a link when the app is open and logged in on a web integration remains on the same screen")]
    [BusinessRule("BR-NOT-01.32","Actioning a link to a web integration that the user does not have access to displays an error")]
    [BusinessRule("BR-NOT-01.33","Actioning a push notification on an iOS device with a link to an area of the app when the app is open and logged in on a browser overlay remains on the same screen")]
    [BusinessRule("BR-NOT-01.34","Actioning a push notification on an iOS device with a link to an unsupported service or invalid url when the app is open and logged in on a browser overlay remains on the same screen")]
    [BusinessRule("BR-NOT-01.35","Actioning a link to a P5 level service for a P5 user when user is logged in redirects to the appropriate screen")]
    [BusinessRule("BR-NOT-01.36","Actioning a link to a P9 level service for a P5 user when the user is logged in redirects the user the appropriate screen with the prompt to uplift")]
    [BusinessRule("BR-NOT-01.37","Actioning a push notification while in an active eConsult consultation prompts the user to confirm if they wish to exit the eConsult service")]
    public class ActioningPushNotificationTests
    {
        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileTheAppIsClosedIsTakenToTheExpectedServiceAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileTheAppIsClosedIsTakenToTheExpectedServiceIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileLoggedOutIsTakenToTheExpectedServiceAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileLoggedOutIsTakenToTheExpectedServiceIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileOnNhsLoginIsTakenToTheExpectedServiceAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileOnNhsLoginIsTakenToTheExpectedServiceIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileLoggedInIsTakenToTheExpectedServiceAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileLoggedInIsTakenToTheExpectedServiceIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileOnABrowserOverlayIsTakenToTheExpectedServiceAndroid() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileOnAWebIntegrationIsTakenToTheExpectedServiceAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToNhsAppServiceWhileOnAWebIntegrationIsTakenToTheExpectedServiceIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileTheAppIsClosedIsTakenToTheWebIntegrationAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileTheAppIsClosedIsTakenToTheWebIntegrationIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileLoggedOutIsTakenToTheWebIntegrationAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileLoggedOutIsTakenToTheWebIntegrationIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileOnNhsLoginIsTakenToTheWebIntegrationAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileOnNhsLoginIsTakenToTheWebIntegrationIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileLoggedInIsTakenToTheWebIntegrationAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileLoggedInIsTakenToTheWebIntegrationIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileOnABrowserOverlayIsTakenToTheWebIntegrationAndroid() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileOnAWebIntegrationIsTakenToTheWebIntegrationAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationWhileOnAWebIntegrationIsTakenToTheWebIntegrationIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileTheAppIsClosedIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileTheAppIsClosedIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileLoggedOutIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileLoggedOutIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileOnNhsLoginIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileOnNhsLoginIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileLoggedInIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileLoggedInIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileOnABrowserOverlayIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileOnAWebIntegrationIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
         public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileOnAWebIntegrationIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileTheAppIsClosedIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileTheAppIsClosedIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileLoggedOutIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileLoggedOutIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileOnNhsLoginIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkServiceWhileOnNhsLoginIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileLoggedInIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileLoggedInIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileOnABrowserOverlayIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileOnAWebIntegrationIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileOnAWebIntegrationIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileTheAppIsClosedIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileTheAppIsClosedIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileLoggedOutIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileLoggedOutIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileOnNhsLoginIsTakenToTheLoggedInHomePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileOnNhsLoginIsTakenToTheLoggedInHomePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileLoggedInRemainsOnTheSamePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileLoggedInRemainsOnTheSamePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileOnABrowserOverlayRemainsOnTheSamePageAndroid() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileOnABrowserOverlayRemainsOnTheSamePageIos() { }

        [NhsAppManualTest("NHSO-14226", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileOnAWebIntegrationRemainsOnTheSamePageAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithoutALinkWhileOnAWebIntegrationRemainsOnTheSamePageIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationTheyDontHaveAccessToIsShownAnErrorAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAWebIntegrationTheyDontHaveAccessToIsShownAnErrorIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnAreaOfTheAppWhileOnABrowserOverlayRemainsOnTheSameScreenIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithAnInvalidLinkWhileOnABrowserOverlayRemainsOnTheSameScreenIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationWithALinkToAnUnsupportedServiceWhileOnABrowserOverlayRemainsOnTheSameScreenIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationAsAp5UserToAp5ServiceIsDirectedToTheAppropriateScreenAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationAsAp5UserToAp5ServiceIsDirectedToTheAppropriateScreenIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationAsAp5UserToAp9ServiceIsDirectedToScreenWithAnUpliftPromptAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationAsAp5UserToAp9ServiceIsDirectedToScreenWithAnUpliftPromptIos() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationAWhileOnEConsultIsAskedIfTheyWantToExitTheEConsultJourneyAndroid() { }

        [NhsAppManualTest("NHSO-14280", "Sending notifications is not feasible for browserstack tests")]
        public void APatientActioningAPushNotificationAWhileOnEConsultIsAskedIfTheyWantToExitTheEConsultJourneyIos() { }
    }
}