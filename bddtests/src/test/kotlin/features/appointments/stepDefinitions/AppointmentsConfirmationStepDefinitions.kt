package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.stepDefinitions.factories.AppointmentsBookingFactory
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AvailableAppointmentsSteps
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var availableAppointmentsSteps: AvailableAppointmentsSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Given("^I have selected an appointment slot to book$")
    fun i_have_selected_an_appointment_slot_to_book() {
        availableAppointmentsSteps.selectOptionsToRevealSlots()
        var date = Serenity.sessionVariableCalled<String>(AppointmentsBookingFactory.TargetAppointmentDateKey)
        var time = Serenity.sessionVariableCalled<String>(AppointmentsBookingFactory.TargetAppointmentTimeKey)

        availableAppointmentsSteps.selectSlot(date, time)
        availableAppointmentsSteps.clickOnBookAppointmentButton()
    }

    @When("^I click the 'Confirm and book appointment' button$")
    fun iClickTheConfirmAndBookAppointmentButton() {
        appointmentsConfirmationSteps.clickOnConfirmAndBookAppointmentButton()
    }

    @When("^I choose to change the appointment slot$")
    fun iChooseToChangeTheAppointmentSlot() {
        appointmentsConfirmationSteps.appointmentsConfirmation.clickOnButtonContainingText("Change this appointment")
    }

    @When("^I click the button to go back to my appointments$")
    fun i_click_the_button_to_go_back_to_my_appointments() {
        appointmentsConfirmationSteps.goBackToMyAppointments()
    }

    @When("^I enter symptoms of (\\d+) characters$")
    fun i_enter_symptoms_of_character(n: Int) {
        appointmentsConfirmationSteps.describeSymptoms("x".repeat(n))
    }

    @When("^I enter symptoms$")
    fun i_enter_symptoms() {
        appointmentsConfirmationSteps.describeSymptoms("Eye problems. Double vision.")
    }

    @When("^I paste symptoms of (\\d+) characters$")
    fun i_paste_symptoms_of_characters(length: Int) {
        appointmentsConfirmationSteps.pasteSymptoms(length)
    }

    @Then("^only the first (\\d+) characters will be displayed$")
    fun only_the_first_characters_will_be_displayed(length: Int) {
        appointmentsConfirmationSteps.checkSymptomsLength(length)
    }

    @Then("^I see appropriate information message when there is an error on appointment confirmation page$")
    fun i_see_appropriate_information_message_when_there_is_an_error_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message after 10 seconds when it times-out on appointment confirmation page$")
    fun i_see_appropriate_information_message_after_seconds_when_it_times_out_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message when there is an error sending data on appointment confirmation page$")
    fun i_see_appropriate_information_message_when_there_is_an_error_sending_data_on_appointment_confirmation_page() {
        appointmentsConfirmationSteps.checkErrorSendingMessage()
    }

    @Then("^there should be a button to go back to my appointments$")
    fun there_should_be_a_button_to_go_back_to_my_appointments() {
        appointmentsConfirmationSteps.checkIfButtonIsVisible("Back to my appointments")
    }

    @Then("^an error is displayed that \"Describe your symptoms\" is mandatory$")
    fun an_error_is_displayed_that_is_mandatory() {
        appointmentsConfirmationSteps.checkValidationErrorMessage()
    }
}
