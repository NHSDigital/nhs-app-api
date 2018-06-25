package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.CancelAppointmentSteps
import net.thucydides.core.annotations.Steps
import cucumber.api.java.en.Given

class CancelAppointmentStepDefinitions {

    @Steps
    lateinit var cancelAppointmentSteps: CancelAppointmentSteps

    @Given("^I am on the appointment cancellation screen$")
    fun iAmOnTheCancellationScreen() {
        cancelAppointmentSteps.progressToAppointmentCancellationScreen()
    }

    @Then("^I will be on the \"Cancellation reason\" screen$")
    fun iWillBeOnTheCancellationScreen() {
        cancelAppointmentSteps.verifyWeAreOnTheCancelAppointmentScreen()
    }

    @Then("^I am presented with the appointment details$")
    fun iAmPresentedWithTheSelectedAppointmentDetails() {
        cancelAppointmentSteps.verifyTheCorrectAppointmentDetailsAreDisplayed()
    }

    @Then("^there is a cancellation reasons drop-down with the appropriate reasons$")
    fun thereIsACancellationReasonsDropDownWithTheAppropriateReasons(reasons: List<String>) {
        cancelAppointmentSteps.verifyTheDropDownMenuLabel()
        cancelAppointmentSteps.verifyTheReasonIsAvailable("Select reason")
        reasons.forEach {
            cancelAppointmentSteps.verifyTheReasonIsAvailable(it)
        }
    }
}
