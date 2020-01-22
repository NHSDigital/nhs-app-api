package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.im1Appointments.steps.AppointmentsConfirmationSteps
import features.im1Appointments.steps.CancelAppointmentSteps
import features.im1Appointments.steps.YourAppointmentsTelephoneSteps
import features.im1Appointments.steps.YourAppointmentsUISteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.appointments.BookingSuccessPage
import pages.appointments.CancellingSuccessPage
import pages.assertSingleElementPresent
import pages.assertIsVisible
import pages.waitUntilPresent
import pages.isDisplayed
import pages.isCurrentlyEnabled
import pages.navigation.HeaderNative
import pages.navigation.WebHeader

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
    lateinit var cancelAppointmentSteps: CancelAppointmentSteps

    lateinit var headerNative: HeaderNative
    lateinit var webHeader: WebHeader

    @When("^I select \"([^\"]*)\" button$")
    fun whenISelectButton(buttonText: String) {
        yourAppointmentsUISteps.yourAppointmentsPage.locatorMethods.waitForNativeStepToComplete()
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
    }

    @Then("^the Appointment Booking success page is displayed$")
    fun appointmentBookingSuccessPage() {
        bookingSuccessPage.checkBookingSuccessMessage()
        yourAppointmentsUISteps.checkBackToAppointmentsLink()
    }

    @Then("^the Appointment Cancel success page is displayed$")
    fun appointmentCancelSuccessPage() {
        cancelSuccessPage.checkCancelSuccessMessage()
        yourAppointmentsUISteps.checkBackToAppointmentsLink()
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

    @Then("^the page title is \"Your appointments\"$")
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

    @Then("^I see page header indicating there is an appointment data error$")
    fun iSeePageHeaderIndicatingAppointmentDataError() {
        webHeader.getPageTitle().withText("Appointment data error")
    }

    @Then("^I see the appropriate error messages for the appointment data error$")
    fun iSeeTheAppropriateErrorMessagesForTheAppointmentDataError() {
        yourAppointmentsUISteps.checkAppointmentDataErrorMessagesAreCorrect()
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
}
