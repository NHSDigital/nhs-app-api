package features.messages.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade
import worker.models.messages.MessagesResponse
import worker.models.messages.MessagesResponseMessage

class MessagesGetStepDefinitionsBackend {

    @Given("^I am an api user wishing to get my messages$")
    fun iAmAnApiUserWishingToGetTheirMessages() {
        val factory = MessagesFactory()
        factory.setUpUser("EMIS")
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am an api user wishing to get my messages, but I have no messages$")
    fun iAmAnApiUserWishingToGetTheirMessagesButIHaveNoMessages() {
        val factory = MessagesFactory()
        factory.setUpUser("EMIS")
    }

    @When("^I get a summary of my messages from the api$")
    fun iGetASummaryOfMyMessagesFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.getSummary(authToken)
    }

    @When("^I get a summary of my messages from the api without an access token$")
    fun iGetMyMessagesFromTheApiWithoutAuthToken() {
        MessagesApi.getSummary(authToken = null)
    }

    @Then("^an attempt to get a summary of my messages with an invalid access token will return an Unauthorised error$")
    fun aAttemptToGetMessagesWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.getSummary(authToken = token)
        }
    }

    @When("^I get my messages from a sender from the api$")
    fun iGetMyMessagesFromASenderFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        MessagesApi.getFromSender(authToken, targetSender)
    }

    @When("^I get my messages from a sender from the api without an access token$")
    fun iGetMyMessagesFromASenderFromTheApiWithoutAuthToken() {
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        MessagesApi.getFromSender(authToken = null, targetSender = targetSender)
    }

    @When("^I get my messages with a summary flag and a target sender$")
    fun iGetMyMessagesWithASummaryFlagAndATargetSender() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        MessagesApi.get(authToken,true, targetSender)
    }

    @When("^I get my messages without a summary flag and without a target sender$")
    fun iGetMyMessagesWithoutASummaryFlagAndwithoutATargetSender() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.get(authToken,false, null)
    }

    @Then("^an attempt to get messages from a sender with an invalid access token will return an Unauthorised error$")
    fun aAttemptToGetMessagesFromASenderWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.getFromSender(authToken = token, targetSender = targetSender)
        }
    }

    @Then("^I receive a summary of my messages$")
    fun iReceiveASummaryOfMyMessages() {
        val responseMessages =
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.getOrFail<Array<MessagesResponse>>()
        val expectedMessages =
                MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES.getOrFail<ArrayList<MessagesSummaryFacade>>()

        assertReceivedMessages(expectedMessages, responseMessages)
    }

    @Then("^I receive my messages from a sender$")
    fun iReceiveMyMessagesFromASender() {
        val responseMessages =
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.getOrFail<Array<MessagesResponse>>()
        val expectedMessages =
                MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.getOrFail<MessagesSummaryFacade>()
        assertReceivedMessages(arrayListOf(expectedMessages), responseMessages)
    }

    private fun assertReceivedMessages(expectedMessages: ArrayList<MessagesSummaryFacade>,
                                       responseMessages: Array<MessagesResponse>) {

        Assert.assertEquals("Number Of Messages", expectedMessages.count(), responseMessages.count())
        responseMessages.forEach { response ->
            response.messages.forEach { message -> Assert.assertNotNull("SentTime", message.sentTime) }
        }

        val responseMessagesFacades = responseMessages.map { message ->
            MessagesSummaryFacade(
                    message.sender,
                    message.unreadCount,
                    toFacade(message.messages))
        }
        for (x in 0 until expectedMessages.count()){
            assertEquals(expectedMessages[x],responseMessagesFacades[x] )
        }
    }

    fun assertEquals(expected: MessagesSummaryFacade, actual: MessagesSummaryFacade) {
        Assert.assertEquals("sender", expected.sender, actual.sender)
        Assert.assertEquals("unreadCount", expected.unreadCount, actual.unreadCount)
        Assert.assertEquals("messages Count", expected.messages.count(), actual.messages.count())
        for (x in 0 until expected.messages.count()){
            assertEquals(expected.messages[x],actual.messages[x] )
        }
    }

    fun assertEquals(expected: SingleMessageFacade, actual: SingleMessageFacade) {
        Assert.assertEquals("sender", expected.sender, actual.sender)
        Assert.assertEquals("body", expected.body, actual.body)
        Assert.assertEquals("read", expected.read, actual.read)
        Assert.assertEquals("sentTime",
                expected.sentTime.replaceAfter("T", ""),
                actual.sentTime.replaceAfter("T", ""))
    }

    private fun toFacade(messages: Array<MessagesResponseMessage>): List<SingleMessageFacade> {
        return messages.map { message ->
            SingleMessageFacade(
                    message.sender,
                    message.body,
                    message.read?.toLowerCase() =="true" ,
                    message.sentTime)
        }
    }
}

