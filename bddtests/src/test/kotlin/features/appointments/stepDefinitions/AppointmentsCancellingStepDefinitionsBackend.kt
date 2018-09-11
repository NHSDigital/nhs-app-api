package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsCancellingFactory
import features.appointments.factories.ViewAppointmentsFactory
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.MockingClient
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus.SC_NO_CONTENT
import org.junit.Assert.assertTrue
import worker.models.appointments.GenericResponseObject
import java.time.LocalDateTime

class AppointmentsCancellingStepDefinitionsBackend {

    val mockingClient = MockingClient.instance

    private val commonSteps: CommonSteps = CommonSteps()

    private val SLOT_ID = 1
    val HTTP_EXCEPTION = "HttpException"
    val HTTP_RESPONSE = "HttpResponse"

    @Given("^(.*) is available to cancel a previously booked appointment$")
    fun gpSystemIsAvailableToCancelAnAppointment(gpSystem: String) {

        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem)
        var reason = ""
        if (gpSystem == "EMIS") {
            reason = "No longer required"
        }
        mockCancellationRequestStubForReason(reason, gpSystem)
    }

    @Given("^(.*) is available to cancel a previously booked appointment because (.*)$")
    fun gpSystemIsAvailableToCancelAnAppointmentForReason(gpSystem: String, reason: String) {
        mockCancellationRequestStubForReason(reason, gpSystem)
    }

    private fun mockCancellationRequestStubForReason(reason: String, gpSystem: String) {

        val patient = Patient.getDefault(gpSystem)

        val viewAppointmentFactory = ViewAppointmentsFactory.getForSupplier(gpSystem)
        Serenity.setSessionVariable(Patient::class).to(patient)
        val response = viewAppointmentFactory.createUpcomingAppointments(patient)
        viewAppointmentFactory.setUpViewAppointmentsWithResult(gpSystem) { builder ->
            builder.respondWithSuccess(response)
        }

        val factory = AppointmentsCancellingFactory.getForSupplier(gpSystem)
        val request = factory.defaultRequest(patient, SLOT_ID, reason)

        factory.setupRequestAndResponse(request) { cancelAppointmentRequest(patient, request).respondWithSuccess() }
    }

    @When("^I send a cancellation request to the API with a valid cancellation reason$")
    @Throws(Exception::class)
    fun i_send_a_cancellation_request_to_the_API_with_a_valid_cancellation_reason() {
        var id = ""
        val reasons = retrieveCancellationReasons()
        if (reasons.any()) {
            id = reasons.first().id

        }
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID.toString(), id)

        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .deleteAppointment(body)

            Serenity.setSessionVariable(HTTP_RESPONSE).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I send a cancellation request to the API without a cancellation reason$")
    @Throws(Exception::class)
    fun i_send_a_cancellation_request_to_the_API_without_a_cancellation_reason() {
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID.toString(), "")

        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .deleteAppointment(body)

            Serenity.setSessionVariable(HTTP_RESPONSE).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I send a cancellation request to the API with an invalid cancellation reason$")
    @Throws(Exception::class)
    fun i_send_a_cancellation_request_to_the_API_with_an_invalid_cancellation_reason() {
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID.toString(), "NOT_EXISTING_REASON_ID")

        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .deleteAppointment(body)

            Serenity.setSessionVariable(HTTP_RESPONSE).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I will receive a successful response$")
    @Throws(Exception::class)
    fun i_will_receive_a_successful_response() {
        val response = Serenity.sessionVariableCalled<HttpResponse>(HTTP_RESPONSE)
        assertTrue(response.statusLine.statusCode == SC_NO_CONTENT)
    }

    private fun retrieveCancellationReasons(): ArrayList<GenericResponseObject> {
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .getMyAppointments(LocalDateTime.now().toString())

        return result.cancellationReasons
    }
}
