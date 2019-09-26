package features.messages.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.MessageRequest
import worker.models.messages.MessageResponses

class MessagesGetStepDefinitionsBackend {

    @Given("^I am an api user wishing to my their messages$")
    fun iAmAnApiUserWishingToGetTheirMessages() {
        val factory = MessagesFactory()
        factory.setUpUser("EMIS")
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am an api user wishing to get my messages, but I have no messages$")
    fun iAmAnApiUserWishingToGetTheirMessagesButIHaveNoMessages() {
        val factory = MessagesFactory()
        factory.setUpUser("EMIS")
        factory.setUpMultipleMessagesInCache()
    }

    @When("^I get my messages from the api$")
    fun iGetMyMessagesFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.get(authToken)
    }

    @When("^I get my messages from the api without an auth token$")
    fun iGetMyMessagesFromTheApiWithoutAuthToken() {
        MessagesApi.get(authToken = null)
    }

    @Then("^an attempt to get messages with an invalid access token will return an Unauthorised error$")
    fun aAttemptToGetMessagesWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.get( authToken = token)
        }
    }

    @Then("^I receive my messages$")
    fun iReceiveMyMessages() {
        val response =
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.getOrFail<MessageResponses>()
        val expectedMessages =
                MessagesSerenityHelpers.EXPECTED_MESSAGES.getOrFail<ArrayList<MessageRequest>>()
        Assert.assertNotNull("Messages response", response)
        Assert.assertEquals("Number Of Messages",
                expectedMessages.count(),
                response.messages.count())
        val actualContent = response.messages.map { message -> Pair(message.body, message.sender) }.toTypedArray()
        val expectedContent = expectedMessages.map { message -> Pair(message.body, message.sender) }.toTypedArray()
        Assert.assertArrayEquals("Message Content", expectedContent, actualContent)
    }
}

