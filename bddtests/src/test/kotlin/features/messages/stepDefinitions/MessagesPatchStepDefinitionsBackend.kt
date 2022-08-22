package features.messages.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryMessage
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.MessageRequest

class MessagesPatchStepDefinitionsBackend {

    @Given("^I am an api user with a read message$")
    fun iAmAnApiUserWithAReadMessage() {
        val factory = MessagesFactory()
        factory.setUpUser()
        factory.setUpSingleReadMessage()
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

    @Then("^an attempt to mark a message as read with an invalid access token will return an Internal Server error$")
    fun aAttemptToGetMessagesFromASenderWithAnInvalidAccessTokenWillReturnAnInternalServerError() {
        val messageId = MessagesSerenityHelpers.MESSAGE_ID.getOrFail<String>()
        InvalidAccessTokenTester.assertInvalidTokensThrowInternalServer { token ->
            MessagesApi.patch(authToken = token, messageId =  messageId, patch = MessagesFactory.patchToUpdateAsRead)
        }
    }

    @Then("^the message has been marked as read in the repository$")
    fun theMessageIsAvailableInTheDatabase() {
        val expectedMessage = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MessageRequest>()
        MessagesRepository.assertSingleMessageInRepository(expectedMessage, read = true)
    }

    @Then("^the message has not been updated in the repository$")
    fun theMessagesHasNotBeenUpdatedInTheDatabase() {
        val expectedMessage = MessagesSerenityHelpers.EXPECTED_MESSAGE.getOrFail<MongoRepositoryMessage>()
        val message = MongoDBConnection.MessagesCollection
            .getValues<MongoRepositoryMessage>(MongoRepositoryMessage::class.java)
            .first()
        Assert.assertEquals(expectedMessage.ReadTime, message.ReadTime)
    }
}

