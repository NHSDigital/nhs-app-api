package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AppointmentsSteps
import net.thucydides.core.annotations.Steps

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var appointmentsSteps: AppointmentsSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Given("^I have selected an appointment slot to book$")
    @Throws(Exception::class)
    fun i_have_selected_an_appointment_slot_to_book() {
        appointmentsSteps.selectSlot()

        appointmentsSteps.clickOnBookAppointmentButton()    }

    @When("^I click the 'Confirm and book appointment' button$")
    @Throws(Exception::class)
    fun i_click_the_Confirm_and_book_appointment_button() {
        appointmentsConfirmationSteps.clickOnConfirmAndBookAppointmentButton()
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

    @Given("^GP system doesn't respond a timely fashion for booking an appointment$")
    @Throws(Exception::class)
    fun gp_system_doesn_t_respond_a_timely_fashion_for_booking_an_appointment() {
        appointmentsConfirmationSteps.mockEmisSuccessResponseDelayedBy(30)
    }

    @Then("^I see appropriate information message when there is an error on appointment confirmation page$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_when_there_is_an_error_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }
}
