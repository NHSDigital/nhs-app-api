package features.messages.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import pages.messages.MessagesInboxPage
import pages.messages.MessagesPage
import utils.getOrFail
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade

class MessagesStepDefinitions {

    private lateinit var messagesPage: MessagesPage
    private lateinit var messagesInboxPage: MessagesInboxPage

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
}

