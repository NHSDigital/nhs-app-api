package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.im1Appointments.steps.AppointmentGuidanceSteps
import net.thucydides.core.annotations.Steps

class AppointmentGuidanceStepDefinitions {

    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps

    @Then("^I am given guidance as to my options before booking an appointment$")
    @Throws(Exception::class)
    fun thenIAmGivenGuidanceAsToMyOptionsBeforeBookingAnAppointment() {
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkTheContentIsCorrect()
    }
}
