package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.im1Appointments.steps.CancelAppointmentSteps
import mockingFacade.appointments.MyAppointmentsFacade
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps

class CancelAppointmentStepDefinitions {

    @Steps
    lateinit var cancelAppointmentSteps: CancelAppointmentSteps

    @Given("^(.*) is available to cancel a previously booked appointment before cutoff time because (.*)$")
    fun gpSystemIsAvailableToCancelAnAppointmentForReason(gpSystem: String, reason: String) {
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, gpSystem) { cancelRequest ->
            cancelRequest.respondWithSuccess()
        }
    }

    @Given("^VISION is available to cancel a previously booked appointment before cutoff time, " +
            "with only one available reason$")
    fun visionIsAvailableToCancelWithOneReason() {
        cancelAppointmentSteps.mockCancellationRequestStubForReason(gpSystem = "VISION") { cancelRequest ->
            cancelRequest.respondWithSuccess()
        }
    }

    @Given("^I select a cancellation reason of (.*)$")
    fun iSelectACancellationReason(reason: String) {
        cancelAppointmentSteps.selectReason(reason)
    }

    @Given("^I select the cancellation reason$")
    fun iSelectTheCancellationReason() {
        iSelectACancellationReason(
                Serenity.sessionVariableCalled<MyAppointmentsFacade>(MyAppointmentsFacade::class)
                        .myAppointments!!
                        .cancellationReasons!!
                        .first()
                        .displayName
        )
    }

    @Given("^(.*) is unavailable to cancel a previously booked appointment because (.*)$")
    fun gpSystemIsUnavailableToCancelAnAppointmentForReason(gpSystem: String, reason: String) {
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, gpSystem) { cancelRequest ->
            cancelRequest.responseWithExceptionWhenServiceUnavailable()
        }
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

    @Then("^cancellation reasons drop-down is hidden$")
    fun cancellationReasonsDropDownIsHidden() {
        cancelAppointmentSteps.verifyTheDropDownMenuLabelIsNotVisible()
    }

    @Then("^I will receive a cancellation validation error$")
    fun iWillReceiveACancellationValidationError() {
        cancelAppointmentSteps.verifyTheValidationErrorSummary()
        cancelAppointmentSteps.verifyTheInlineReasonValidationError()
    }
}
