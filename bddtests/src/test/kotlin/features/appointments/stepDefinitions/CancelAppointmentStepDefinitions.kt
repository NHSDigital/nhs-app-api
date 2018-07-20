package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.CancelAppointmentSteps
import net.thucydides.core.annotations.Steps
import cucumber.api.java.en.Given

class CancelAppointmentStepDefinitions {

    @Steps
    lateinit var cancelAppointmentSteps: CancelAppointmentSteps

    @Given("^I am on the (.*) appointment cancellation screen$")
    fun iAmOnTheCancellationScreen(gpService: String) {
        cancelAppointmentSteps.progressToAppointmentCancellationScreen(gpService)
    }

    @Given("^I select a cancellation reason of (.*)$")
    fun iSelectACancellationReason(reason: String) {
        cancelAppointmentSteps.selectReason(reason)
    }

    @Then("^I will be on the \"Cancellation reason\" screen$")
    fun iWillBeOnTheCancellationScreen() {
        cancelAppointmentSteps.verifyWeAreOnTheCancelAppointmentScreen()
    }

    @Then("^I am presented with the appointment details$")
    fun iAmPresentedWithTheSelectedAppointmentDetails() {
        cancelAppointmentSteps.verifyTheCorrectAppointmentDetailsAreDisplayed()
    }

    @Then("^there is a cancellation reasons drop-down$")
    fun thereIsACancellationReasonsDropDownWithTheAppropriateReasons() {
        cancelAppointmentSteps.verifyTheDropDownMenuLabel()
        cancelAppointmentSteps.selectReason("Select reason")
    }

    @Then("^I will receive a cancellation validation error$")
    fun iWillReceiveACancellationValidationError() {
        cancelAppointmentSteps.verifyTheValidationErrorSummary()
        cancelAppointmentSteps.verifyTheInlineReasonValidationError()
    }
}
