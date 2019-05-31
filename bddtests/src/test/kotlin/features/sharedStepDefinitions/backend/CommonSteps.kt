package features.sharedStepDefinitions.backend

import com.google.gson.GsonBuilder
import config.Config
import cucumber.api.java.Before
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import utils.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.tpp.models.Authenticate
import mocking.defaults.VisionMockDefaults
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.getCurrentSession
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.NhsoHttpException
import worker.NhsoHttpExceptionErrorBody
import worker.WorkerClient
import java.util.concurrent.TimeUnit

private const val ADDITIONAL_TIME_TO_DELAY = 10

class CommonSteps : AbstractSteps() {
    companion object {
        val GP_SYSTEM: String = "GP_SYSTEM"
    }

    @Before
    fun beforeEachScenario() {
        getCurrentSession().clear()

        mockingClient = MockingClient.instance
        workerClient = WorkerClient()
        mockingClient.clearWiremock()

        setSessionVariable(MockingClient::class).to(mockingClient)
        setSessionVariable(WorkerClient::class).to(workerClient)
    }

    @Given("^(EMIS|TPP|VISION) is not available$")
    fun givenXIsNotAvailable(gpSystem: String) {

        val patient = Patient.getDefault(gpSystem)
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis {
                    authentication.endUserSessionRequest()
                            .respondWithServiceUnavailable()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    authentication.authenticateRequest(Authenticate())
                            .respondWithServiceUnavailable()
                }
            }
            "VISION" -> {
                mockingClient.forVision {
                    authentication.getConfigurationRequest(
                            VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithServiceUnavailable()
                }
            }
        }
    }

    @Then("^I receive (?:a|an) \"(.*)\" error$")
    fun thenIReceiveAMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val errorResponse = SerenityHelpers.getHttpException()
        assertNotNull(
                "An exception was expected but was not returned within the expected time limit.",
                errorResponse
        )
        assertEquals("Incorrect status code returned. ", converted, errorResponse!!.statusCode)
    }

    @Then("the response contains an empty body$")
    fun theResponseBodyIsEmpty(){
        val errorResponse = SerenityHelpers.getHttpException()
        assertNotNull("Expected Response", errorResponse)
        assertEquals("Expected an empty body. ", "", errorResponse!!.body)
    }

    @Then("^I receive (?:a|an) \"(.*)\" success code")
    fun thenIReceiveASuccessMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val httpResponse = sessionVariableCalled<HttpResponse>("HttpResponse")
        assertEquals(converted, httpResponse.statusLine.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" error status code$")
    fun thenIReceiveAStatusCode(expectedStatusCode: Int) {
        val exception = sessionVariableCalled<NhsoHttpException>("HttpException")
        assertEquals(expectedStatusCode, exception.statusCode)
    }

    @Then("the Bad Gateway error response includes a retry option$")
    fun theOrganDonationBadGatewayErrorResponseDoesIncludeARetryOption(){
        val errorResponse = SerenityHelpers.getHttpException()
        val errorResponseBody = GsonBuilder().create()
                .fromJson<NhsoHttpExceptionErrorBody>(errorResponse?.body.toString(),
                        NhsoHttpExceptionErrorBody::class.java)

        Assert.assertNotNull("Expected Response", errorResponse)
        Assert.assertEquals("Expected errorCode", "1", errorResponseBody.errorCode)
        Assert.assertEquals("Expected statusCode", HttpStatus.SC_BAD_GATEWAY, errorResponse!!.statusCode)
    }

    @Then("the Bad Gateway error response does not include a retry option$")
    fun theOrganDonationBadGatewayErrorResponseDoesNotIncludeARetryOption(){
        val errorResponse = SerenityHelpers.getHttpException()
        val errorResponseBody = GsonBuilder().create()
                .fromJson<NhsoHttpExceptionErrorBody>(errorResponse?.body.toString(),
                        NhsoHttpExceptionErrorBody::class.java)

        Assert.assertNotNull("Expected Response", errorResponse)
        Assert.assertEquals("Expected errorCode", "0", errorResponseBody.errorCode)
        Assert.assertEquals("Expected statusCode", HttpStatus.SC_BAD_GATEWAY, errorResponse!!.statusCode)
    }

    @Then("the Internal Server Error response does not include a retry option$")
    fun theOrganDonationInternalServerErrorResponseDoesNotIncludeARetryOption(){
        val errorResponse = SerenityHelpers.getHttpException()
        Assert.assertNotNull("Expected Response", errorResponse)
        Assert.assertEquals("Expected Body", "", errorResponse?.body.toString())
        Assert.assertEquals("Expected statusCode", HttpStatus.SC_INTERNAL_SERVER_ERROR, errorResponse!!.statusCode)
    }

    private val _statusCodeMapping: HashMap<String, Int> = hashMapOf(
            "ok" to HttpStatus.SC_OK,
            "created" to HttpStatus.SC_CREATED,
            "bad gateway" to HttpStatus.SC_BAD_GATEWAY,
            "bad request" to HttpStatus.SC_BAD_REQUEST,
            "gateway timeout" to HttpStatus.SC_GATEWAY_TIMEOUT,
            "not found" to HttpStatus.SC_NOT_FOUND,
            "internal server error" to HttpStatus.SC_INTERNAL_SERVER_ERROR,
            "conflict" to HttpStatus.SC_CONFLICT,
            "forbidden" to HttpStatus.SC_FORBIDDEN,
            "service unavailable" to HttpStatus.SC_SERVICE_UNAVAILABLE,
            "unauthorized" to HttpStatus.SC_UNAUTHORIZED,
            "not implemented" to HttpStatus.SC_NOT_IMPLEMENTED
    )

    fun httpStatusCodeTransform(statusName: String): Int? {
        return _statusCodeMapping[statusName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $statusName")
    }

    @Given("^I have logged into (.*) and have a valid session cookie$")
    fun givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)

        Serenity.setSessionVariable(GP_SYSTEM).to(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)

        givenIHaveLoggedInAndHaveAValidSessionCookie()
    }

    @Given("^I have logged in and have a valid session cookie$")
    fun givenIHaveLoggedInAndHaveAValidSessionCookie() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.cidUserSession)
    }

    @And("I allow my session to expire")
    fun andIDelayMyRequestByTheDefaultTime() {
        val delayTime = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
        Thread.sleep(delayTime + ADDITIONAL_TIME_TO_DELAY)
    }
}
