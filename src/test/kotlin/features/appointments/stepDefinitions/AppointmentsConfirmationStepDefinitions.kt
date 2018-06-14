package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AppointmentsBookingSteps
import net.thucydides.core.annotations.Steps

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var appointmentsBookingSteps: AppointmentsBookingSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Given("^I have selected an appointment slot to book$")
    @Throws(Exception::class)
    fun i_have_selected_an_appointment_slot_to_book() {
        appointmentsBookingSteps.selectSlot()

        appointmentsBookingSteps.clickOnBookAppointmentButton()    }

    @When("^I click the 'Confirm and book appointment' button$")
    @Throws(Exception::class)
    fun i_click_the_Confirm_and_book_appointment_button() {
        appointmentsConfirmationSteps.clickOnConfirmAndBookAppointmentButton()
    }

    @When("^I click the button to go back to my appointments$")
    @Throws(Exception::class)
    fun i_click_the_button_to_go_back_to_my_appointments() {
        appointmentsConfirmationSteps.clickOnButton("Back to my appointments")
    }

    @Then("^an error is displayed that \"Describe your symptoms\" is mandatory$")
    @Throws(Exception::class)
    fun an_error_is_displayed_that_is_mandatory() {
        appointmentsConfirmationSteps.checkValidationErrorMessage()
    }

    @Given("^I enter symptoms of (\\d+) characters$")
    @Throws(Exception::class)
    fun i_enter_symptoms_of_character(n: Int) {
        appointmentsConfirmationSteps.describeSymptoms("x".repeat(n))
    }

    @When("^I enter symptoms$")
    @Throws(Exception::class)
    fun i_enter_symptoms() {
        appointmentsConfirmationSteps.describeSymptoms("Eye problems. Double vision.")
    }

    @When("^I paste symptoms of (\\d+) characters$")
    @Throws(Exception::class)
    fun i_paste_symptoms_of_characters(length: Int) {
        appointmentsConfirmationSteps.pasteSymptoms(length)
    }

    @Then("^only the first (\\d+) characters will be displayed$")
    @Throws(Exception::class)
    fun only_the_first_characters_will_be_displayed(length: Int) {
        appointmentsConfirmationSteps.checkSymptomsLength(length)
    }

    @Given("^GP system doesn't respond a timely fashion when booking an appointment$")
    @Throws(Exception::class)
    fun gp_system_doesn_t_respond_a_timely_fashion_when_booking_an_appointment() {
        appointmentsConfirmationSteps.mockEmisSuccessResponseDelayedBy(30)
    }

    @Given("^GP system is unavailable when booking an appointment$")
    @Throws(Exception::class)
    fun gp_system_is_unavailable_when_booking_an_appointment() {

        appointmentsConfirmationSteps.mockEmisUnavailableResponse()
    }

    @Given("^the appointment slot has already been booked by somebody else$")
    @Throws(Exception::class)
    fun the_appointment_slot_has_already_been_booked_by_somebody_else() {
        appointmentsConfirmationSteps.mockEmisConflictesponse()
    }

    @Then("^I see appropriate information message when there is an error on appointment confirmation page$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_when_there_is_an_error_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message after 10 seconds when it times-out on appointment confirmation page$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_after_seconds_when_it_times_out_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message when there is an error sending data on appointment confirmation page$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_when_there_is_an_error_sending_data_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkErrorSendingMessage()
    }

    @Then("^I see appropriate information message when appointment has already been booked$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_when_appointment_has_already_been_booked() {
        appointmentsConfirmationSteps.checkErrorSendingMessage()
    }

    @Then("^there should be a button to go back to my appointments$")
    @Throws(Exception::class)
    fun there_should_be_a_button_to_go_back_to_my_appointments() {
        appointmentsConfirmationSteps.checkIfButtonIsVisible("Back to my appointments")
    }
}
