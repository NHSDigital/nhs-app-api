using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.SecondaryCare;
using NHSOnline.IntegrationTests.Pages.IOS.Wayfinder;
using NHSOnline.IntegrationTests.Pages.IOS.Appointments;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookWayfinderSecondaryCareSummaryPageTests
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - iOS",
            FlipbookTestName = "A user accesses the Appointment Hub when Wayfinder is toggled on")]
        public void APatientWithProofLevelNineCanAccessAppointmentHubWhenWayfinderToggledOnIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("2014105132");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot: true)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver, screenshot: true)
                .PageContent.NavigateToSecondaryCareSummaryPage();
        }

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user logs in when they have no secondary care referrals or appointments")]
        public void APatientWithProofLevelNineAndNoReferralsOrAppointmentsCanViewWayfinderSCSPSignpostingScreensIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("2014105131");
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
                .ScrollToAppointmentsHelpPageLinkAndScreenshotThenClick();

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

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user logs in and attempts to access Secondary Care while Aggregator is unavailable")]
        public void APatientWithProofLevelNineCanViewWayfinderSCSPWithAggregatorErrorIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("2092013752");
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

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user attempts to access Secondary Care Summary Screen when under 16 years old")]
        public void APatientWithProofLevelNineAndUnderSixteenViewsWayfinderSCSPIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithAge(13,20)
                .WithNhsNumber("2090220899");
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

        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user accesses the Appointment Hub when Wayfinder is toggled on - iOS",
            FlipbookTestName = "A user accesses secondary care referrals or appointments and clicks a deep link")]
        public void APatientWithProofLevelNineCanViewWayfinderSCSPAndClickDeepLinksIOS(IIOSDriverWrapper driver)
        {
            var patient = new WayfinderPatient(WayfinderPatientOds.ERS)
                .WithName(b => b.GivenName("Terry").FamilyName("Tibbs"))
                .WithNhsNumber("2014105132");
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver)
                .Navigation.NavigateToAppointments();

            IOSAppointmentsPage
                .AssertOnPage(driver)
                .PageContent.NavigateToSecondaryCareSummaryPage();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver, screenshot:true, errorType:WayfinderErrorType.none, totalReferralsOrAppointments:7, totalConfirmedAppointments:5, totalReferralsInReview:2)
                .ScrollToReadyToConfirmAppointmentDeepLinkButtonAndScreenshotThenClick();

            IOSBlueScreenInterruptPage
                .AssertOnPage(driver, screenshot: true);

            driver.SwipeBack();

            IOSSecondaryCareSummaryPage
                .AssertOnPage(driver, screenshot:true, errorType:WayfinderErrorType.none, totalReferralsOrAppointments:7, totalConfirmedAppointments:5, totalReferralsInReview:2)
                .ScrollToCancelledAppointmentDeepLinkButtonAndScreenshotThenClick();

            IOSBlueScreenInterruptPage
                .AssertOnPage(driver, screenshot: true);
        }
    }
}