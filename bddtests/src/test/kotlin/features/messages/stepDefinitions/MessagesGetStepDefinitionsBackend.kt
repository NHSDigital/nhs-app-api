package features.messages.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.InvalidAccessTokenTester
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import models.IdentityProofingLevel
import org.junit.Assert
import org.junit.Assert.assertNotNull
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.models.messages.MessageCreateResponse
import worker.models.messages.SenderFacade
import worker.models.messages.SingleMessageFacade
import worker.models.messages.MessagesResponse
import worker.models.messages.MessagesResponseMessage

class MessagesGetStepDefinitionsBackend {
    @Given("^I am an api user with proof level 5 wishing to get my messages$")
    fun iAmAnApiUserWithProofLevelFiveWishingToGetMyMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null,
                SJRJourneyType.MESSAGES_ENABLED, IdentityProofingLevel.P5)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesInCache()
    }

    @When("^I get a summary of my messages from the api$")
    fun iGetASummaryOfMyMessagesFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.getSummary(authToken)
    }

    @Then("^an attempt to get a summary of my messages with an invalid access token will return an Unauthorised error$")
    fun aAttemptToGetMessagesWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.getSummary(authToken = token)
        }
    }

    @When("^I get my messages from a sender by name from the api$")
    fun iGetMyMessagesFromASenderByNameFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER_NAME.getOrFail<String>()
        MessagesApi.getFromSenderByName(authToken, targetSender)
    }

    @When("^I get my messages from a sender by sender Id from the api$")
    fun iGetMyMessagesFromASenderByIdFromTheApi() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSenderId = MessagesSerenityHelpers.TARGET_SENDER_ID.getOrFail<String>()
        MessagesApi.getFromSenderById(authToken, targetSenderId)
    }

    @When("^I get my messages from a sender by sender Id from the api without an access token$")
    fun iGetMyMessagesFromASenderByIdFromTheApiWithoutAuthToken() {
        val targetSenderId = MessagesSerenityHelpers.TARGET_SENDER_ID.getOrFail<String>()
        MessagesApi.getFromSenderById(null, targetSenderId)
    }

    @When("^I get my messages with a summary flag and a target sender$")
    fun iGetMyMessagesWithASummaryFlagAndATargetSender() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER_NAME.getOrFail<String>()
        MessagesApi.get(authToken, true, targetSender)
    }

    @When("^I get my messages with a summary flag and a target sender Id$")
    fun iGetMyMessagesWithASummaryFlagAndATargetSenderId() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSenderId = MessagesSerenityHelpers.TARGET_SENDER_ID.getOrFail<String>()
        MessagesApi.get(authToken, true, targetSenderId = targetSenderId)
    }

    @When("^I get my messages with a target sender and a target sender Id$")
    fun iGetMyMessagesWithATargetSenderAndATargetSenderId() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER_NAME.getOrFail<String>()
        val targetSenderId = MessagesSerenityHelpers.TARGET_SENDER_ID.getOrFail<String>()
        MessagesApi.get(authToken, false, targetSender = targetSender, targetSenderId = targetSenderId)
    }

    @When("^I get my messages with a summary flag, a target sender and a target sender Id$")
    fun iGetMyMessagesWithASummaryFlagTargetSenderAndATargetSenderId() {
        val authToken = SerenityHelpers.getPatient().accessToken
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER_NAME.getOrFail<String>()
        val targetSenderId = MessagesSerenityHelpers.TARGET_SENDER_ID.getOrFail<String>()
        MessagesApi.get(authToken, true, targetSender = targetSender, targetSenderId = targetSenderId)
    }

    @When("^I get my messages without a summary flag, target sender name, or target sender Id$")
    fun iGetMyMessagesWithoutASummaryFlagTargetSenderNameOrTargetSenderId() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.get(authToken, false, null, null)
    }

    @Then("^an attempt to get messages from a sender by name with an invalid access token " +
            "will return an Unauthorised error$")
    fun aAttemptToGetMessagesFromASenderByNameWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER_NAME.getOrFail<String>()
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.getFromSenderByName(token, targetSender)
        }
    }

    @Then("^an attempt to get messages from a sender by sender Id with an invalid access token " +
            "will return an Unauthorised error$")
    fun aAttemptToGetMessagesFromASenderByIdWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        val targetSenderId = MessagesSerenityHelpers.TARGET_SENDER_ID.getOrFail<String>()
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { token ->
            MessagesApi.getFromSenderById(token, targetSenderId)
        }
    }

    @Then("^I receive a summary of my messages$")
    fun iReceiveASummaryOfMyMessages() {
        val responseMessages =
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.getOrFail<Array<MessagesResponse>>()
        val expectedMessages =
                MessagesSerenityHelpers.EXPECTED_SENDERS.getOrFail<ArrayList<SenderFacade>>()
        assertReceivedMessages(expectedMessages, responseMessages)
    }

    @Then("^I receive my messages from a sender$")
    fun iReceiveMyMessagesFromASender() {
        val responseMessages =
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.getOrFail<Array<MessagesResponse>>()
        val expectedMessages =
                MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.getOrFail<SenderFacade>()
        val reversedExpectedMessages = expectedMessages.copy(messages = expectedMessages.messages.reversed())
        assertReceivedMessages(arrayListOf(reversedExpectedMessages), responseMessages)
    }

    @Then("^I receive the message id$")
    fun iReceiveTheMessageId() {
        val response = 
                MessagesSerenityHelpers.CREATE_MESSAGE_RESPONSE.getOrFail<MessageCreateResponse>()
        assertNotNull("Message Id",response.messageId)
        MessagesSerenityHelpers.MESSAGE_ID.set(response.messageId)
    }

    @When("^I try to get the message using the message id$")
    fun iTryToGetTheMessageUsingTheMessageId(){
        val authToken = SerenityHelpers.getPatient().accessToken
        val messageId = MessagesSerenityHelpers.MESSAGE_ID.getOrFail<String>()
        MessagesApi.getMessage(authToken, messageId)
    }

    @When("^I try to get the message using the message id without passing an access token$")
    fun iTryToGetTheMessageWithoutPassingAnAccessToken(){
        val authToken = ""
        val messageId = MessagesSerenityHelpers.MESSAGE_ID.getOrFail<String>()
        MessagesApi.getMessage(authToken, messageId)
    }

    @When("^I try to get the message using a blank string$")
    fun iTryToGetTheMessageUsingABlankString(){
        val authToken = SerenityHelpers.getPatient().accessToken
        val messageId = ""
        MessagesApi.getMessage(authToken, messageId)
    }

    @When("^I try to get the message using an unrecognised message id$")
    fun iTryToGetTheMessageUsingAnUnrecognisedMessageId(){
        val authToken = SerenityHelpers.getPatient().accessToken
        val messageId = "012345678901234567890123"
        MessagesApi.getMessage(authToken, messageId)
    }

    @Then("^I receive the message$")
    fun iReceiveTheMessage() {
        val response = MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.getOrFail<MessagesResponseMessage>()
        assertNotNull("Message Id", response.id)
    }
    private fun assertReceivedMessages(expectedMessages: ArrayList<SenderFacade>,
                                       responseMessages: Array<MessagesResponse>) {
        Assert.assertEquals("Number Of Messages", expectedMessages.count(), responseMessages.count())
        responseMessages.forEach { response ->
            response.messages.forEach { message -> assertNotNull("SentTime", message.sentTime) }
        }

        val responseMessagesFacades = responseMessages.map { message ->
            SenderFacade(
                    message.sender,
                    message.unreadCount,
                    toFacade(message.messages))
        }
        for (x in 0 until expectedMessages.count()){
            assertEquals(expectedMessages[x],responseMessagesFacades[x] )
        }
    }

    fun assertEquals(expected: SenderFacade, actual: SenderFacade) {
        Assert.assertEquals("sender", expected.name, actual.name)
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
                    message.id,
                    message.sender,
                    message.body,
                    message.read?.toLowerCase() == "true",
                    message.sentTime,
                    message.version,
                    message.senderContext, message.reply)
        }
    }
}

