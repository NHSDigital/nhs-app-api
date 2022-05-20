package features.messages.stepDefinitions

import config.Config
import constants.Supplier
import io.cucumber.datatable.DataTable
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.AccessTokenBuilder
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import models.IdentityProofingLevel
import pages.ErrorDialogPage
import pages.messages.MessagePage
import pages.messages.MessageSendersPage
import pages.messages.MessagesPage
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.SenderFacade
import worker.models.messages.SingleMessageFacade
import java.util.*
import kotlin.collections.ArrayList

private const val ACCESS_TOKEN_ABOUT_TO_EXPIRE_MILLISECONDS = 50000

class MessagesStepDefinitions {

    private lateinit var messagesPage: MessagesPage
    private lateinit var messagePage: MessagePage
    private lateinit var messageSendersPage: MessageSendersPage
    private lateinit var errorDialogPage: ErrorDialogPage

    @Given("^I am a user wishing to view my messages$")
    fun iAmAUserWishingToViewTheirMessages() {
        val factory = setupMessagesEnabledPatient()
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am a user with proof level 5 wishing to view my messages$")
    fun iAmAUserWithProofLevel5WishingToViewTheirMessages() {
        val patient = ServiceJourneyRulesMapper
                .findPatientForConfiguration(null, SJRJourneyType.MESSAGES_ENABLED, IdentityProofingLevel.P5)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am a user with proof level 5 whose access token is about to expire wishing to view my messages$")
    fun iAmAUserWithProofLevel5WhoseAccessTokenIsAboutToExpireWishingToViewTheirMessages() {
        val patient = ServiceJourneyRulesMapper
                .findPatientForConfiguration(null, SJRJourneyType.MESSAGES_ENABLED, IdentityProofingLevel.P5)
        patient.accessToken = AccessTokenBuilder()
                .getSignedToken(
                        patient,
                        expirationTimeOverride = Date(Date().time + ACCESS_TOKEN_ABOUT_TO_EXPIRE_MILLISECONDS))
                .serialize()
        patient.refreshToken = AccessTokenBuilder().getSignedToken(patient).serialize()

        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesInCache()
    }

    @Given("^I am a user wishing to view my messages and GP surgery messages$")
    fun iAmAUserWishingToViewTheirMessagesAndGPMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                arrayListOf(
                        SJRJourneyType.MESSAGES_ENABLED,
                        SJRJourneyType.IM1_MESSAGING_ENABLED))
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesInCache()

        PracticePatientMessagingFactory
                .getForSupplier(Supplier.TPP)
                .enabled(patient)
    }

    @Given("^I am a user wishing to view my appointments and my messages with content$")
    fun iAmAUserWishingToViewTheirAppointmentsAndMessagesWithContent(table: DataTable) {
        val factory = setupMessagesEnabledPatient()
        factory.setUpMultipleMessagesWithContentInCache(table, MessageVersion.PLAIN_TEXT)
        val factoryAppointments = AppointmentsBookingFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factoryAppointments.generateDefaultAvailableAppointmentSlotExample()
        factoryAppointments.generateSuccessfulBookingResponse()
    }

    @Given("^I am a user wishing to view my appointments and my messages with markdown content$")
    fun iAmAUserWishingToViewTheirAppointmentsAndMessagesWithMarkdownContent(table: DataTable) {
        val factory = setupMessagesEnabledPatient()
        factory.setUpMultipleMessagesWithContentInCache(table, MessageVersion.MARKDOWN)
        val factoryAppointments = AppointmentsBookingFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factoryAppointments.generateDefaultAvailableAppointmentSlotExample()
        factoryAppointments.generateSuccessfulBookingResponse()
    }

    @Given("^I am a user wishing to view my messages with content$")
    fun iAmAUserWishingToViewTheirMessagesWithContent(table: DataTable) {
        val factory = setupMessagesEnabledPatient()
        factory.setUpMultipleMessagesWithContentInCache(table, MessageVersion.PLAIN_TEXT)
    }

    @Given("^I am a user wishing to view my messages with markdown content$")
    fun iAmAUserWishingToViewTheirMessagesWithMarkdownContent(table: DataTable) {
        val factory = setupMessagesEnabledPatient()
        factory.setUpMultipleMessagesWithContentInCache(table, MessageVersion.MARKDOWN)
    }

    @Given("^I am a user wishing to view my messages, but I have no messages$")
    fun iAmAUserWishingToViewTheirMessagesButIHaveNoMessages() {
        setupMessagesEnabledPatient()
    }

    @When("^the messages can be retrieved successfully$")
    fun theMessagesCanBeRetrievedSuccessfully(){
        MessagesFactory().setUpValidMessageInCache()
    }

    @When("^retrieving the messages will cause a server error$")
    fun retrievingTheMessagesFromTheApiWillCauseAServerError(){
        MessagesFactory().setUpInvalidMessageInCache()
    }

    @When("^I click on (.*) sender$")
    fun iClickOnSender(sender: String) {
        messageSendersPage.senders.select(sender)
    }

    @Then("^the (.*) sender is displayed as (unread|read)$")
    fun theSenderIsDisplayedAsStatus(sender: String, status: String) {
        when(status) {
            "unread" -> messageSendersPage.senders.assertUnread(sender)
            "read" -> messageSendersPage.senders.assertRead(sender)
        }
    }

    @When("^I click on (a|the unread|.*) message on the Sender Messages page$")
    fun iClickOnMessageOnTheSenderMessagesPage(message: String) {
        val messageBody = when(message) {
            "a" -> MessagesSerenityHelpers.TARGET_MESSAGE.getOrFail()
            "the unread" -> MessagesSerenityHelpers.TARGET_UNREAD_MESSAGE.getOrFail()
            else -> message
        }
        messagesPage.messages.select(messageBody)
    }

    @Then("^the Message Senders page is displayed$")
    fun theMessagesInboxPageIsDisplayed() {
        messageSendersPage.assertHeaderDisplayed()
    }

    @Then("^the senders are displayed on the Messages Inbox page$")
    fun theSendersAreDisplayedOnTheMessageInboxPage() {
        messageSendersPage.senders.assertAll(MessagesSerenityHelpers.EXPECTED_SENDERS.getOrFail())
    }

    @Then("^a message is displayed indicating that there are no messages in the Messages Inbox$")
    fun aMessageIsDisplayedIndicatingThatThereAreNoMessagesInTheMessagesInbox() {
        messageSendersPage.assertNoSenders()
    }

    @Then("^the Sender Messages page is displayed$")
    fun theSenderMessagesPageIsDisplayed() {
        messagesPage.assertDisplayed(MessagesSerenityHelpers.TARGET_SENDER.getOrFail())
    }

    @Then("^my messages from the sender are displayed$")
    fun myMessagesFromTheSenderAreDisplayed() {
        val expectedUnreadMessages = MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES
                .getOrFail<ArrayList<SingleMessageFacade>>()
        val expectedReadMessages = MessagesSerenityHelpers.EXPECTED_READ_MESSAGES
                .getOrFail<ArrayList<SingleMessageFacade>>()
        messagesPage.messages.assertRead(expectedReadMessages)
        messagesPage.messages.assertUnread(expectedUnreadMessages)
    }

    @Then("^my messages from the sender are displayed as read$")
    fun myMessagesFromTheSenderAreDisplayedAsRead() {
        val expectedMessages = MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER
                .getOrFail<SenderFacade>().messages
        val expectedReadMessages = arrayListOf<SingleMessageFacade>()
        expectedReadMessages.addAll(expectedMessages)
        messagesPage.messages.assertAllRead(expectedReadMessages)
    }

    @Then( "^an error is displayed indicating that there was a problem getting messages$")
    fun anErrorIsDisplayedIndicatingThatThereWasAProblemGettingMessages(){
        errorDialogPage.assertPageTitle("Messages error")
            .assertPageHeader("Messages error")
            .assertShutterParagraphText("There is a problem getting your messages.");
    }

    @Then( "^an error is displayed indicating that there was a problem getting a message$")
    fun anErrorIsDisplayedIndicatingThatThereWasAProblemGettingAMessage(){
        errorDialogPage.assertPageTitle("Message error")
            .assertPageHeader("Message error")
            .assertShutterParagraphText("There is a problem getting your message.");
    }

    @Then("^I click on the '(.*)' link in the message$")
    fun iClickOnTheNamedLinkInTheMessages(link: String){
        val linkTitle = Config.instance.url + link
        messagesPage.assertLinkExists(linkTitle,link, internal = true).click()
    }

    @Then("^the email address '(.*)' is identified as a link in the message$")
    fun theNamedEmailAddressIsIdentifiedAsALinkInTheMessage(email: String){
        val href = "mailto:$email"
        messagesPage.assertLinkExists(email,href, internal = false)
    }

    @Then("^the Message page is displayed$")
    fun theMessagePageIsDisplayed() {
        messagePage.assertDisplayed(MessagesSerenityHelpers.TARGET_SENDER.getOrFail())
    }

    private fun setupMessagesEnabledPatient(): MessagesFactory {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                arrayListOf(SJRJourneyType.MESSAGES_ENABLED, SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB))
        val factory = MessagesFactory()
        factory.setUpUser(patient)

        return factory
    }
}
