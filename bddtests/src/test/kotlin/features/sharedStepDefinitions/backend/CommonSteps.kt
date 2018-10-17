package features.sharedStepDefinitions.backend

import config.Config
import cucumber.api.java.Before
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.defaults.MockDefaults
import mocking.defaults.MockDefaults.Companion.DEFAULT_END_USER_SESSION_ID

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.AuthenticateReply
import models.Patient
import net.serenitybdd.core.Serenity.*
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.NhsoHttpException
import worker.WorkerClient
import java.util.*
import java.util.concurrent.TimeUnit
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes
import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import features.sharedSteps.SerenityHelpers
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.tpp.models.Authenticate


class CommonSteps : AbstractSteps() {
    companion object {
        val GP_SYSTEM: String = "GP_SYSTEM"
    }

    private val EMIS = "EMIS"
    private val TPP = "TPP"
    private val VISION = "VISION"

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
            EMIS -> {
                mockingClient.forEmis {
                    authentication.endUserSessionRequest()
                            .respondWithServiceUnavailable()
                }
            }
            TPP -> {
                mockingClient.forTpp {
                    authentication.authenticateRequest(Authenticate())
                            .respondWithServiceUnavailable()
                }
            }
            VISION -> {
                mockingClient.forVision {
                    getConfigurationRequest(MockDefaults.visionUserSession)
                            .respondWithServiceUnavailable()
                }
            }
        }
    }

    @Then("^I receive (?:a|an) \"(.*)\" error")
    fun thenIReceiveAMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val exception = sessionVariableCalled<NhsoHttpException>("HttpException")
        assertNotNull("An exception was expected but was not returned within the expected time limit.", exception)
        assertEquals(converted, exception.statusCode)
    }

    @Then("^I receive (?:a|an) \"(.*)\" success code")
    fun thenIReceiveASuccessMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val httpResponse = sessionVariableCalled<HttpResponse>("HttpResponse")
        assertEquals(converted, httpResponse.statusLine.statusCode)
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
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        sessionVariableCalled<WorkerClient>(WorkerClient::class).postSessionConnection(patient.cidUserSession)
        Serenity.setSessionVariable(GP_SYSTEM).to(gpSystem)
        setSessionVariable(GLOBAL_PROVIDER_TYPE).to(gpSystem)
        SerenityHelpers.setPatient(patient)
    }

    @And("I allow my session to expire")
    fun andIDelayMyRequestByTheDefaultTime() {
        var delayTime = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
        Thread.sleep(delayTime + 10)
    }
}