package features.im1Appointments.stepDefinitions

import io.cucumber.java.en.Then
import pages.appointments.AppointmentBookingSuccessPage
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail

open class AppointmentBookingSuccessStepDefinitions {

    private lateinit var appointmentBookingSuccessPage: AppointmentBookingSuccessPage

    @Then("^The appointment booking success page is shown$")
    fun theBookingSuccessPageIsShown() {
        appointmentBookingSuccessPage
                .isLoaded(LinkedProfilesSerenityHelpers.PROXY_DISPLAY_NAME.getOrFail())

        appointmentBookingSuccessPage.appointmentDetailsNotShown()
    }
}
