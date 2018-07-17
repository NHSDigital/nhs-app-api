package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.stepDefinitions.factories.AppointmentsCancellingFactory
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus.SC_NO_CONTENT
import org.junit.Assert
import worker.models.appointments.GenericResponseObject
import java.time.LocalDateTime

class AppointmentsCancellingStepDefinitionsBackend {

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    private val commonSteps : CommonSteps = CommonSteps()
    private var cancellationReasons: ArrayList<GenericResponseObject> = arrayListOf()

    private val SLOT_ID = 1;
    val HTTP_EXCEPTION = "HttpException"
    val HTTP_RESPONSE = "HttpResponse"

    @Given("^(.*) is available to cancel an appointment$")
    fun gpSystemIsAvailableToCancelAnAppointment(gpSystem: String) {
        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem)
        var reason = ""
        retrieveCancellationReasons()
        if (cancellationReasons.size > 0) {
            reason = cancellationReasons.first().displayName
        }

        mockCancellationRequestStubForReason(reason, gpSystem)
    }

    @Given("^(.*) is available to cancel an appointment for (.*)$")
    fun gpSystemIsAvailableToCancelAnAppointmentForReason(gpSystem: String, reason: String) {
        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem)
        mockCancellationRequestStubForReason(reason, gpSystem)
    }

    private fun mockCancellationRequestStubForReason(reason: String, gpSystem: String) {
        var factory = AppointmentsCancellingFactory.getForSupplier(gpSystem)
        var patient = factory.patient
        var request = factory.defaultRequest(patient, SLOT_ID, reason)

        factory.setupRequestAndResponse(request) { cancelAppointmentRequest(patient, request).respondWithSuccess() }
    }

    @When("^I send a cancellation request to the API with a valid cancellation reason$")
    @Throws(Exception::class)
    fun i_send_a_cancellation_request_to_the_API_with_a_valid_cancellation_reason() {
        var id = ""
        if (cancellationReasons.size > 0) {
            id = cancellationReasons.first().id
        }
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID.toString(), id)

        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .deleteAppointment(body, null)

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
                    .deleteAppointment(body, null)

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
                    .deleteAppointment(body, null)

            Serenity.setSessionVariable(HTTP_RESPONSE).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I will receive a successful response$")
    @Throws(Exception::class)
    fun i_will_receive_a_successful_response() {
        val response = Serenity.sessionVariableCalled<HttpResponse>(HTTP_RESPONSE)
        Assert.assertTrue(response.statusLine.statusCode == SC_NO_CONTENT)
    }

    private fun retrieveCancellationReasons()
    {
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .getMyAppointments(LocalDateTime.now().toString())

        cancellationReasons = result.cancellationReasons
    }
}
