package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Then
import mocking.MockingClient
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.appointments.AvailableAppointmentsPage
import pages.text

private const val WAIT_FOR_TIMEOUT = 15000L

class AvailableAppointmentsSlotsErrorStepDefinitions {

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance

    @Then("^I see appropriate information message for time-outs$")
    fun iSeeAppropriateInformationMessageAfterSecondsWhenItTimesOut() {
        Thread.sleep(WAIT_FOR_TIMEOUT)

        val expectedHeader = "There's been a problem loading this page"

        val expectedMessageText = "Try again now. If the problem continues and you need to book an appointment now, " +
                "contact your GP surgery directly. For urgent medical advice, call 111."

        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                expectedHeader, errorPage.heading.text)

        assertEquals("expected error text $expectedMessageText but found ${errorPage.errorText1.text}",
                expectedMessageText, errorPage.errorText1.text)
    }

    @Then("^there should be a button to try again$")
    fun thenThereShouldBeAButtonToTryAgain() {
        errorPage.assertHasButton("Try again")
    }

    @Then("^I see appropriate information message when there is a error retrieving data$")
    fun thenISeeAppropriateInformationMessageWhenThereIsAErrorRetrievingData() {
        val expectedHeader = "There's been a problem loading this page"
        val expectedBody = "Try again later. If the problem continues and you need to book an appointment now, " +
                "contact your GP surgery directly. For urgent medical advice, call 111."
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                expectedHeader, errorPage.heading.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.text}",
                expectedBody, errorPage.errorText1.text)
    }

    @Then("^I see appropriate information message when appointments are disabled$")
    fun thenISeeAppropriateInformationMessageWhenAppointmentsAreDisabled() {
        val expectedHeader = "You are not currently able to book appointments online."
        val expectedBody = "Contact your GP surgery for more information. For urgent medical help, call 111."
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                expectedHeader, errorPage.heading.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.text}",
                expectedBody, errorPage.errorText1.text)
    }

    @Then("^there should not be an option to try again$")
    fun thenThereShouldNotBeAnOptionToTryAgain() {
        errorPage.assertNoButton("Try again")
    }

    @Then("^a message is displayed indicating that the slot has already been taken$")
    fun aMessageIsDisplayedInformingTheSlotHasAlreadyBeenTaken() {
        val expectedHeader = "This slot is no longer available"
        val expectedMsg = "Please select a different time."

        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                expectedHeader, errorPage.heading.text)
        assertEquals("expected error text $expectedMsg but found ${errorPage.errorText1.text}",
                expectedMsg, errorPage.errorText1.text)
    }

    @Then("^I see a timeout on the appointment booking page$")
    fun iSeeATimeOutOnTheAppointmentBookingPage() {
        errorPage.assertHeaderText("There's been a problem loading this page")
        errorPage.assertHasButton("Try again")
    }
}
