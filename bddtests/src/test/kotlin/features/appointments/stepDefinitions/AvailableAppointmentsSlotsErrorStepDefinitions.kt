package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.MockingClient
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.appointments.AvailableAppointmentsPage


class AvailableAppointmentsSlotsErrorStepDefinitions {

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance

    @When("^I click try again button on appointment page$")
    fun i_click_try_again_button_on_appointment_page() {
        errorPage.waitForSpinnerToDisappear(11) // 1 second more than timeout
        errorPage.clickOnButtonContainingText("Try again")
    }

    @Then("^I see appropriate information message for time-outs$")
    fun iSeeAppropriateInformationMessageAfterSecondsWhenItTimesOut() {
        val expectedHeader = "There's been a problem loading this page"
        val expectedMessageText = "Try again now. If the problem continues and you need to book an appointment now, " +
                "contact your GP surgery directly. For urgent medical advice, call 111."
        errorPage.waitForSpinnerToDisappear(70)
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        errorPage.subHeading.assertElementNotPresent()
        assertEquals("expected error text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedMessageText, errorPage.errorText1.element.text)
    }

    @Then("^there should be a button to try again$")
    fun there_should_be_a_button_to_try_again() {
        errorPage.assertHasButton("Try again")
    }

    @Then("^I see appropriate information message when there is a error retrieving data$")
    fun i_see_appropriate_information_message_when_there_is_a_error_retrieving_data() {
        val expectedHeader = "There's been a problem loading this page"
        val expectedBody = "Try again later. If the problem continues and you need to book an appointment now, " +
                "contact your GP surgery directly. For urgent medical advice, call 111."
        errorPage.waitForSpinnerToDisappear()
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.element.text}",
                expectedBody, errorPage.errorText1.element.text)
    }

    @Then("^I see appropriate information message when appointments are disabled$")
    fun i_see_appropriate_information_message_when_appointments_are_disabled() {
        val expectedHeader = "You are not currently able to book appointments online"
        val expectedBody = "Contact your GP surgery for more information. For urgent medical help, call 111."
        errorPage.waitForSpinnerToDisappear()
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.element.text}",
                expectedBody, errorPage.errorText1.element.text)
    }

    @Then("^there should not be an option to try again$")
    fun there_should_not_be_an_option_to_try_again() {
        errorPage.assertNoButton("Try again")
    }

    @Then("^a message is displayed indicating that the slot has already been taken$")
    fun aMessageIsDisplayedInformingTheSlotHasAlreadyBeenTaken() {
        val expectedHeader = "This slot is no longer available"
        val expectedMsg = "Please select a different time."

        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedMsg but found ${errorPage.errorText1.element.text}",
                expectedMsg, errorPage.errorText1.element.text)
    }

    @Then("^I see a timeout on the appointment booking page$")
    fun iSeeATimeOutOnTheAppointmentBookingPage() {
        availableAppointmentsPage.waitForSpinnerToDisappear(70)
        errorPage.assertHasButton("Try again")
    }
}
