package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Then
import pages.appointments.AppointmentCancellationSuccessPage
import utils.ProxySerenityHelpers

open class AppointmentCancellationSuccessStepDefinitions {

    private lateinit var appointmentCancellationSuccessPage: AppointmentCancellationSuccessPage

    @Then("^The appointment cancellation success page is shown$")
    fun theCancellationSuccessPageIsShown() {
        appointmentCancellationSuccessPage.isLoaded(ProxySerenityHelpers.getPatientOrProxy().firstName)
    }
}