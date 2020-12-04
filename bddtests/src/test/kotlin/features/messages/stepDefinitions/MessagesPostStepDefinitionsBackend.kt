package features.messages.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mongodb.MongoDBConnection
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.models.messages.MessageRequest

class MessagesPostStepDefinitionsBackend {

    @Given("^I am an api user wishing to post a message$")
    fun iAmAApiUserWishingToPostAMessage() {
        iAmAnApiUserWishingToPostAMessage("Communication One", "Transmission One")
    }

    @Given("^I am an api user wishing to post a message without a communication ID or Transmission ID$")
    fun iAmAApiUserWishingToPostAMessageWithoutACommunicationIdOrTransmissionId() {
        iAmAnApiUserWishingToPostAMessage()
    }

    private fun iAmAnApiUserWishingToPostAMessage(communicationId: String? = null, transmissionId: String? = null) {
        MongoDBConnection.MessagesCollection.clearCache()
        val message = MessageRequest(
                sender = "Sender One",
                body = "Message One",
                communicationId = communicationId,
                transmissionId = transmissionId,
                version = MessageVersion.PLAIN_TEXT.value)
        val nhsLoginId = "0123456789ABCDEF"
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        MessagesSerenityHelpers.EXPECTED_MESSAGE.set(message)
    }

    @When("^I post a message to the api$")
    fun iPostAMessageToTheApi() {
        val request = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MessageRequest>()
        val nhsLoginId = MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.getOrFail<String>()
        MessagesApi.post(request, nhsLoginId)
    }

    @When("^I post a message to the api without the api key$")
    fun iPostAMessageToTheApiWithoutTheApiKey() {
        val request = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MessageRequest>()
        val nhsLoginId = MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.getOrFail<String>()
        MessagesApi.post(request, nhsLoginId, includeApiKey = false)
    }

    @Then("^the message is available in the database$")
    fun theMessageIsAvailableInTheDatabase() {
        val expectedMessage = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MessageRequest>()
        MessagesRepository.assertSingleMessageInRepository(expectedMessage, read = false)
    }

    @Then("^an attempt to post incomplete messages will return a Bad Request error$")
    fun anAttemptToPostMessagesWillReturnABadRequestError() {
        val validRequest = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MessageRequest>()
        val nhsLoginId = MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.getOrFail<String>()

        assertInvalidMessageThrowsBadRequest(validRequest, nhsLoginId, "body")
        { messageRequest -> messageRequest.body = "" }
        assertInvalidMessageThrowsBadRequest(validRequest, nhsLoginId, "sender")
        { messageRequest -> messageRequest.sender = "" }
        assertInvalidMessageThrowsBadRequest(validRequest, nhsLoginId, "version")
        { messageRequest -> messageRequest.version = 0 }
    }

    private fun assertInvalidMessageThrowsBadRequest(
            request: MessageRequest,
            nhsLoginId: String,
            invalidParam: String,
            requestChange: (MessageRequest) -> Unit) {

        SerenityHelpers.clearHttpException()
        requestChange.invoke(request)
        MessagesApi.post(request, nhsLoginId)
        val errorResponse = SerenityHelpers.getHttpException()
        Assert.assertNotNull(
                "An exception was expected but was not returned within the expected time limit. " +
                        "Invalid Param: $invalidParam"
        )
        Assert.assertEquals("Incorrect status code returned. Invalid Param: $invalidParam",
                HttpStatus.SC_BAD_REQUEST,
                errorResponse!!.statusCode)
    }
}

