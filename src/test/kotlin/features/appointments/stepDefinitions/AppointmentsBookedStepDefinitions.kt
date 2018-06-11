package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.AppointmentsBookedSteps
import mocking.defaults.MockDefaults
import mocking.MockingClient
import net.thucydides.core.annotations.Steps

class AppointmentsBookedStepDefinitions {

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    @Steps
    lateinit var appointmentsBookedSteps: AppointmentsBookedSteps

    @Then("^Appointment Booking confirmation screen is displayed$")
    @Throws(Exception::class)
    fun appointment_Booking_confirmation_screen_is_displayed() {
        appointmentsBookedSteps.checkSuccessMessage()
    }
}
