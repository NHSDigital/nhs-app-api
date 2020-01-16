package features.patientPracticeMessaging.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import mocking.MockingClient
import mocking.emis.models.PatientPracticeMessagingTypes
import mocking.emis.patientPracticeMessaging.MessageRecipientsResponseModel
import mocking.emis.patientPracticeMessaging.MessageResponseModel
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

open class PatientPracticeMessageStepDefinitions {

    private lateinit var patientPracticeMessagingPage: PatientPracticeMessagingPage
    private lateinit var patientPracticeMessagingDetailsPage: PatientPracticeMessagingDetailsPage
    private lateinit var patientPracticeMessagingUrgencyPage: PatientPracticeMessagingUrgencyPage
    private lateinit var patientPracticeMessagingContactYourGpPage: PatientPracticeMessagingContactYourGpPage
    private lateinit var patientPracticeMessagingRecipientsPage: PatientPracticeMessagingRecipientsPage
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

    @Given("^I have patient practice messages in my inbox, all of which are read$")
    fun thePatientHasPatientPracticeMessagesInTheirInboxWithoutUnreadMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient(), false)
    }

    @Given("^I have no patient practice messages in my inbox$")
    fun thePatientHasNoPatientPracticeMessagesInTheirInbox() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .patientHasNoMessages(SerenityHelpers.getPatient())
    }

    @Given("^there is an unknown error getting patient practice messages$")
    fun givenThereIsAnErrorGettingPatientPracticeMessages() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .errorWithPatientPracticeMessaging(SerenityHelpers.getPatient())
    }

    @Given("^there is an unknown error getting patient practice message details$")
    fun givenThereIsAnErrorGettingPatientPracticeMessagesDetails() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .errorWithPatientPracticeMessagingMessageDetails(SerenityHelpers.getPatient())
    }

    @When("^I select a patient practice message in my inbox$")
    fun iSelectAPatientPracticeMessageInMyInbox() {
        clickMessageBySerenityVariable(PatientPracticeMessagingTypes.AVAILABLE_MESSAGE)
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

    @Then("^I see a message explaining patient practice messaging is not for urgent advice$")
    fun iSeeAMessageExplainingPatientPracticeMessagingIsNotForUrgentAdvice() {
        patientPracticeMessagingContactYourGpPage.assertMessagingPurposeText()
        patientPracticeMessagingContactYourGpPage.assertWhatToDoNextText()
        patientPracticeMessagingContactYourGpPage.assertCareCardContent(expectedCareCardContent)
    }

    @Then("^I see a list of patient practice messages$")
    fun iSeeAListOfPatientPracticeMessages() {
        patientPracticeMessagingPage
                .assertCorrectMessagesDisplayed(
                        SerenityHelpers.getValueOrNull<List<ExpectedMessage>>(
                                PatientPracticeMessagingTypes.EXPECTED_MESSAGES)!!)
    }

    @Then("^the patient to practice inbox page is displayed$")
    fun thePatientToPracticePageisDisplayed() {
        patientPracticeMessagingPage.assertDisplayed()
    }

    @Then("^I see a message indicating that I have no patient practice messages$")
    fun theMessageIndicatingNoPatientPracticeMessagesIsDisplayed() {
        patientPracticeMessagingPage.assertNoMessagesTextDisplayed()
    }

    @Then("^I see the appropriate error for patient practice messaging$")
    fun iSeeTheAppropriateErrorForPatientPracticeMessaging(){
        errorPage.assertPageHeader("Messages Error")
        errorPage.assertNoSubHeader()
        errorPage.assertHeaderText("There is a problem getting your messages")
        errorPage.assertMessageText("Try again now.")
        errorPage.assertHasButton("Try again")
    }

    @Then("^I see the appropriate error for getting patient practice message details$")
    fun iSeeTheAppropriateErrorForGettingPatientPracticeMessageDetails(){
        errorPage.assertPageHeader("Message Error")
        errorPage.assertNoSubHeader()
        errorPage.assertHeaderText("There is a problem getting your message")
        errorPage.assertMessageText("Try again now. If the problem continues and you need this information now, " +
                "contact the person directly.")
        errorPage.assertHasButton("Try again")
    }

    @Then("^I see my patient practice message along with the replies from the GP$")
    fun iSeeMyPatientPracticeMessageAlongWithTheReplies() {
        val message = SerenityHelpers.getValueOrNull<MessageResponseModel>(
                PatientPracticeMessagingTypes.SELECTED_MESSAGE)!!.Message
        val replies = message.messageReplies
        val unreadReplies = mutableListOf<MessageReply>()
        val readReplies = mutableListOf<MessageReply>()
        replies.filterTo(unreadReplies, {reply: MessageReply -> reply.isUnread})
        replies.filterTo(readReplies, {reply: MessageReply -> !reply.isUnread})
        patientPracticeMessagingDetailsPage.assertReadRepliesCorrect(readReplies)
        patientPracticeMessagingDetailsPage.assertUnreadRepliesCorrect(unreadReplies)
        patientPracticeMessagingDetailsPage.assertSentMessageCorrect(message)
        patientPracticeMessagingDetailsPage.assertSentDateTimeCorrect()
        patientPracticeMessagingDetailsPage.assertSentSubjectCorrect(message)
        if (unreadReplies.count() > 1) {
            patientPracticeMessagingDetailsPage.assertUnreadReceivedDateTimeCorrect()
            patientPracticeMessagingDetailsPage.assertUnreadDividerIsOnSceen()
        } else {
            patientPracticeMessagingDetailsPage.assertReadReceivedDateTimeCorrect()
        }
    }

    @Then("^I see a list of patient practice messaging recipients$")
    fun iSeeAListOfPatientPracticeMessagingRecipients() {
        val expectedRecipients = SerenityHelpers.getValueOrNull<MessageRecipientsResponseModel>(
                PatientPracticeMessagingTypes.AVAILABLE_RECIPIENTS)!!.MessageRecipients
        patientPracticeMessagingRecipientsPage.assertInfoText()
        patientPracticeMessagingRecipientsPage.assertRecipients(expectedRecipients)
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