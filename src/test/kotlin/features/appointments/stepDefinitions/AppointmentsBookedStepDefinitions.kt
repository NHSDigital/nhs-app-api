package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.AppointmentsBookedSteps
import net.thucydides.core.annotations.Steps

class AppointmentsBookedStepDefinitions {

    @Steps
    lateinit var appointmentsBookedSteps: AppointmentsBookedSteps

    @Then("^booking request is successfully made with valid details$")
    @Throws(Exception::class)
    fun bookingRequestIsMade() {
        appointmentsBookedSteps.checkBookingWasRequested()
    }

    @Then("^Appointment Booking confirmation screen is displayed$")
    @Throws(Exception::class)
    fun appointmentBookingConfirmationScreenIsDisplayed() {
        appointmentsBookedSteps.checkSuccessMessage()
    }
}
