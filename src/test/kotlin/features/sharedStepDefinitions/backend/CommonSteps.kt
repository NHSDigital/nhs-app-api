package features.sharedStepDefinitions.backend

import config.Config
import cucumber.api.java.Before
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.defaults.MockDefaults
import mocking.defaults.MockDefaults.Companion.DEFAULT_END_USER_SESSION_ID
import mocking.defaults.MockDefaults.Companion.patient

import mocking.MockingClient
import mocking.emis.models.AssociationType
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.NhsoHttpException
import worker.WorkerClient
import java.util.concurrent.TimeUnit


class CommonSteps : AbstractSteps() {
    @Before
    fun beforeEachScenario() {
        mockingClient = MockingClient.instance
        workerClient = WorkerClient()
        mockingClient.clearWiremock()

        setSessionVariable(MockingClient::class).to(mockingClient)
        setSessionVariable(WorkerClient::class).to(workerClient)
    }

    @Given("EMIS is unavailable")
    fun givenEmisIsUnavailable() {
        val connectionToken = "f6ca8e0c-dd67-4863-ba9e-3d34bfe930d0"
        val odsCode = "A29928"

        setSessionVariable("ConnectionToken").to(connectionToken)
        setSessionVariable("NationalPracticeCode").to(odsCode)

        mockingClient.forEmis {
            endUserSessionRequest()
                    .respondWithServiceUnavailable()
        }
    }

    @Then("^I receive (?:a|an) \"(.*)\" error")
    fun thenIReceiveAMessage(expectedStatusCode: String) {
        val converted = httpStatusCodeTransform(expectedStatusCode)
        val exception = sessionVariableCalled<NhsoHttpException>("HttpException")
        println("$exception")
        assertNotNull("An exception was expected but was not returned within the expected time limit.", exception)
        assertEquals(converted, exception.StatusCode)
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
            "unauthorized" to HttpStatus.SC_UNAUTHORIZED
    )

    fun httpStatusCodeTransform(statusName: String): Int? {
        return _statusCodeMapping[statusName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $statusName")
    }

    @Given("^I have logged in and have a valid session cookie$")
    fun givenIHaveLoggedInAndHaveAValidSessionCookie() {

        val accessToken = "access_token"

        mockingClient.forCitizenId {
            tokenRequest(MockDefaults.userSessionRequest.codeVerifier, MockDefaults.userSessionRequest.authCode)
                    .respondWithSuccess(
                            accessToken,
                            "30",
                            "30",
                            "refresh_token",
                            "token_type")
        }

        mockingClient.forCitizenId {
            userInfoRequest("Bearer ".plus(accessToken))
                    .respondWithSuccess()
        }

        mockingClient.forEmis {
            endUserSessionRequest()
                    .respondWithSuccess(DEFAULT_END_USER_SESSION_ID)
        }

        mockingClient.forEmis {
            sessionRequest(patient)
                    .respondWithSuccess(patient, AssociationType.Self)
        }

        sessionVariableCalled<WorkerClient>(WorkerClient::class).postSessionConnection(MockDefaults.userSessionRequest)
    }

    @And("I allow my session to expire")
    fun andIDelayMyRequestByTheDefaultTime() {
        var delayTime = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
        Thread.sleep(delayTime + 10)
    }
}
