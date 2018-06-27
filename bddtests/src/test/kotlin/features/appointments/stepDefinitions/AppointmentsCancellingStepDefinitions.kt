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

class AppointmentsCancellingStepDefinitions {

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    private val commonSteps : CommonSteps = CommonSteps()
    private var cancellationReasons: ArrayList<GenericResponseObject> = arrayListOf()

    private val SLOT_ID = "1";
    val HTTP_EXCEPTION = "HttpException"
    val HTTP_RESPONSE = "HttpResponse"

    @Given("^the Emis is available to cancel an appointment$")
    @Throws(Exception::class)
    fun the_emis_is_available_to_cancel_an_appointment() {
        commonSteps.givenIHaveLoggedInAndHaveAValidSessionCookie()
        retrieveCancellationReasons()

        mockingClient.forEmis {
            cancelAppointmentRequest(patient, CancelAppointmentRequest(patient.userPatientLinkToken, SLOT_ID, cancellationReasons.first().displayName))
                    .respondWithSuccess(DeleteAppointmentResponseModel(true))
        }
    }

    @When("^I send a cancellation request to the API with a valid cancellation reason$")
    @Throws(Exception::class)
    fun i_send_a_cancellation_request_to_the_API_with_a_valid_cancellation_reason() {
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID, cancellationReasons.first().id)

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
        val body = worker.models.appointments.CancelAppointmentRequest(SLOT_ID, "NOT_EXISTING_REASON_ID")

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
