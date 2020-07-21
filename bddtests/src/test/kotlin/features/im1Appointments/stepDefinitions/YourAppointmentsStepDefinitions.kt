package features.im1Appointments.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.im1Appointments.steps.AppointmentsConfirmationSteps
import features.im1Appointments.steps.YourAppointmentsTelephoneSteps
import features.im1Appointments.steps.YourAppointmentsUISteps
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import mocking.vision.VisionConstants
import mockingFacade.appointments.MyAppointmentsFacade
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.AppointmentHubPage
import pages.ErrorDialogPage
import pages.appointments.BookingSuccessPage
import pages.appointments.CancellingSuccessPage
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.avoidChromeWebDriverServiceCrash
import pages.isCurrentlyEnabled
import pages.isDisplayed
import pages.navigation.WebHeader
import pages.waitUntilPresent

private const val ERROR_SCENARIO = "error scenario"
private const val ERROR_SCENARIO_SECOND = "error scenario second"
private const val ERROR_SCENARIO_WILL_SUCCEED = "to succeed"

class YourAppointmentsStepDefinitions {

    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps
    @Steps
    lateinit var yourAppointmentsUISteps: YourAppointmentsUISteps
    @Steps
    lateinit var bookingSuccessPage: BookingSuccessPage
    @Steps
    lateinit var cancelSuccessPage: CancellingSuccessPage
    @Steps
    lateinit var yourAppointmentsTelephoneSteps: YourAppointmentsTelephoneSteps
    @Steps
    lateinit var appointmentHubPage: AppointmentHubPage

    private lateinit var webHeader: WebHeader
    private lateinit var errorDialogPage: ErrorDialogPage

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    @Given("^(.*) user is not allowed to view appointments")
    fun visionUserIsNotAllowedToViewAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        if (supplier == Supplier.VISION) {
            Serenity.setSessionVariable(VisionConstants.gpAppointmentsDisabled).to("true")
        }
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        viewAppointmentFactory.generateDefaultUserData()
        viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.BOTH) {
            respondWithGPErrorWhenNotEnabled()
        }
    }

    @Given("^(.*) returns corrupted response once when trying to retrieve my appointments$")
    fun gpSystemReturnsCorruptedResponseOnceWhenTryingToRetrieveMyAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(supplier)
        val example = MyAppointmentsFacade(
                appointmentSlotsExample.getGenericExample()
        )
        viewAppointmentFactory.generateDefaultUserData()
        viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.BOTH) {
            respondWithCorrupted()
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo(ERROR_SCENARIO_SECOND)
        }
        viewAppointmentFactory.mockMyAppointments(IMyAppointmentsBuilder.AppointmentType.BOTH) {
            respondWithSuccess(example)
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(ERROR_SCENARIO_SECOND)
                    .willSetStateTo(ERROR_SCENARIO_WILL_SUCCEED)
        }
    }

    @When("^I select \"([^\"]*)\" button$")
    fun whenISelectButton(buttonText: String) {
        yourAppointmentsUISteps.yourAppointmentsPage.locatorMethods.waitForNativeStepToComplete()
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        yourAppointmentsUISteps.yourAppointmentsPage.avoidChromeWebDriverServiceCrash()
        yourAppointmentsUISteps.yourAppointmentsPage.clickOnButtonContainingText(buttonText)
    }

    @When("^I select a \"Cancel this appointment\" link$")
    fun iSelectACancelThisAppointmentLink() {
        yourAppointmentsUISteps.yourAppointmentsPage.clickFirstCancelAppointmentLink()
    }

    @Then("^I select the back to home link on the appointments page$")
    fun thenISelectTheBackToHomeLink() {
        yourAppointmentsUISteps.yourAppointmentsPage.clickOnBackLink()
    }

    @Then("^the Appointment Slot page is displayed$")
    fun theAppointmentSlotPageIsDisplayed() {
        webHeader.getPageTitle().withText("Confirm your appointment")
        appointmentsConfirmationSteps.checkAppointmentDetails()
    }

    @Then("^the Appointment Booking success message is displayed$")
    fun appointmentBookingSuccessMessage() {
        yourAppointmentsUISteps.yourAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(yourAppointmentsUISteps.yourAppointmentsPage.bookButton)
        bookingSuccessPage.checkBookingSuccessMessage()
        bookingSuccessPage.checkAppointmentDetails()
    }

    @Then("^the Appointment Booking success page is displayed$")
    fun appointmentBookingSuccessPage() {
        bookingSuccessPage.checkBookingSuccessMessage()
        yourAppointmentsUISteps.checkBackToAppointmentsLink()
        bookingSuccessPage.checkAppointmentDetails()
    }

    @Then("^the Appointment Cancel success page is displayed$")
    fun appointmentCancelSuccessPage() {
        cancelSuccessPage.checkCancelSuccessMessage()
        yourAppointmentsUISteps.checkBackToAppointmentsLink()
    }

    @Then("^the Appointment Hub page is displayed$")
    fun appointmentHubPage() {
        appointmentHubPage.assertAppointmentsHubIsDisplayed()
    }

    @Then("^the booked appointment before cutoff time is correctly displayed with ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancel() {
        yourAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        yourAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @Then("^booked appointments before and one appointment within cutoff time " +
            "are correctly displayed with relevant ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancelExceptOnesWithinCutoffTime() {
        yourAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        yourAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment(1)
    }

    @Then("^the Appointment Booking success page is displayed without reference to being able to cancel$")
    fun appointmentBookingConfirmationPageIsDisplayedWithoutReferenceToCancel() {
        bookingSuccessPage.checkBookingSuccessMessage()
        bookingSuccessPage.checkAppointmentDetails()
    }

    @Then("^the booked appointment is correctly displayed without ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithoutCancel() {
        yourAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        yourAppointmentsUISteps.verifyThatThereAreNoCancelLinks()
    }

    @Then("^I can book an appointment$")
    fun iCanBookAnAppointment() {
        yourAppointmentsUISteps.yourAppointmentsPage.bookButton.assertSingleElementPresent().assertIsVisible()
        yourAppointmentsUISteps.yourAppointmentsPage.bookButton.waitUntilPresent()

        Assert.assertTrue("Book an appointment is not displaying",
                yourAppointmentsUISteps.yourAppointmentsPage.bookButton.isDisplayed)

        Assert.assertTrue("Book an appointment is not enabled",
                yourAppointmentsUISteps.yourAppointmentsPage.bookButton.isCurrentlyEnabled)
    }

    @Then("^the page title is \"Your GP appointments\"$")
    fun thePageTitleIsYourAppointments() {
        yourAppointmentsUISteps.checkHeaderTextIsCorrect()
    }

    @Then("^the Your Appointments page is displayed$")
    fun iWillBeOnTheMyAppointmentsScreen() {
        yourAppointmentsUISteps.yourAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(yourAppointmentsUISteps.yourAppointmentsPage.bookButton)
        iCanBookAnAppointment()
        thePageTitleIsYourAppointments()
    }

    @Then("^I am informed I have no upcoming appointments$")
    fun thenIAmInformedIHaveNoBookedAppointments() {
        yourAppointmentsUISteps.checkNoUpcomingAppointmentsTextIsDisplaying()
    }

    @Then("^I am informed I have no historical appointments$")
    fun thenIAmInformedIHaveNoHistoricalAppointments() {
        yourAppointmentsUISteps.checkNoHistoricalAppointmentsTextIsDisplaying()
    }

    @Then("^I am not informed I have no historical appointments$")
    fun thenIAmNotInformedIHaveNoHistoricalAppointments() {
        yourAppointmentsUISteps.yourAppointmentsPage.assertPastTextNotPresent()
    }

    @Then("^I am given the list of upcoming appointments$")
    fun thenIAmGivenTheListOfUpcomingAppointments() {
        yourAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        yourAppointmentsUISteps.checkIfUpcomingSlotsAreInCorrectOrder()
    }

    @Then("^I am given the list of historical appointments$")
    fun thenIAmGivenTheListOfHistoricalAppointments() {
        yourAppointmentsUISteps.checkHistoricalAppointmentsAreCorrectlyPopulated()
        yourAppointmentsUISteps.checkIfHistoricalSlotsAreInCorrectOrder()
    }

    @Then("^each appointment can be cancelled$")
    fun eachAppointmentCanBeCancelled() {
        Assert.assertEquals(
                "Missing at least one cancel link. ",
                yourAppointmentsUISteps.yourAppointmentsPage.getWebAppointmentSlotDivs().size,
                yourAppointmentsUISteps.yourAppointmentsPage.getNumberOfCancelLinks()
        )
    }

    @Then("^no appointment can be cancelled$")
    fun noAppointmentCanBeCancelled() {
        yourAppointmentsUISteps.verifyThatThereAreNoCancelLinks()
    }

    @Then("^I can see the list of upcoming telephone appointments")
    fun thenICanSeeTheListOfUpcomingTelephoneAppointments() {
        yourAppointmentsTelephoneSteps.checkUpcomingTelephoneAppointmentsAreCorectlyPopulated()
    }

    @Then("^I can see the list of past telephone appointments$")
    fun thenICanSeeTheListOfPastTelephoneAppointments() {
        yourAppointmentsTelephoneSteps.checkPastTelephoneAppointmentsAreCorectlyPopulated()
    }

    @Then("^I can see the booked telephone appointment and it has a cancel link")
    fun thenICanSeeTheBookedTelephoneAppointmentAndItHasACancelLink() {
        yourAppointmentsTelephoneSteps.checkUpcomingTelephoneAppointmentsAreCorectlyPopulated()
        yourAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @Then("^I see appropriate error message when appointments are disabled$")
    fun iSeeAppropriateErrorMessageWhenAppointmentsAreDisabled() {
        errorDialogPage
                .assertPageTitle(yourAppointmentsUISteps.yourAppointmentsPage.unavailableTitle)
                .assertPageHeader(yourAppointmentsUISteps.yourAppointmentsPage.unavailableTitle)
                .assertParagraphText(yourAppointmentsUISteps.yourAppointmentsPage.notAbleToBook)
                .assertParagraphText(yourAppointmentsUISteps.yourAppointmentsPage.contactForMoreInformation)
                .assertSubHeader(yourAppointmentsUISteps.yourAppointmentsPage.coronaVirusHeader)
                .assertParagraphText(yourAppointmentsUISteps.yourAppointmentsPage.coronaVirusText)
                .assertLink(yourAppointmentsUISteps.yourAppointmentsPage.coronaVirusLink)

    }

    @Then("^I see appropriate try again error message when there is an error with '(.*)'$")
    fun iSeeAppropriateTryAgainErrorMessageWhenThereIsAnErrorWithPrefix(prefix: String) {
        errorDialogPage.assertReferenceCode(prefix)
                .assertParagraphText(yourAppointmentsUISteps.yourAppointmentsPage.ifItContinues)
                .assertParagraphText(yourAppointmentsUISteps.yourAppointmentsPage.tryAgainNow)
                .assertPageHeader(yourAppointmentsUISteps.yourAppointmentsPage.problemLoadingTitle)
                .assertPageTitle(yourAppointmentsUISteps.yourAppointmentsPage.problemLoadingTitle)
    }

    @Then("^I see appropriate try again book/cancel error message when there is an error with '(.*)'$")
    fun iSeeAppropriateTryAgainBookCancelErrorMessageWhenThereIsAnErrorWithPrefix(prefix: String) {
        val tryAgainParagraph = yourAppointmentsUISteps.yourAppointmentsPage.getTryAgainNowParagraph(prefix)
        errorDialogPage.assertParagraphText(tryAgainParagraph)
                .assertParagraphText(yourAppointmentsUISteps.yourAppointmentsPage.ifItContinuesBookOrCancel)
                .assertPageHeader(yourAppointmentsUISteps.yourAppointmentsPage.problemLoadingTitle)
                .assertPageTitle(yourAppointmentsUISteps.yourAppointmentsPage.problemLoadingTitle)
    }
}
