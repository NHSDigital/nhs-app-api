package features.configuration.stepDefinitions

import cucumber.api.java.en.When
import net.serenitybdd.core.Serenity
import utils.set
import worker.WorkerClient
import worker.models.clientLogger.ClientLoggerRequest

class ClientLoggerStepDefinitions {

    @When("^I post a valid log event to the logger$")
    fun iPostAValidLogEventToTheLogger() {
        val level = "Error"
        val message = "This is a message"
        val timeStamp = "2020-06-14T17:32:28Z"
        val request = ClientLoggerRequest(timeStamp, level, message)

        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .clientLogger
                .post(request)
        ClientLoggerSerenityHelpers.CLIENT_LOGGER_RESPONSE.set(response)
    }

    @When("^I post an invalid log event to the logger$")
    fun iPostAnInvalidLogEventToTheLogger() {
        val level = "ErrorXX"
        val message = "This is a message"
        val timeStamp = "2020-06-14T17:32:28Z"
        val request = ClientLoggerRequest(timeStamp, level, message)

        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .clientLogger
                .post(request)
        ClientLoggerSerenityHelpers.CLIENT_LOGGER_RESPONSE.set(response)
    }

}
