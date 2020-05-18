package features.authentication.stepDefinitions

import config.Config
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.MockingClient
import utils.SerenityHelpers
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.LinkedProfilesSerenityHelpers
import utils.getOrNull
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentBookRequest
import worker.models.patient.Im1ConnectionResponse

class EndpointStepDefinitions  {

    private val mockingClient = MockingClient.instance

    private val _pfsResponse = "PFSResponse"
    private val _pfsException = "PFSException"
    private val _im1HttpException = "IM1HttpException"
    private val _im1ConnectionResponse = "Im1ConnectionResponse"

    @Given("^I have an IM1 request and a Patient Facing Request$")
    fun iHaveValidPatientDataToRegisterNewAccount() {
        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, supplier)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        AppointmentsBookingFactory.getForSupplier(supplier)
                .defaultAppointmentBookingSetupWithResult { builder -> builder.respondWithSuccess() }
    }

    @Given("^I target the IM1 endpoint$")
    fun iTargetTheIM1Endpoint() {
        val config = Config.createConfig()
        config.apiBackendUrl = config.cidBackendUrl
        targetPort(config)
    }

    @Given("^I target the Patient Facing Services endpoint$")
    fun iTargetThePatientFacingServicesEndpoint() {
        val config = Config.createConfig()
        config.cidBackendUrl = config.apiBackendUrl
        targetPort(config)
    }

    private fun targetPort(config: Config) {
        val workerClient = WorkerClient(config)
        Serenity.setSessionVariable(WorkerClient::class).to(workerClient)
    }

    @Then("^I receive a response from the IM1 request$")
    fun iReceiveAResponseFromTheIM1Request() {
        val message = "Expected Positive Outcome for IM1"
        retrieveIM1()
        val response = Serenity.sessionVariableCalled<Im1ConnectionResponse>(_im1ConnectionResponse)
        val exception = Serenity.sessionVariableCalled<NhsoHttpException>(_im1HttpException)
        val exceptionMessage = if (exception != null) {
            "Exception with Status code: ${exception.statusCode}"
        } else {
            ""
        }
        Assert.assertNotNull("$message, but did not find a response $exceptionMessage",
                response)
    }

    @Then("^I receive a Not Found response from the IM1 request$")
    fun iReceiveANotFoundResponseFromTheIM1Request() {
        val message = "Expected Not Found Outcome for IM1"
        retrieveIM1()
        assertExceptionStatusCode(_im1HttpException, HttpStatus.SC_NOT_FOUND, message)
    }

    @Then("^I receive a response from the Patient Facing request$")
    fun iReceiveAResponseFromThePatientFacingRequest() {
        retrievePFS()
        val message = "Expected Unauthorised Outcome for PFS"
        assertExceptionStatusCode(_pfsException, HttpStatus.SC_UNAUTHORIZED, message)
    }

    @Then("^I receive a Not Found response from the Patient Facing request$")
    fun iReceiveANotFoundResponseFromThePatientFacingRequest() {
        retrievePFS()
        val message = "Expected Not Found Outcome for PFS"
        assertExceptionStatusCode(_pfsException, HttpStatus.SC_NOT_FOUND, message)
    }

    private fun assertExceptionStatusCode(exceptionKey: String, expectedStatusCode: Int, message: String) {
        val exception = Serenity.sessionVariableCalled<NhsoHttpException>(exceptionKey)
        Assert.assertNotNull("$message, but did not find exception", exception)
        Assert.assertEquals("$message, but StatusCode was not as expected",
                expectedStatusCode,
                exception.statusCode)
    }

    private fun retrieveIM1() {
        val patient = SerenityHelpers.getPatient()
        submitRequest(_im1ConnectionResponse, _im1HttpException) {
            worker -> worker.authentication.getIm1Connection(patient.connectionToken, patient.odsCode)
        }
    }

    private fun retrievePFS() {
        val appointmentToBook = Serenity.sessionVariableCalled<AppointmentBookRequest>(
                AppointmentsBookingFactory.appointmentToBookKey)
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()

        submitRequest(_pfsResponse, _pfsException) {
            worker -> worker.appointments.postAppointment(patientId, appointmentToBook)
        }
    }

    private fun <T> submitRequest(responseKey: String,
                                  exceptionKey: String,
                                  request: (WorkerClient) -> T) {
        val workerClient = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
        val result = request.invoke(workerClient)
        Serenity.setSessionVariable(responseKey).to(result)
        val errorResponse = SerenityHelpers.getHttpException()
        Serenity.setSessionVariable(exceptionKey).to(errorResponse)
    }
}
