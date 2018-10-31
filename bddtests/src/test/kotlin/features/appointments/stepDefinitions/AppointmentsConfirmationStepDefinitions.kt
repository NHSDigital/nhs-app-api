package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsBookingFactory.Companion.SymptomsToEnter
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentDateKey
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentTimeKey
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AvailableAppointmentFilterSteps
import features.appointments.steps.AvailableAppointmentsSteps
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.junit.Assert

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var availableAppointmentsFilterSteps: AvailableAppointmentFilterSteps
    @Steps
    lateinit var availableAppointmentsSteps: AvailableAppointmentsSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Given("^I have selected an appointment slot to book$")
    fun i_have_selected_an_appointment_slot_to_book() {
        availableAppointmentsFilterSteps.selectOptionsToRevealSlots()
        val date = sessionVariableCalled<String>(TargetAppointmentDateKey)
        val time = sessionVariableCalled<String>(TargetAppointmentTimeKey)
        availableAppointmentsSteps.availableAppointmentsPage.selectSlot(date, time)
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

    @When("^I click the error page back button$")
    fun i_click_the_error_page_back_button() {
        appointmentsConfirmationSteps.clickErrorPageBackButton()
    }

    @When("^I enter symptoms of (\\d+) characters$")
    fun i_enter_symptoms_of_character(length: Int) {
        val symptoms: String = getSymptomsOfLength(length)
        appointmentsConfirmationSteps.describeSymptoms(symptoms)
    }

    @When("^I enter symptoms$")
    fun i_enter_symptoms() {
        val symptoms = Serenity.sessionVariableCalled<String>(SymptomsToEnter)
        Assert.assertNotNull("Expected symptoms to be set, incorrect test setup", symptoms)
        appointmentsConfirmationSteps.describeSymptoms(symptoms)
    }

    @When("^I paste symptoms of (\\d+) characters$")
    fun i_paste_symptoms_of_characters(length: Int) {
        val symptoms: String = getSymptomsOfLength(length)
        appointmentsConfirmationSteps.pasteSymptoms(symptoms)
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

    @Then("^I don't see option to type in booking reason$")
    fun i_don_t_see_option_to_type_in_booking_reason() {
        appointmentsConfirmationSteps.appointmentsConfirmation.symptomsFormDiv.assertElementNotPresent()
    }

    @Then("^a message is displayed indicating that user has reached maximum appointment limit$")
    fun aMessageIsDisplayedInformingTheAppointmentLimitReached() {
        appointmentsConfirmationSteps.verifyThatAppointmentLimitReachedErrorDisplayed()
    }

    private fun getSymptomsOfLength(length: Int): String {
        val symptoms = Serenity.sessionVariableCalled<String>(SymptomsToEnter)
        Assert.assertNotNull("Expected symptoms to be set, incorrect test setup", symptoms)
        Assert.assertEquals("Expected number of characters in symptoms, incorrect test setup", length, symptoms.length)
        return symptoms
    }
}
