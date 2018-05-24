package features.sharedStepDefinitions.backend

import config.Config
import cucumber.api.java.Before
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then

import mocking.MockingClient
import mocking.emis.session.EmisEndUserSessionBuilder
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.apache.http.HttpStatus
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.NhsoHttpException
import worker.WorkerClient


class CommonSteps : AbstractSteps() {

    @Before
    fun beforeEachScenario() {
        mockingClient = MockingClient.instance
        workerClient = WorkerClient()
        mockingClient.resetWiremock()

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
        var exception = sessionVariableCalled<NhsoHttpException>("HttpException")
        println("$exception")
        assertNotNull("An exception was expected but was not returned within the expected time limit.", exception)
        assertEquals(converted, exception.StatusCode)
    }

    private val _errorMapping: HashMap<String, Int> = hashMapOf<String, Int>(
            "bad gateway" to HttpStatus.SC_BAD_GATEWAY,
            "bad request" to HttpStatus.SC_BAD_REQUEST,
            "gateway timeout" to HttpStatus.SC_GATEWAY_TIMEOUT,
            "not found" to HttpStatus.SC_NOT_FOUND,
            "internal server error" to HttpStatus.SC_INTERNAL_SERVER_ERROR,
            "conflict" to HttpStatus.SC_CONFLICT,
            "forbidden" to HttpStatus.SC_FORBIDDEN,
            "service unavailable" to HttpStatus.SC_SERVICE_UNAVAILABLE
    )

    fun httpStatusCodeTransform(errorName: String): Int? {
        return _errorMapping[errorName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $errorName")
    }
}
