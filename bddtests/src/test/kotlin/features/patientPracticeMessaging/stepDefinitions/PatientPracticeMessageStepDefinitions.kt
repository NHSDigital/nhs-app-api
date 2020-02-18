package features.patientPracticeMessaging.stepDefinitions

import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.emis.models.PatientPracticeMessagingTypes
import mocking.emis.patientPracticeMessaging.MessageRecipientsResponseModel
import mocking.emis.practices.SettingsResponseModel
import models.ExpectedMessage
import org.junit.Assert.assertNotNull
import pages.ErrorPage
import pages.patientPracticeMessaging.PatientPracticeMessagingContactYourGpPage
import pages.patientPracticeMessaging.PatientPracticeMessagingDetailsPage
import pages.patientPracticeMessaging.PatientPracticeMessagingPage
import pages.patientPracticeMessaging.PatientPracticeMessagingRecipientsPage
import pages.patientPracticeMessaging.PatientPracticeMessagingUrgencyPage
import utils.SerenityHelpers
import mocking.emis.patientPracticeMessaging.MessageReply
import mocking.emis.patientPracticeMessaging.MessageResponseModel
import net.thucydides.core.annotations.Steps
import pages.patientPracticeMessaging.PatientPracticeMessagingDeletePage
import pages.patientPracticeMessaging.PatientPracticeMessagingDeleteSuccessPage
import pages.patientPracticeMessaging.PracticePatientMessagingCreateMessagePage
import worker.models.patientPracticeMessaging.CreateMessageRequest

const val RACE_CONDITION_WAIT: Long = 60

open class PatientPracticeMessageStepDefinitions {

    private lateinit var patientPracticeMessagingPage: PatientPracticeMessagingPage
    private lateinit var patientPracticeMessagingDetailsPage: PatientPracticeMessagingDetailsPage
    private lateinit var patientPracticeMessagingUrgencyPage: PatientPracticeMessagingUrgencyPage
    private lateinit var patientPracticeMessagingContactYourGpPage: PatientPracticeMessagingContactYourGpPage
    private lateinit var patientPracticeMessagingRecipientsPage: PatientPracticeMessagingRecipientsPage
    private lateinit var patientPracticeMessagingDeletePage: PatientPracticeMessagingDeletePage
    private lateinit var patientPracticeMessagingDeleteSuccessPage: PatientPracticeMessagingDeleteSuccessPage
    private lateinit var errorPage: ErrorPage

    private val expectedCareCardContent = arrayListOf(
            Pair("signs of a heart attack", "signs of a heart attack - pain like a very tight band, heavy weight or " +
                    "squeezing in the centre of your chest"),
            Pair("signs of a stroke", "signs of a stroke - face drooping on one side, cannot hold both arms up, " +
                    "difficulty speaking"),
            Pair("severe difficulty breathing", "severe difficulty breathing - gasping, not being able to get words " +
                    "out, choking or lips turning blue"),
            Pair("heavy bleeding", "heavy bleeding - that will not stop"),
            Pair("severe injuries", "severe injuries - or deep cuts after a serious accident"),
            Pair("seizure (fit)", "seizure (fit) - someone is shaking or jerking because of a fit, or is unconscious " +
                    "(cannot be woken up)"))
    private lateinit var patientPracticePatientMessagingCreateMessagePage: PracticePatientMessagingCreateMessagePage

    @Steps
    lateinit var browser: BrowserSteps

    var mockingClient = MockingClient.instance

    @Given("^I am a user who can access patient practice messaging$")
    fun iAmAUserWishingToViewTheirMessages() {
        val response = SettingsResponseModel()
        response.services.practicePatientCommunicationSupported = true

        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            practiceSettingsRequest(currentPatient)
                    .respondWithSuccess(response)
        }
    }

    @Given("^the Patient has no access to patient practice messaging$")
    fun thePatientHasNoAccessToPracticePatientMessaging() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .disabled(SerenityHelpers.getPatient())
    }

    @Given("^I have patient practice messages in my inbox, some of which are unread$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxWithUnreadMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient(), true)
    }

    @When("^I enter url address for the send message page$")
    fun whenIEnterTheUrlAddressForSendMessagePage() {
        val fullUrl = Config.instance.url + "/patient-practice-messaging/send-message"
        browser.browseTo(fullUrl)
    }


    @Given("^I have patient practice messages in my inbox$")
    fun thePatientHasPatientPracticeMessagesInTheirInbox() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient(), true)
    }

    @Given("^I have patient practice messages in my inbox, all of which are read$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxWithoutUnreadMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient(), false)
    }

    @Given("^The patient can successfully send a message to their practice")
    fun thePatientSuccessfullySendsAMessage() {
        val createMessageRequest = CreateMessageRequest("Test Results",
                "When will my test results be ready", "Recipient 1")
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .patientSuccessfullySendsAMessage(SerenityHelpers.getPatient(), createMessageRequest)

    }

    @Given("^I have no patient practice messages in my inbox$")
    fun thePatientHasNoPatientPracticeMessagesInTheirInbox() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .patientHasNoMessages(SerenityHelpers.getPatient())
    }

    @Given("^there is an unknown error getting patient practice messages$")
    fun givenThereIsAnUnknownErrorGettingPatientPracticeMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .unknownErrorWithPatientPracticeMessaging(SerenityHelpers.getPatient())
    }

    @Given("^there is a forbidden error getting patient practice messages$")
    fun givenThereIsAForbiddenErrorGettingPatientPracticeMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .forbiddenErrorWithPatientPracticeMessaging(SerenityHelpers.getPatient())
    }

    @Given("^The patient receives an error trying to send a message to their practice")
    fun thePatientReceivesAnErrorWhenTryingToSendAMessage() {
       val createMessageRequest = CreateMessageRequest("Test Results",
           "When will my test results be ready", "Recipient 1")
        PracticePatientMessagingFactory.getForSupplier(SerenityHelpers.getGpSupplier())
        .errorSendingAMessage(SerenityHelpers.getPatient(), createMessageRequest)
    }

    @Then("^I am on the send message page")
    fun iAmOnTheSendMessagePage() {
        val expectedRecipients = SerenityHelpers.getValueOrNull<MessageRecipientsResponseModel>(
                PatientPracticeMessagingTypes.AVAILABLE_RECIPIENTS)!!.MessageRecipients
        patientPracticePatientMessagingCreateMessagePage.assertHeaderContainsRecipient(expectedRecipients[0].name!!)
        patientPracticePatientMessagingCreateMessagePage.assertDisplayed()
    }

    @When("^I insert a subject and message")
    fun iInsertSubjectAndMessageText() {
        val messageDetails = SerenityHelpers
                .getValueOrNull<CreateMessageRequest>(PatientPracticeMessagingTypes.SENT_MESSAGE)!!
        patientPracticePatientMessagingCreateMessagePage.insertSubjectAndMessageText(
                messageDetails.subject,
                messageDetails.messageBody)
    }

    @When("^I click on a recipient$")
    fun iClickOnARecipient(){
        val expectedRecipients = SerenityHelpers.getValueOrNull<MessageRecipientsResponseModel>(
                PatientPracticeMessagingTypes.AVAILABLE_RECIPIENTS)!!.MessageRecipients
        patientPracticeMessagingRecipientsPage.clickRecipient(expectedRecipients)
    }

    @When("^I leave the message and subject fields blank")
    fun iDoNotInsertSubjectAndMessageText() {
        patientPracticePatientMessagingCreateMessagePage.insertSubjectAndMessageText("", "")
    }

    @When("^I click send message")
    fun iClickSendMessage() {
        patientPracticePatientMessagingCreateMessagePage.sendMessage()
    }

    @Then("I see validation errors for subject and message")
    fun iSeeValidationErrorsForSubjectAndMessage(){
        patientPracticePatientMessagingCreateMessagePage.assertValidationErrorsDisplayed()
    }

    @Given("^there is an unknown error getting patient practice message details$")
    fun givenThereIsAnErrorGettingPatientPracticeMessagesDetails() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .errorWithPatientPracticeMessagingMessageDetails(SerenityHelpers.getPatient())
    }

    @When("I have no recipients for patient practice messaging$")
    fun iHaveNoRecipients() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .noRecipients(SerenityHelpers.getPatient())
    }

    @And("^there is a bad request deleting the patient practice conversation$")
    fun givenThereIsABadRequestDeletingThePatientPracticeConversation() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .errorWithPatientPracticeMessagingConversationDelete(SerenityHelpers.getPatient())
    }

    @When("^I select a patient practice message in my inbox$")
    fun iSelectAPatientPracticeMessageInMyInbox() {
        clickMessageBySerenityVariable(PatientPracticeMessagingTypes.AVAILABLE_MESSAGE)
    }

    @When("^I click the Send a message button on the patient practice messaging inbox$")
    fun iClickSendMessageOnThePatientPracticeMessagingInbox() {
        patientPracticeMessagingPage.clickSendAMessageButton()
    }

    @When("^I click the Send a message button and " +
            "I choose that I need urgent advice via patient practice messaging$")
    fun iChooseThatINeedUrgentAdviceViaPatientPracticeMessaging() {
        patientPracticeMessagingPage.clickSendAMessageButton()
        patientPracticeMessagingUrgencyPage.chooseUrgentAndContinue()
    }

    @When("^I click the Send a message button and " +
            "I choose that I do not need urgent advice via patient practice messaging$")
    fun iChooseThatIDoNotNeedUrgentAdviceViaPatientPracticeMessaging() {
        patientPracticeMessagingPage.clickSendAMessageButton()
        patientPracticeMessagingUrgencyPage.chooseNonUrgentAndContinue()
    }

    @Then("^I see the patient practice messaging urgency contact your gp page$")
    fun iSeeThePatientPracticeMessagingUrgencyContactYourGpPage() {
        patientPracticeMessagingContactYourGpPage.assertIsDisplayed()
    }

    @Then("^I see the patient practice messaging recipients page$")
    fun iSeeThePatientPracticeMessagingUrgencyRecipientsPage() {
        patientPracticeMessagingRecipientsPage.assertIsDisplayed()
    }

    @Then("^I see a message indicating that I have no recipients for patient practice messaging$")
    fun iSeeThePatientPracticeMessagingUrgencyRecipientsPageWithNoRecipientsMessage() {
        patientPracticeMessagingUrgencyPage.assertNoRecipientsHeader()
        patientPracticeMessagingUrgencyPage.assertNoRecipientsMessage()
    }

    @Then("^I see a message explaining patient practice messaging is not for urgent advice$")
    fun iSeeAMessageExplainingPatientPracticeMessagingIsNotForUrgentAdvice() {
        patientPracticeMessagingContactYourGpPage.assertMessagingPurposeText()
        patientPracticeMessagingContactYourGpPage.assertWhatToDoNextText()
        patientPracticeMessagingContactYourGpPage.assertCareCardContent(expectedCareCardContent)
    }

    @Then("^the patient to practice inbox page is displayed$")
    fun thePatientToPracticePageisDisplayed() {
        patientPracticeMessagingPage.assertDisplayed()
    }

    @Then("^I see a list of patient practice messages$")
    fun iSeeAListOfPatientPracticeMessages() {
        patientPracticeMessagingPage
                .assertCorrectMessagesDisplayed(
                        SerenityHelpers.getValueOrNull<List<ExpectedMessage>>(
                                PatientPracticeMessagingTypes.EXPECTED_MESSAGES)!!)
    }

    @Then("^I see a message indicating that I have no patient practice messages$")
    fun theMessageIndicatingNoPatientPracticeMessagesIsDisplayed() {
        patientPracticeMessagingPage.assertNoMessagesTextDisplayed()
    }

    @Then("^I see the appropriate forbidden error for patient practice messaging$")
    fun iSeeTheAppropriateForbiddenErrorForPatientPracticeMessaging(){
        errorPage.assertPageHeader("Service unavailable")
        errorPage.assertNoSubHeader()
        errorPage.assertHeaderText("You are not currently able to use messaging.")
        errorPage.assertMessageText("Contact your GP surgery for more information. For urgent medical advice, " +
                "go to 111.nhs.uk or call 111.")
    }

    @Then("^I see the appropriate error for (.*) patient practice message\\(s\\)$")
    fun iSeeTheAppropriateErrorForPatientPracticeMessage(action: String){
        when (action) {
            "deleting" -> {
                errorPage.assertPageHeader("Error deleting conversation")
                errorPage.assertNoSubHeader()
                errorPage.assertHeaderText("Sorry, we could not delete your conversation")
                errorPage.assertMessageText("Try again now.")
                errorPage.assertHasButton("Try again")
            }
            "getting" -> {
                errorPage.assertPageHeader("Message Error")
                errorPage.assertNoSubHeader()
                errorPage.assertHeaderText("There is a problem getting your message")
                errorPage.assertMessageText("Try again now. If the problem " +
                        "continues and you need this information now, " +
                        "contact the person directly.")
                errorPage.assertHasButton("Try again")
            }
            "listing" -> {
                errorPage.assertPageHeader("Messages Error")
                errorPage.assertNoSubHeader()
                errorPage.assertHeaderText("There is a problem getting your messages")
                errorPage.assertMessageText("Try again now.")
                errorPage.assertHasButton("Try again")
            }
        }


    }

    @Then("^I see my new message after it has been sent")
    fun iSeeMyNewMessageAfterItHasBeenSent(){
        val messageDetails = SerenityHelpers
                .getValueOrNull<CreateMessageRequest>(PatientPracticeMessagingTypes.SENT_MESSAGE)!!
        patientPracticeMessagingDetailsPage.assertSentSubjectCorrect(messageDetails.subject)
        patientPracticeMessagingDetailsPage.assertSentMessageCorrect(messageDetails.messageBody)
    }

    @Then("^I see my patient practice message along with the replies from the GP")
    fun iSeeMyPatientPracticeMessageAlongWithTheReplies() {
        val message = SerenityHelpers.getValueOrNull<MessageResponseModel>(
                PatientPracticeMessagingTypes.SELECTED_MESSAGE)!!.Message
        val replies = message.messageReplies
        val expectedSentMessageDate = SerenityHelpers.getValueOrNull<String>(
                PatientPracticeMessagingTypes.EXPECTED_MESSAGE_SENT_DATE)!!
        val expectedReadMessageReplyDates = SerenityHelpers.getValueOrNull<List<String>>(
                PatientPracticeMessagingTypes.EXPECTED_READ_MESSAGE_REPLY_DATES)
        val expectedUnreadMessageReplyDates = SerenityHelpers.getValueOrNull<List<String>>(
                PatientPracticeMessagingTypes.EXPECTED_UNREAD_MESSAGE_REPLY_DATES)

        val unreadReplies = mutableListOf<MessageReply>()
        val readReplies = mutableListOf<MessageReply>()
        replies.filterTo(unreadReplies, {reply: MessageReply -> reply.isUnread})
        replies.filterTo(readReplies, {reply: MessageReply -> !reply.isUnread})
        patientPracticeMessagingDetailsPage.assertReadRepliesCorrect(readReplies)
        patientPracticeMessagingDetailsPage.assertUnreadRepliesCorrect(unreadReplies)
        patientPracticeMessagingDetailsPage.assertSentMessageCorrect(message.content)
        patientPracticeMessagingDetailsPage.assertSentDateTimeCorrect(expectedSentMessageDate)
        patientPracticeMessagingDetailsPage.assertSentSubjectCorrect(message.subject)
        if (unreadReplies.count() > 1) {
            patientPracticeMessagingDetailsPage.assertReceivedDateTimesCorrect(expectedUnreadMessageReplyDates!!, false)
            patientPracticeMessagingDetailsPage.assertUnreadDividerIsOnSceen()
        } else {
            patientPracticeMessagingDetailsPage.assertReceivedDateTimesCorrect(expectedReadMessageReplyDates!!, true)
        }
    }

    @Then("^I see a list of patient practice messaging recipients$")
    fun iSeeAListOfPatientPracticeMessagingRecipients() {
        val expectedRecipients = SerenityHelpers.getValueOrNull<MessageRecipientsResponseModel>(
                PatientPracticeMessagingTypes.AVAILABLE_RECIPIENTS)!!.MessageRecipients
        patientPracticeMessagingRecipientsPage.assertInfoText()
        patientPracticeMessagingRecipientsPage.assertRecipients(expectedRecipients)
    }

    @When("^I select delete conversation on the view conversation page$")
    fun iClickOnDeleteConversationFromViewDetailsScreen(){
        patientPracticeMessagingDetailsPage.clickDeleteConversation()
    }

    @Then("^I am prompted to confirm my intention to delete the conversation$")
    fun iSeeTheDeleteInfoPage(){
        patientPracticeMessagingDeletePage.assertDisplayed()
    }

    @When("^I click delete conversation on the delete page to confirm my decision$")
    fun iClickDeleteConversationOnDeletePage() {
        patientPracticeMessagingDeletePage.clickDeleteConversation()
    }

    @Then("^I see a page indicating my patient practice message has been deleted$")
    fun iSeeTheDeleteSuccessPage(){
        //This wait has been added to ensure race condition does not occur
        Thread.sleep(RACE_CONDITION_WAIT)
        patientPracticeMessagingDeleteSuccessPage.assertDisplayed()
    }

    @When("^I click go back to patient practice messages$")
    fun iClickToGoBackToPatientPracticeMessages() {
        patientPracticeMessagingDeleteSuccessPage.clickBackToMessages()
    }

    private fun clickMessageBySerenityVariable(messageToClick: PatientPracticeMessagingTypes) {
        assertNotNull("Expected the value for the serenity variable 'messageToClick' to not be null",
                SerenityHelpers.getValueOrNull<MessageResponseModel>(messageToClick)!!)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.SELECTED_MESSAGE,
                SerenityHelpers.getValueOrNull<MessageResponseModel>(messageToClick)!!)
        patientPracticeMessagingPage.clickFirstMessage()
    }
}