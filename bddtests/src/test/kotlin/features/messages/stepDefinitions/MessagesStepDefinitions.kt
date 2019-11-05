package features.messages.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import pages.messages.MessagesErrorPage
import pages.messages.MessagesInboxPage
import pages.messages.MessagesPage
import utils.getOrFail
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade

class MessagesStepDefinitions {

    private lateinit var messagesPage: MessagesPage
    private lateinit var messagesInboxPage: MessagesInboxPage
    private lateinit var messagesErrorPage: MessagesErrorPage

    @Given("^I am a user wishing to view my messages$")
    fun iAmAUserWishingToViewTheirMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am a user wishing to view my messages, but I have no messages$")
    fun iAmAUserWishingToViewTheirMessagesButIHaveNoMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
    }

    @Given("^I am a user wishing to view my messages but retrieving the messages will cause an internal server error$")
    fun iAmAUserWishingToViewTheirMessagesButIHaveAnInvalidMessage() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpInvalidMessageInCache()
    }

    @When("^the messages in the repository can be retrieved successfully$")
    fun theMessagesInTheRepositoryCanBeRetrievedSuccessfully(){
        val factory = MessagesFactory()
        factory.setUpMultipleMessagesInCache()
    }

    @When("^retrieving the messages from the repository will cause an internal server error$")
    fun retrievingTheMessagesFromTheRepositoryWillCauseAnInternalServerError(){
        val factory = MessagesFactory()
        factory.setUpInvalidMessageInCache()
    }

    @When("^I click on a sender in the Messages Inbox$")
    fun iClickOnASenderInTheMessagesInbox() {
        messagesInboxPage.messages.selectMessageSummary(MessagesSerenityHelpers.TARGET_SENDER.getOrFail())
    }

    @Then("the Messages Inbox page is displayed")
    fun theMessagesInboxPageIsDisplayed() {
        messagesInboxPage.assertDisplayed()
    }

    @Then("the senders and latest messages are displayed on the Messages Inbox page")
    fun theSendersAndLatestMessagesAreDisplayedOnTheMessageInboxPage() {
        messagesInboxPage.messages.assertMessages(MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES.getOrFail())
    }

    @Then("the viewed messages are marked as read on the Messages Inbox page")
    fun theViewedMessagesAreMarkedAsReadOnTheMessageInboxPage() {
        messagesInboxPage.messages.assertMessages(
                MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES_AFTER_READING_SENDER.getOrFail())
    }

    @Then("a message is displayed indicating that there are no messages in the Messages Inbox")
    fun aMessageIsDisplayedIndicatingThatThereAreNoMessagesInTheMessagesInbox() {
        messagesInboxPage.assertNoMessages()
    }

    @Then("the Messages page is displayed")
    fun theMessagesPageIsDisplayed() {
        messagesPage.assertDisplayed(MessagesSerenityHelpers.TARGET_SENDER.getOrFail())
    }

    @Then("^my messages from the sender are displayed$")
    fun myMessagesFromTheSenderAreDisplayed() {
        val expectedUnreadMessages = MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES
                .getOrFail<ArrayList<SingleMessageFacade>>()
        val expectedReadMessages = MessagesSerenityHelpers.EXPECTED_READ_MESSAGES
                .getOrFail<ArrayList<SingleMessageFacade>>()
        val sender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        messagesPage.messages.assertReadMessages(expectedReadMessages, sender)
        messagesPage.messages.assertUnreadMessages(expectedUnreadMessages, sender)
    }

    @Then("^my messages from the sender are displayed as read$")
    fun myMessagesFromTheSenderAreDisplayedAsRead() {
        val expectedMessages = MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER
                .getOrFail<MessagesSummaryFacade>().messages
        val expectedReadMessages = arrayListOf<SingleMessageFacade>()
        expectedReadMessages.addAll(expectedMessages)
        val sender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        messagesPage.messages.assertAllReadMessages(expectedReadMessages, sender)
    }

    @Then( "^an error with a retry button is displayed indicating that there was a problem getting messages$")
    fun anErrorWithARetryButtonIsDisplayedIndicatingThatThereWasAProblemGettingMessages(){
        messagesErrorPage.assertInboxErrorPage()
    }

    @Then( "^an error with a retry button is displayed indicating that there was a problem getting " +
            "messages from the sender$")
    fun anErrorWithARetryButtonIsDisplayedIndicatingThatThereWasAProblemGettingMessagesFromTheSender(){
        val sender = MessagesSerenityHelpers.TARGET_SENDER.getOrFail<String>()
        messagesErrorPage.assertSenderErrorPage(sender)
    }
}

