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
import worker.models.messages.Sender
import worker.models.messages.SendersResponse

class MessagesGetStepDefinitionsBackend {

    @Given("^I am an api user wishing to get my messages$")
    fun iAmAnApiUserWishingToGetTheirMessages() {
        val factory = MessagesFactory()
        factory.setUpUser()
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am an api user with proof level 5 wishing to get my messages$")
    fun iAmAnApiUserWithProofLevelFiveWishingToGetMyMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null,
                SJRJourneyType.MESSAGES_ENABLED, IdentityProofingLevel.P5)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am an api user wishing to get my messages, but I have no messages$")
    fun iAmAnApiUserWishingToGetTheirMessagesButIHaveNoMessages() {
        val factory = MessagesFactory()
        factory.setUpUser()
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

    @When("^I try to get the message senders without passing an access token$")
    fun iTryToGetTheMessageSendersWithoutPassingAnAccessToken() {
        val authToken = ""
        MessagesApi.getSenders(authToken)
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

    @When("^I try to get a list of message senders$")
    fun iTryToGetAListOfMessageSenders() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.getSenders(authToken)
    }

    @Then("^I can see a list of message senders along with a count of unread messages per sender$")
    fun iCanSeeAListOfMessageSendersAlongWithACountOfUnreadMessagesPerSender() {
        val response = MessagesSerenityHelpers.GET_SENDERS.getOrFail<SendersResponse>()
        val expectedMessages = MessagesSerenityHelpers.EXPECTED_SENDERS
            .getOrFail<ArrayList<SenderFacade>>()
        assertNotNull(response)
        assertNotNull(response.senders)
        val expectedSenders = expectedMessages.map { message ->
            Sender(
                message.name,
                message.unreadCount
            )
        }

        Assert.assertEquals("Number Of Senders", expectedSenders.count(), response.senders.count())

        for (x in 0 until expectedSenders.count()){
            Assert.assertEquals(expectedSenders[x].name, response.senders[x].name)
            Assert.assertEquals(expectedSenders[x].unreadCount, response.senders[x].unreadCount)
        }
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
                    message.read?.toLowerCase() =="true" ,
                    message.sentTime,
                    message.version)
        }
    }
}

