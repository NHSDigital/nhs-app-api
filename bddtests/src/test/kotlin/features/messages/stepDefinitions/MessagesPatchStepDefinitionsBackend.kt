package features.messages.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.MessageRequest

class MessagesPatchStepDefinitionsBackend {

    @Given("^I am an api user wishing to mark a message as read$")
    fun iAmAApiUserWishingToMarkAMessageAsRead() {
        val factory = MessagesFactory()
        factory.setUpUser()
        factory.setUpSingleUnreadMessage()
    }

    @When("^I patch the message to indicate that it has been read$")
    fun iPatchTheMessageToIndicateThatItHasBeenRead() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val messageId = MessagesSerenityHelpers.MESSAGE_ID.getOrFail<String>()
        MessagesApi.patch(authToken, messageId, MessagesFactory.patchToUpdateAsRead )
    }

    @When("^I patch the message to indicate that it has been read without an access token$")
    fun iGetMyMessagesFromASenderFromTheApiWithoutAuthToken() {
        val messageId = MessagesSerenityHelpers.MESSAGE_ID.getOrFail<String>()
        MessagesApi.patch(authToken = null, messageId =  messageId, patch = MessagesFactory.patchToUpdateAsRead)
    }

    @Then("^an attempt to mark a message as read with an invalid access token will return an Unauthorised error$")
    fun aAttemptToGetMessagesFromASenderWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        val messageId = MessagesSerenityHelpers.MESSAGE_ID.getOrFail<String>()
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.patch(authToken = token, messageId =  messageId, patch = MessagesFactory.patchToUpdateAsRead)
        }
    }

    @Then("^the message has been marked as read in the repository$")
    fun theMessageIsAvailableInTheDatabase() {
        val expectedMessage = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MessageRequest>()
        MessagesRepository.assertSingleMessageInRepository(expectedMessage, read = true)
    }
}

