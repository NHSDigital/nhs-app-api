package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.appointments.DeleteAppointmentResponseModel
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import mocking.emis.appointments.CancelAppointmentRequest
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

    @Given("^the Emis is available to cancel an appointment$")
    fun theEmisIsAvailableToCancelAnAppointment() {
        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie("EMIS")
        retrieveCancellationReasons()
        mockCancellationRequestStubForReason(cancellationReasons.first().displayName)
    }

    @Given("^the Emis is available to cancel an appointment for (.*)$")
    fun theEmisIsAvailableToCancelAnAppointmentForReason(reason: String) {
        commonSteps.givenIHaveLoggedIntoXAndHaveAValidSessionCookie("EMIS")
        mockCancellationRequestStubForReason(reason)
    }

    private fun mockCancellationRequestStubForReason(reason: String) {
        mockingClient.forEmis {
            cancelAppointmentRequest(patient, CancelAppointmentRequest(patient.userPatientLinkToken, SLOT_ID, reason))
                    .respondWithSuccess(DeleteAppointmentResponseModel(true))
        }
    }

    @When("^I send a cancellation request to the API with a valid cancellation reason$")
    @Throws(Exception::class)
    fun i_send_a_cancellation_request_to_the_API_with_a_valid_cancellation_reason() {
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID.toString(), cancellationReasons.first().id)

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
