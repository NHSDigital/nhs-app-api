package features.messages.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import pages.messages.MessagesPage
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.MessageFacade

class MessagesStepDefinitions {

    private lateinit var messagesPage: MessagesPage

    @Given("^I am a user wishing to view my messages$")
    fun iAmAUserWishingToViewTheirMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(SerenityHelpers.getGpSupplier(), patient)
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am a user wishing to view my messages, but I have no messages$")
    fun iAmAUserWishingToViewTheirMessagesButIHaveNoMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                ServiceJourneyRulesMapper.Companion.JourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(SerenityHelpers.getGpSupplier(), patient)
    }

    @Then("the Messages page is displayed")
    fun theMessagesPageIsDisplayed(){
        messagesPage.assertDisplayed("Everyone")
    }

    @Then("my messages are displayed")
    fun myMessagesAreDisplayed(){
        val expectedUnreadMessages = MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES
                .getOrFail<ArrayList<MessageFacade>>()
        val expectedReadMessages = MessagesSerenityHelpers.EXPECTED_READ_MESSAGES
                .getOrFail<ArrayList<MessageFacade>>()
        messagesPage.messages.assertReadMessages(expectedReadMessages)
        messagesPage.messages.assertUnreadMessages(expectedUnreadMessages)
    }

    @Then("no messages are displayed")
    fun noMessagesAreDisplayed(){
        messagesPage.messages.assertReadMessages(arrayListOf())
        messagesPage.messages.assertUnreadMessages(arrayListOf())
    }
}

