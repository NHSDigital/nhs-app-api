package features.serviceStatus.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.serenitybdd.core.Serenity
import org.apache.http.HttpResponse
import org.junit.Assert.assertEquals
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.WorkerClientHealth

const val HTTP_STATUS_NO_CONTENT = 204

class ServiceStatusStepDefinitions {

    @When("^the service journey rules service readiness is requested$")
    fun theServiceJourneyRulesServiceReadinessIsRequested() =
            theServiceReadinessIsRequested { workerClient -> workerClient.sjrHealth }

    @Then("^the response from the service readiness endpoint has status code 204$")
    fun theResponseFromTheServiceReadinessEndpointHasStatusCode204() {
        val response = ServiceStatusSerenityHelpers.READINESS_ENDPOINT_RESPONSE.getOrFail<HttpResponse>()
        assertEquals(HTTP_STATUS_NO_CONTENT, response.statusLine.statusCode)
    }

    private fun theServiceReadinessIsRequested(client: (WorkerClient) -> WorkerClientHealth) {
        val workerClient = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
        val response = client(workerClient).getReady()
        ServiceStatusSerenityHelpers.READINESS_ENDPOINT_RESPONSE.set(response)
    }
}
