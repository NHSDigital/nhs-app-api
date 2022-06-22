using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.SecondaryCare;
using NHSOnline.IntegrationTests.Pages.Android.Appointments;
using NHSOnline.IntegrationTests.Pages.Android.Home;
using NHSOnline.IntegrationTests.Pages.Android.Wayfinder;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookWayfinderSecondaryCareSummaryPageTests
    {
        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - Android",
            FlipbookTestName = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on")]
        public void APatientWithProofLevelNineCanAccessAppointmentHubWhenWayfinderToggledOnAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9414105132");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .NavigateToAppointments();

             AndroidAppointmentsPage
                 .AssertOnPage(driver, screenshot: true)
                 .PageContent.NavigateToSecondaryCareSummaryPage();
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - iOS",
            FlipbookTestName = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on")]
        public void APatientWithProofLevelNineCanAccessAppointmentHubWhenWayfinderToggledOnIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9414105132");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateToSecondaryCareSummaryPage();
        }

        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - Android",
            FlipbookTestName = "A user logs in when they have no secondary care referrals or appointments")]
        public void APatientWithProofLevelNineAndNoReferralsOrAppointmentsCanViewWayfinderSCSPSignpostingScreensAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9414105131");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

             AndroidAppointmentsPage
                 .AssertOnPage(driver)
                 .PageContent.NavigateToSecondaryCareSummaryPage();

             AndroidSecondaryCareSummaryPage
                 .AssertOnPage(driver)
                 .ScrollToMissingOrIncorrectReferralsAppointmentsHelpPageLinkAndScreenshotThenClick();

            AndroidWayfinderHelpPage
                .AssertOnPage(driver, screenshot: true)
                .TabIntoFocus()
                .KeyboardNavigateViaBackButton();

            AndroidSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToConfirmedAppointmentsHelpPageLinkAndScreenshotThenClick();

            AndroidWayfinderHelpPage
                .AssertOnPage(driver, screenshot: true)
                .TabIntoFocus()
                .KeyboardNavigateViaBackButton();

            AndroidSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToReferralsInReviewHelpPageLinkAndScreenshotThenClick();

            AndroidWayfinderHelpPage
                .AssertOnPage(driver, screenshot: true)
                .TabIntoFocus()
                .KeyboardNavigateViaBackButton();
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user logs in when they have no secondary care referrals or appointments")]
        public void APatientWithProofLevelNineAndNoReferralsOrAppointmentsCanViewWayfinderSCSPSignpostingScreensIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9414105131");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToSecondaryCareSummaryPage();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToMissingOrIncorrectReferralsAppointmentsHelpPageLinkAndScreenshotThenClick();

            IOSWayfinderHelpPage
                .AssertOnPage(driver, screenshot: true)
                 .PageContent.NavigateViaBackButton();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToConfirmedAppointmentsHelpPageLinkAndScreenshotThenClick();

            IOSWayfinderHelpPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateViaBackButton();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToReferralsInReviewHelpPageLinkAndScreenshotThenClick();

            IOSWayfinderHelpPage
                .AssertOnPage(driver, screenshot: true)
                 .PageContent.NavigateViaBackButton();
        }

        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - Android",
            FlipbookTestName = "A user logs in and attempts to access Secondary Care while Aggregator is unavailable")]
        public void APatientWithProofLevelNineCanViewWayfinderSCSPWithAggregatorErrorAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9392013752");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

             AndroidAppointmentsPage
                 .AssertOnPage(driver)
                 .PageContent.NavigateToSecondaryCareSummaryPage();

             AndroidSecondaryCareSummaryPage
                 .AssertOnPage(driver, screenshot: true, errorType: WayfinderErrorType.generalError);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user logs in and attempts to access Secondary Care while Aggregator is unavailable")]
        public void APatientWithProofLevelNineCanViewWayfinderSCSPWithAggregatorErrorIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9392013752");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToSecondaryCareSummaryPage();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver, screenshot: true, errorType: WayfinderErrorType.generalError);
        }

        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - Android",
            FlipbookTestName = "A user attempts to access Secondary Care Summary Screen when under 16 years old")]
        public void APatientWithProofLevelNineAndUnderSixteenViewsWayfinderSCSPAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithAge(13,20)
                .WithNhsNumber("9290220899");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

            AndroidAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToSecondaryCareSummaryPage();

            AndroidSecondaryCareSummaryPage
                .AssertOnPage(driver, screenshot: true, errorType: WayfinderErrorType.underSixteen);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user attempts to access Secondary Care Summary Screen when under 16 years old")]
        public void APatientWithProofLevelNineAndUnderSixteenViewsWayfinderSCSPIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithAge(13,20)
                .WithNhsNumber("9290220899");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToSecondaryCareSummaryPage();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver, screenshot: true, errorType: WayfinderErrorType.underSixteen);
        }

        [NhsAppAndroidTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - Android",
            FlipbookTestName = "A user accesses secondary care referrals or appointments and clicks a deep link")]
        public void APatientWithProofLevelNineCanViewWayfinderSCSPAndClickDeepLinksAndroid(IAndroidDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9414105132");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogAndroidPatientIn(driver, patient);

            AndroidLoggedInHomePage
                .AssertOnPage(driver)
                .NavigateToAppointments();

             AndroidAppointmentsPage
                 .AssertOnPage(driver)
                 .PageContent.NavigateToSecondaryCareSummaryPage();

             AndroidSecondaryCareSummaryPage
                 .AssertOnPage(driver)
                 .ScrollToReadyToConfirmAppointmentDeepLinkButtonAndScreenshotThenClick();

             AndroidBlueScreenInterruptPage
                 .AssertOnPage(driver, screenshot: true);

             driver.PressBackButton();

             AndroidSecondaryCareSummaryPage
                 .AssertOnPage(driver)
                 .ScrollToCancelledAppointmentDeepLinkButtonAndScreenshotThenClick();

             AndroidBlueScreenInterruptPage
                 .AssertOnPage(driver, screenshot: true);
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app and accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user accesses secondary care referrals or appointments and clicks a deep link")]
        public void APatientWithProofLevelNineCanViewWayfinderSCSPAndClickDeepLinksIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("9414105132");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToSecondaryCareSummaryPage();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToReadyToConfirmAppointmentDeepLinkButtonAndScreenshotThenClick();

            IOSBlueScreenInterruptPage
                .AssertOnPage(driver, screenshot: true);

            driver.SwipeBack();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver)
                .ScrollToCancelledAppointmentDeepLinkButtonAndScreenshotThenClick();

            IOSBlueScreenInterruptPage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}

