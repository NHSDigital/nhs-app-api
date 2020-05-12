package features.patientPracticeMessaging.stepDefinitions

import config.Config
import constants.Supplier
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.BrowserSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.MockingClient
import mocking.patientPracticeMessaging.MessageReply
import mocking.patientPracticeMessaging.MessageResponseModel
import mocking.patientPracticeMessaging.Recipient
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import models.ExpectedMessage
import org.junit.Assert.assertNotNull
import pages.ErrorPage
import pages.patientPracticeMessaging.PatientPracticeMessagingContactYourGpPage
import pages.patientPracticeMessaging.PatientPracticeMessagingDetailsPage
import pages.patientPracticeMessaging.PatientPracticeMessagingPage
import pages.patientPracticeMessaging.PatientPracticeMessagingRecipientsPage
import pages.patientPracticeMessaging.PatientPracticeMessagingUrgencyPage
import utils.SerenityHelpers
import net.thucydides.core.annotations.Steps
import pages.assertIsVisible
import pages.patientPracticeMessaging.PatientPracticeDownloadAttachmentPage
import pages.patientPracticeMessaging.PatientPracticeMessagingDeletePage
import pages.patientPracticeMessaging.PatientPracticeMessagingDeleteSuccessPage
import pages.patientPracticeMessaging.PracticePatientMessagingCreateMessagePage
import utils.getOrNull
import utils.setIfNotAlreadySet
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
    private lateinit var patientPracticeMessagingDownloadAttachmentPage: PatientPracticeDownloadAttachmentPage
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

    @Given("^I am an? (.*) user who can access patient practice messaging$")
    fun iAmAUserWishingToViewTheirMessages(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                supplier,
                arrayListOf(
                        SJRJourneyType.IM1_MESSAGING_ENABLED,
                        SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_ENABLED),
                null)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)

        PracticePatientMessagingFactory
                .getForSupplier(supplier)
                .enabled(patient)
    }

    @Given("^I am a user who can access patient practice messaging$")
    fun iAmAUserWishingToViewTheirMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabled(SerenityHelpers.getPatient())
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

    @Given("^I have patient practice messages in my inbox, some of which are unread with an attachment$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxWithUnreadMessagesWithAnAttachment() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient(),
                        hasUnread = true,
                        hasAttachment = true)
    }

    @Given("^I want to send a message to a (.*) recipient and have unread messages in my inbox$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxAndSendsToAUnitRecipient(recipientType: String) {
        val unitRecipient = recipientType == "unit"
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient(),
                        hasUnread = true,
                        unitRecipient = unitRecipient)
    }


    @Given("^that attachment is invalid$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxWithUnreadMessagesWithInvalidAttachment() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithInvalidAttachmentOnMessage(SerenityHelpers.getPatient())
    }

    @When("^I enter url address for the send message page$")
    fun whenIEnterTheUrlAddressForSendMessagePage() {
        val fullUrl = Config.instance.url + "/messages/gp-messages/send-message"
        browser.browseTo(fullUrl)
    }

    @When("^I click on the view link$")
    fun iClickOnTheViewLink() {
        patientPracticeMessagingDetailsPage.clickLink("viewLink")
    }

    @When("^I click on the download link$")
    fun iClickOnTheDownloadLink() {
        patientPracticeMessagingDetailsPage.clickLink("downloadLink")
    }

    @When("^I click on the download button$")
    fun iClickOnTheDownloadButton() {
        patientPracticeMessagingDownloadAttachmentPage.downloadButtonClicked()
    }

    @Then("^I see the download information page$")
    fun iSeeTheDownloadInformationPage() {
        patientPracticeMessagingDownloadAttachmentPage.assertDownloadButtonDisplayed()
        patientPracticeMessagingDownloadAttachmentPage.assertInformationParagraph()
    }

    @Then("^the attachment has been downloaded$")
    fun attachmentHasBeenDownloaded() {
        patientPracticeMessagingDownloadAttachmentPage.hasAttachmentDownloaded("Attachment_14 April 2020")
    }

    @Then("^I see the invalid attachment message$")
    fun iSeeInvalidAttachmentMessage() {
        patientPracticeMessagingDownloadAttachmentPage.assertInvalidMessage()
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

    @Given("^I have patient practice messages in my inbox, one of which came from the GP$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxWithMessageFromGp() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessagingFromGP(SerenityHelpers.getPatient(), false)
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
        val expectedRecipients = PatientPracticeMessagingSerenityHelpers
                .AVAILABLE_RECIPIENTS
                .getOrNull<List<Recipient>>()!!
        patientPracticePatientMessagingCreateMessagePage.assertHeaderContainsRecipient(expectedRecipients[0].name!!)

        when(SerenityHelpers.getGpSupplier()){
            Supplier.EMIS -> {
                patientPracticePatientMessagingCreateMessagePage.assertDisplayed(true)
            }
            Supplier.TPP -> {
                patientPracticePatientMessagingCreateMessagePage.assertDisplayed(false)
            }
            else -> throw NotImplementedError()
        }
    }

    @When("^I insert a subject")
    fun iInsertSubjectText() {
        val subject = PatientPracticeMessagingSerenityHelpers.SENT_MESSAGE.getOrNull<CreateMessageRequest>()!!.subject
        patientPracticePatientMessagingCreateMessagePage.insertSubjectText(
                subject!!)
    }

    @When("^I insert a message")
    fun iInsertMessageText() {
        val messageBody = PatientPracticeMessagingSerenityHelpers.SENT_MESSAGE.getOrNull<CreateMessageRequest>()!!
                .messageBody
        patientPracticePatientMessagingCreateMessagePage.insertMessageText(messageBody)
    }

    @When("^I click on a (.*) recipient$")
    fun iClickOnARecipient(recipientType: String){
        val expectedRecipients = PatientPracticeMessagingSerenityHelpers
                .AVAILABLE_RECIPIENTS
                .getOrNull<List<Recipient>>()!!

        when(recipientType){
            "regular" -> {
                patientPracticeMessagingRecipientsPage.clickRecipient(expectedRecipients)
            }
            "unit" -> {
                patientPracticeMessagingRecipientsPage.clickRecipient(expectedRecipients, true)
            }
        }
    }


    @When("^I leave the message and subject fields blank")
    fun iDoNotInsertSubjectAndMessageText() {
        patientPracticePatientMessagingCreateMessagePage.insertMessageText("")

        if (SerenityHelpers.getGpSupplier() == Supplier.EMIS) {
            patientPracticePatientMessagingCreateMessagePage.insertSubjectText("")
        }
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
        clickMessageBySerenityVariable(PatientPracticeMessagingSerenityHelpers.AVAILABLE_MESSAGE)
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

    @Then("^I click the send message link on the message details page and I do not need urgent advice")
    fun iClickTheLinkOnThePatientPracticeMessageDetailsPage() {
        patientPracticeMessagingDetailsPage.clickLink("newMessage")
        patientPracticeMessagingUrgencyPage.chooseNonUrgentAndContinue()
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
                        PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES.getOrNull<List<ExpectedMessage>>()!!)
    }

    @Then("^I see a list of patient practice messages without the subject and with the unread count$")
    fun iSeeAListOfPatientPracticeMessagesWithNoSubjectWithUnreadCount() {
        patientPracticeMessagingPage
                .assertCorrectMessagesDisplayed(
                        PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES.getOrNull<List<ExpectedMessage>>()
                        !!, hasSubject = false, hasUnreadCount = true, fromGP = false)
    }

    @Then("^I see a list of patient practice messages without the subject and without the unread count$")
    fun iSeeAListOfPatientPracticeMessagesWithNoSubjectNoUnreadCount() {
        patientPracticeMessagingPage
                .assertCorrectMessagesDisplayed(
                        PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES.getOrNull<List<ExpectedMessage>>()
                        !!, hasSubject = false, hasUnreadCount = false, fromGP = false)
    }

    @Then("^I see a list of patient practice messages from the GP$")
    fun iSeeAListOfPatientPracticeMessagesWithNoSubjectFromTheGp() {
        patientPracticeMessagingPage
                .assertCorrectMessagesDisplayed(
                        PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES.getOrNull<List<ExpectedMessage>>()
                        !!, hasSubject = false, hasUnreadCount = false, fromGP = true)
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
    fun iSeeTheAppropriateErrorForPatientPracticeMessage(action: String) {
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
    fun iSeeMyNewMessageAfterItHasBeenSent() {
        val messageDetails = PatientPracticeMessagingSerenityHelpers.SENT_MESSAGE.getOrNull<CreateMessageRequest>()!!

        if (SerenityHelpers.getGpSupplier() == Supplier.EMIS) {
            patientPracticeMessagingDetailsPage.assertSentSubjectCorrect(messageDetails.subject!!)
        }

        patientPracticeMessagingDetailsPage.assertMessageCorrect(messageDetails.messageBody, "initialMessageSentPanel0")
    }

    @Then("^I can view the message attachment")
    fun iSeeTheMessageAttachment() {
        patientPracticeMessagingDetailsPage.attachment.assertIsVisible()
    }

    @Then("^I see my patient practice message along with the replies from the GP")
    fun iSeeMyPatientPracticeMessageAlongWithTheReplies() {
        val message = PatientPracticeMessagingSerenityHelpers
                .SELECTED_MESSAGE
                .getOrNull<MessageResponseModel>()!!
        val replies = message.Message.messageReplies
        val expectedSentMessageDate = PatientPracticeMessagingSerenityHelpers
                .EXPECTED_MESSAGE_SENT_DATE
                .getOrNull<String>()!!
        val expectedReadMessageReplyDates = PatientPracticeMessagingSerenityHelpers
                .EXPECTED_READ_MESSAGE_REPLY_DATES
                .getOrNull<List<String>>()
        val expectedUnreadMessageReplyDates = PatientPracticeMessagingSerenityHelpers
                .EXPECTED_UNREAD_MESSAGE_REPLY_DATES
                .getOrNull<List<String>>()

        val unreadReplies = mutableListOf<MessageReply>()
        val readReplies = mutableListOf<MessageReply>()

        replies.filterTo(unreadReplies, {reply: MessageReply -> reply.isUnread!!})
        replies.filterTo(readReplies, {reply: MessageReply -> !reply.isUnread!!})

        patientPracticeMessagingDetailsPage.assertReadRepliesCorrect(readReplies)
        patientPracticeMessagingDetailsPage.assertUnreadRepliesCorrect(unreadReplies)

        if (!PatientPracticeMessagingSerenityHelpers.INITIAL_FROM_GP.getOrNull<Boolean>()!!) {
            patientPracticeMessagingDetailsPage.assertMessageCorrect(
                    message.Message.content,
                    "initialMessageSentPanel0")
            patientPracticeMessagingDetailsPage.assertDateTimeCorrect(
                    expectedSentMessageDate,
                    "initialMessageSentDateTime0")
        } else {
            patientPracticeMessagingDetailsPage.assertMessageCorrect(
                    message.Message.content,
                    "initialMessageReplyPanel0")
            patientPracticeMessagingDetailsPage.assertDateTimeCorrect(
                    expectedSentMessageDate,
                    "initialMessageReplyDateTime0")
        }
        if (message.Message.subject !== null) {
            patientPracticeMessagingDetailsPage.assertSentSubjectCorrect(message.Message.subject!!)
        }

        if (unreadReplies.count() > 1) {
            patientPracticeMessagingDetailsPage.assertReceivedDateTimesCorrect(
                    expectedUnreadMessageReplyDates!!, false)
            patientPracticeMessagingDetailsPage.assertUnreadDividerIsOnSceen()
        } else if (readReplies.count() >= 1) {
            patientPracticeMessagingDetailsPage.assertReceivedDateTimesCorrect(
                    expectedReadMessageReplyDates!!, true)
        }
    }

    @Then("^I see the view and download links on the message$")
    fun iSeeTheViewAndDownloadLinksOnTheMessage() {
        patientPracticeMessagingDetailsPage.assertLink("downloadLink")
        patientPracticeMessagingDetailsPage.assertLink("viewLink")
    }

    @Then("^I see a list of patient practice messaging recipients$")
    fun iSeeAListOfPatientPracticeMessagingRecipients() {
        val expectedRecipients = PatientPracticeMessagingSerenityHelpers.AVAILABLE_RECIPIENTS
                .getOrNull<List<Recipient>>()!!
        patientPracticeMessagingRecipientsPage.assertInfoText()
        patientPracticeMessagingRecipientsPage.assertRecipients(expectedRecipients)
    }

    @When("^I select delete conversation on the view conversation page$")
    fun iClickOnDeleteConversationFromViewDetailsScreen() {
        patientPracticeMessagingDetailsPage.clickDeleteConversation()
    }

    @Then("^I am prompted to confirm my intention to delete the conversation$")
    fun iSeeTheDeleteInfoPage() {
        patientPracticeMessagingDeletePage.assertDisplayed()
    }

    @When("^I click delete conversation on the delete page to confirm my decision$")
    fun iClickDeleteConversationOnDeletePage() {
        patientPracticeMessagingDeletePage.clickDeleteConversation()
    }

    @Then("^I see a page indicating my patient practice message has been deleted$")
    fun iSeeTheDeleteSuccessPage() {
        Thread.sleep(RACE_CONDITION_WAIT)
        patientPracticeMessagingDeleteSuccessPage.assertDisplayed()
    }

    @When("^I click go back to patient practice messages$")
    fun iClickToGoBackToPatientPracticeMessages() {
        patientPracticeMessagingDeleteSuccessPage.clickBackToMessages()
    }

    @Then("^the message is marked as read$")
    fun theMessageIsMarkedAsRead() =
            mockingClient.assertRequestWasMade(
                    "/tpp/",
                    headers = mapOf("type" to "MessageMarkAsRead"))

    private fun clickMessageBySerenityVariable(messageToClick: PatientPracticeMessagingSerenityHelpers) {
        assertNotNull("Expected the value for the serenity variable 'messageToClick' to not be null",
                SerenityHelpers.getValueOrNull<MessageResponseModel>(messageToClick)!!)
        PatientPracticeMessagingSerenityHelpers
                .SELECTED_MESSAGE
                .setIfNotAlreadySet(SerenityHelpers.getValueOrNull<MessageResponseModel>(messageToClick)!!)
        patientPracticeMessagingPage.clickFirstMessage()
    }
}
