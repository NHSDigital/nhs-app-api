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
import pages.assertElementNotPresent
import pages.messages.MessagesErrorPage
import pages.messages.MessagesInboxPage
import pages.messages.MessagesPage
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade
import java.util.*
import kotlin.collections.ArrayList

private const val ACCESS_TOKEN_ABOUT_TO_EXPIRE_MILLISECONDS = 30000

class MessagesStepDefinitions {

    private lateinit var messagesPage: MessagesPage
    private lateinit var messagesInboxPage: MessagesInboxPage
    private lateinit var messagesErrorPage: MessagesErrorPage

    @Given("^I am a user wishing to view my messages$")
    fun iAmAUserWishingToViewTheirMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
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
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesWithContentInCache(table)
        val factoryAppointments = AppointmentsBookingFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        factoryAppointments.generateDefaultAvailableAppointmentSlotExample()
        factoryAppointments.generateSuccessfulBookingResponse()
    }

    @Given("^I am a user wishing to view my messages with content$")
    fun iAmAUserWishingToViewTheirMessagesWithContent(table: DataTable) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
        factory.setUpMultipleMessagesWithContentInCache(table)
    }

    @Given("^I am a user wishing to view my messages, but I have no messages$")
    fun iAmAUserWishingToViewTheirMessagesButIHaveNoMessages() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.MESSAGES_ENABLED)
        val factory = MessagesFactory()
        factory.setUpUser(patient)
    }

    @Given("^I am a user wishing to view my messages but retrieving the messages will cause an internal server error$")
    fun iAmAUserWishingToViewTheirMessagesButIHaveAnInvalidMessage() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.MESSAGES_ENABLED)
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

    @Then("^the Messages Inbox page is displayed$")
    fun theMessagesInboxPageIsDisplayed() {
        messagesInboxPage.assertHeaderDisplayed()
        messagesInboxPage.assertSubHeaderDisplayed()
    }

    @Then("^the Back link on the Messages Inbox page is displayed$")
    fun theBackLinkOnMessagesInboxPageIsDisplayed() {
        messagesInboxPage.assertBackLinkDisplayed()
    }

    @Then("^the Back link on the App Messages page is displayed$")
    fun theBackLinkOnAppMessagesPageIsDisplayed() {
        messagesPage.assertBackLinkDisplayed()
    }

    @Then("^the Back link is not shown on the Messages Inbox page$")
    fun theBackLinkOnMessagesInboxPageIsNotDisplayed() {
        messagesInboxPage.backLink.assertElementNotPresent()
    }

    @Then("^the Back link is not shown on the Messages page$")
    fun theBackLinkOnAppMessagesPageIsNotDisplayed() {
        messagesPage.backLink.assertElementNotPresent()
    }

    @Then("^I click on the Back link on the Messages Inbox page$")
    fun iClickOnTheBackLinkOnTheMessagesInboxPage() {
        messagesInboxPage.backLink.click()
    }

    @Then("^I click on the Back link on the App Messages page$")
    fun iClickOnTheBackLinkOnTheAppMessagesPage() {
        messagesPage.backLink.click()
    }

    @Then("^the senders and latest messages are displayed on the Messages Inbox page$")
    fun theSendersAndLatestMessagesAreDisplayedOnTheMessageInboxPage() {
        messagesInboxPage.messages.assertMessages(MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES.getOrFail())
    }

    @Then("^the viewed messages are marked as read on the Messages Inbox page$")
    fun theViewedMessagesAreMarkedAsReadOnTheMessageInboxPage() {
        messagesInboxPage.messages.assertMessages(
                MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES_AFTER_READING_SENDER.getOrFail())
    }

    @Then("^a message is displayed indicating that there are no messages in the Messages Inbox$")
    fun aMessageIsDisplayedIndicatingThatThereAreNoMessagesInTheMessagesInbox() {
        messagesInboxPage.assertNoMessages()
    }

    @Then("^the Messages page is displayed$")
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
}
