package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.AppointmentsSteps
import net.thucydides.core.annotations.Steps

class AppointmentsStepDefinitions {

    @Steps
    lateinit var appointmentsSteps: AppointmentsSteps

    @Then("^booking request is successfully made with valid details$")
    @Throws(Exception::class)
    fun bookingRequestIsMade() {
        appointmentsSteps.checkBookingWasRequested()
    }

    @Then("^Appointment Booking confirmation screen is displayed$")
    @Throws(Exception::class)
    fun appointmentBookingConfirmationScreenIsDisplayed() {
        appointmentsSteps.checkSuccessMessage()
    }

    @Then("^I will be on the My appointments screen$")
    @Throws(Exception::class)
    fun i_will_be_on_the_my_appointments_screen() {
        appointmentsSteps.checkHeader("My appointments")
    }
}
