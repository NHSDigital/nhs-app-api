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
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes;


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

    @Given("^(EMIS|VISION) is not available$")
    fun givenXIsNotAvailable(gpSystem: String) {        when (gpSystem) {
            EMIS -> {
                val connectionToken = "f6ca8e0c-dd67-4863-ba9e-3d34bfe930d0"
                val odsCode = "A29928"

                setSessionVariable("ConnectionToken").to(connectionToken)
                setSessionVariable("NationalPracticeCode").to(odsCode)

                mockingClient.forEmis {
                    endUserSessionRequest()
                            .respondWithServiceUnavailable()
                }
            }
            VISION -> {
                val patient = MockDefaults.patientVision

                setSessionVariable("ConnectionToken").to(patient.connectionToken)
                setSessionVariable("NationalPracticeCode").to(patient.odsCode)

                mockingClient.forVision {
                    getConfigurationRequest(
                            MockDefaults.visionUserSession,
                            MockDefaults.visionGetConfiguration)
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

        when (ProviderTypes.valueOf(gpSystem)) {
            ProviderTypes.EMIS -> {
                mockingClient.forCitizenId {
                    tokenRequest(patient.cidUserSession.codeVerifier, patient.cidUserSession.authCode)
                            .respondWithSuccess(
                                    patient.accessToken,
                                    "30",
                                    "30",
                                    "refresh_token",
                                    "token_type")
                }

                mockingClient.forCitizenId {
                    userInfoRequest("Bearer ".plus(patient.accessToken))
                            .respondWithSuccess(Patient.getDefault("EMIS"))
                }

                mockingClient.forEmis {
                    endUserSessionRequest()
                            .respondWithSuccess(DEFAULT_END_USER_SESSION_ID)
                }

                mockingClient.forEmis {
                    sessionRequest(patient)
                            .respondWithSuccess(patient, AssociationType.Self)
                }
                mockingClient.forEmis {
                    demographicsRequest(patient)
                            .respondWithSuccess(patient,
                                    patientIdentifiers = arrayOf(
                                            PatientIdentifier(
                                                    identifierType = IdentifierType.NhsNumber,
                                                    identifierValue = patient.nhsNumbers[0])))
                }

            }
            ProviderTypes.TPP -> {
                CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patientTpp)

                mockingClient.forTpp {

                    val authReply = AuthenticateReply()
                    authReply.uuid = UUID.randomUUID().toString()
                    authReply.user.person.dateOfBirth = MockDefaults.patientTpp.dateOfBirth
                    authReply.patientId = MockDefaults.patientTpp.patientId
                    authReply.onlineUserId = MockDefaults.patientTpp.onlineUserId

                    authenticateRequest(MockDefaults.tppAuthenticateRequest).respondWithSuccess(authReply)
                }
            }
        }

        sessionVariableCalled<WorkerClient>(WorkerClient::class).postSessionConnection(patient.cidUserSession)
        Serenity.setSessionVariable(GP_SYSTEM).to(gpSystem)
    }

    @And("I allow my session to expire")
    fun andIDelayMyRequestByTheDefaultTime() {
        var delayTime = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
        Thread.sleep(delayTime + 10)
    }
}