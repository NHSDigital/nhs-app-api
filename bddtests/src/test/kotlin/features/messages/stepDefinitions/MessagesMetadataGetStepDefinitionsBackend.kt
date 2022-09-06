package features.messages.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.models.messages.MessagesMetadataResponse

class MessagesMetadataGetStepDefinitionsBackend {

    @Given("^I am an api user wishing to get my unread count of messages$")
    fun iAmAnApiUserWishingToGetUnReadCountOfMessages() {
        val factory = MessagesFactory()
        factory.setUpUser()
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am an api user wishing to get my unread count of messages, but I have no messages$")
    fun iAmAnApiUserWishingToGetTheirMessagesButIHaveNoMessages() {
        val factory = MessagesFactory()
        factory.setUpUser()
    }

    @When("^I get unread count of my messages from the api$")
    fun iGetUnreadCountOfMyMessagesFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.getMessagesMetadata(authToken)
    }

    @When("^I get unread count of my messages from the api without an access token$")
    fun iGetMyUnreadMessagesCountFromTheApiWithoutAuthToken() {
        MessagesApi.getMessagesMetadata(authToken = null)
    }

    @Then("^I receive unread count of my messages$")
    fun iReceiveUnreadCountOfMyMessages() {
        val responseMessages =
            MessagesSerenityHelpers.GET_MESSAGES_METADATA_RESPONSE.getOrFail<MessagesMetadataResponse>()
        val expectedUnReadMessagesCount =
            MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES_COUNT.getOrFail<Long>()

        Assert.assertEquals(responseMessages.unreadMessageCount, expectedUnReadMessagesCount)
    }
}
