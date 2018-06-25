package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.AppointmentGuidanceSteps
import net.thucydides.core.annotations.Steps
import cucumber.api.java.en.When


class AppointmentGuidanceStepDefinitions {
    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps

    @Then("^I am given guidance as to my options before booking an appointment$")
    @Throws(Exception::class)
    fun i_am_given_guidance_as_to_my_options_before_booking_an_appointment() {
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkTheContentHeaderIsCorrect()
        appointmentGuidanceSteps.checkGuidanceItemsHeadersAreCorrect()
    }

    @When("^I select Appointment Guidance Page Check your symptoms button$")
    @Throws(Exception::class)
    fun i_select_Appointment_Guidance_Page_Check_your_symptoms_button() {
        appointmentGuidanceSteps.clickCheckSymptomsButton()
    }
}