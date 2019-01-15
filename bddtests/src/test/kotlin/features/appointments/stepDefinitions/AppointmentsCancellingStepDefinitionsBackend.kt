package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.CancelAppointmentSteps
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.MockingClient
import mocking.stubs.StubbedEnvironment
import mocking.vision.appointments.CancelAppointmentBuilderVision
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus.SC_NO_CONTENT
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import java.time.Duration

class AppointmentsCancellingStepDefinitionsBackend {

    val mockingClient = MockingClient.instance

    private val commonSteps: CommonSteps = CommonSteps()
    private val cancelAppointmentSteps = CancelAppointmentSteps()

    @Given("^(.*) is available to cancel a previously booked appointment before cutoff time$")
    fun gpSystemIsAvailableToCancelAnAppointment(gpSystem: String) {

        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem)
        var reason = ""
        if (gpSystem == "EMIS") {
            reason = "No longer required"
        }

        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, gpSystem) { cancelRequest ->
            cancelRequest.respondWithSuccess()
        }
    }

    @Given("^(.*) will time out when trying to cancel a previously booked appointment")
    fun gpSystemIsAvailableToCancelAnAppointmentButWillTimeout(gpSystem: String) {

        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem)
        var reason = ""
        if (gpSystem == "EMIS") {
            reason = "No longer required"
        }
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, gpSystem) { cancelRequest ->
            cancelRequest.respondWithSuccess().delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    @Given("^as a VISION user I want to cancel an appointment booked by someone else$")
    fun appointmentToBeCancelledIsBookedBySomeoneElseForVision() {
        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie("VISION")
        cancelAppointmentSteps.mockCancellationRequestStubForReason("", "VISION") { cancelRequest ->
            (cancelRequest as CancelAppointmentBuilderVision)
                    .respondWithConflictException()
        }
    }

    @Given("^as a VISION user I want to cancel an appointment that doesn't exist$")
    fun appointmentToBeCancelledDoesNotExistForVision() {
        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie("VISION")
        cancelAppointmentSteps.mockCancellationRequestStubForReason("", "VISION") { cancelRequest ->
            (cancelRequest as CancelAppointmentBuilderVision)
                    .respondWithExceptionWhenNotAvailable()
        }
    }

    @Given("^(.*) returns corrupted response when trying to cancel a previously booked appointment")
    fun gpSystemIsAvailableToCancelAnAppointmentButWillReturnCorruptedResponse(gpSystem: String) {

        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem)
        var reason = ""
        if (gpSystem == "EMIS") {
            reason = "No longer required"
        }
        cancelAppointmentSteps.mockCancellationRequestStubForReason(reason, gpSystem) { cancelRequest ->
            cancelRequest.respondWithCorrupted()
        }
    }

    @When("^I send a cancellation request to the API with a valid cancellation reason$")
    @Throws(Exception::class)
    fun whenISendACancellationRequestToTheAPIWithAValidCancellationReason() {
        var id = ""
        val reasons = cancelAppointmentSteps.retrieveCancellationReasons()
        if (reasons.any()) {
            id = reasons.first().id

        }
        val body = worker.models.appointments.CancelAppointmentRequest(
                cancelAppointmentSteps.retrieveSlotIdOfAppointmentToCancel().toString(),
                id
        )

        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.deleteAppointment(body)

            SerenityHelpers.setHttpResponse(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I send a cancellation request to the API with an invalid cancellation reason$")
    @Throws(Exception::class)
    fun whenISendACancellationRequestToTheAPIWithAnInvalidCancellationReason() {
        val body = worker.models.appointments.CancelAppointmentRequest(
                cancelAppointmentSteps.retrieveSlotIdOfAppointmentToCancel().toString(),
                "NOT_EXISTING_REASON_ID"
        )

        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.deleteAppointment(body)
            SerenityHelpers.setHttpResponse(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I will receive a successful response$")
    @Throws(Exception::class)
    fun thenIWillReceiveASuccessfulResponse() {
        val response = SerenityHelpers.getHttpResponse()
        Assert.assertNotNull("Expected Response", response)
        Assert.assertEquals("Expected statusCode", SC_NO_CONTENT, response!!.statusLine.statusCode )
    }
}
