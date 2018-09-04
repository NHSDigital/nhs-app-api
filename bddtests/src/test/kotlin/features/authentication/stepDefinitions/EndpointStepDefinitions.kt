package features.authentication.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.SerenityHelpers
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.session.UserSessionResponse


class EndpointStepDefinitions : AbstractSteps() {

    val IM1Request = "IM1Request"
    val PFSResponse = "PFSResponse"
    val PFSException="PFSException"
    val IM1HttpException = "IM1HttpException"
    val Im1ConnectionResponse ="Im1ConnectionResponse"

    @Given("^I have an IM1 request and a Patient Facing Request$")
    fun iHaveValidPatientDataToRegisterNewAccount() {
        val gpSystem = "EMIS"
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, gpSystem)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        val im1Request = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
        Serenity.setSessionVariable(IM1Request).to(im1Request)
    }

    @Given("^I target the IM1 endpoint$")
    fun iTargetTheIM1Endpoint(){
        val config = Config.createConfig()
        config.pfsBackendUrl = config.cidBackendUrl
        targetPort(config)
    }

    @Given("^I target the Patient Facing Services endpoint$")
    fun iTargetThePatientFacingServicesEndpoint(){
        val config = Config.createConfig()
        config.cidBackendUrl = config.pfsBackendUrl
        targetPort(config)
    }

    private fun targetPort(config:Config){
        workerClient = WorkerClient(config)
        Serenity.setSessionVariable(WorkerClient::class).to(workerClient)
    }

    @Then("^I receive a response from the IM1 request$")
    fun iReceiveAResponseFromTheIM1Request(){
        val message = "Expected Positive Outcome for IM1"
        retrieveIM1()
        val response = Serenity.sessionVariableCalled<Im1ConnectionResponse>(Im1ConnectionResponse)
        val exception = Serenity.sessionVariableCalled<NhsoHttpException>(IM1HttpException)
        var exceptionMessage = if(exception!=null){"Exception with Status code: ${exception.statusCode}"}else{""}
        Assert.assertNotNull("$message, but did not find a response $exceptionMessage",
                response)
    }

    @Then("^I receive a Not Found response from the IM1 request$")
    fun iReceiveANotFoundResponseFromTheIM1Request() {
        val message = "Expected Not Found Outcome for IM1"
        retrieveIM1()
        assertExceptionStatusCode(IM1HttpException, 404, message)
    }

    private fun retrieveIM1(){
        val patient = SerenityHelpers.getPatient()
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getIm1Connection(patient.connectionToken, patient.odsCode)
            Serenity.setSessionVariable(Im1ConnectionResponse).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(IM1HttpException).to(httpException)
        }
    }

    @Then("^I receive a response from the Patient Facing request$")
    fun iReceiveAResponseFromThePatientFacingRequest() {
        retrievePFS()
        val message = "Expected Bad Gateway Outcome for PFS"
        val response = Serenity.sessionVariableCalled<UserSessionResponse>(PFSResponse)
        if(response ==null){
            assertExceptionStatusCode(PFSException, 502, message )
        }
    }

    @Then("^I receive a Not Found response from the Patient Facing request$")
    fun iReceiveANotFoundResponseFromThePatientFacingRequest() {
        retrievePFS()
        val message = "Expected Not Found Outcome for PFS"
        assertExceptionStatusCode(PFSException, 404, message )
    }

    private fun assertExceptionStatusCode(exceptionKey: String, expectedStatusCode: Int, message: String){
        val exception = Serenity.sessionVariableCalled<NhsoHttpException>(exceptionKey)
        Assert.assertNotNull("$message, but did not find exception", exception)
        Assert.assertEquals("$message, but StatusCode was not as expected", expectedStatusCode, exception.statusCode)
    }

    private fun retrievePFS() {
        try {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .postSessionConnection(SerenityHelpers.getPatient().cidUserSession)
            Serenity.setSessionVariable(PFSResponse).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(PFSException).to(httpException)
        }
    }
}
