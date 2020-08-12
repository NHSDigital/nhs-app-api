package features.im1Appointments.stepDefinitions

import io.cucumber.java.en.Then
import pages.appointments.AppointmentCancellationSuccessPage
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail

open class AppointmentCancellationSuccessStepDefinitions {

    private lateinit var appointmentCancellationSuccessPage: AppointmentCancellationSuccessPage

    @Then("^The appointment cancellation success page is shown$")
    fun theCancellationSuccessPageIsShown() {
        appointmentCancellationSuccessPage
                .isLoaded(LinkedProfilesSerenityHelpers.PROXY_DISPLAY_NAME.getOrFail())
    }
}
