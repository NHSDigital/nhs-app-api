package features.patientPracticeMessaging.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import mocking.emis.practices.SettingsResponseModel
import models.ExpectedMessage
import pages.patientPracticeMessaging.PatientPracticeMessagingDetailsPage
import pages.patientPracticeMessaging.PatientPracticeMessagingPage
import utils.SerenityHelpers
import mocking.MockingClient
import mocking.emis.models.PatientPracticeMessagingMessageTypes
import mocking.emis.patientPracticeMessaging.MessageResponseModel
import org.junit.Assert.assertNotNull
import pages.ErrorPage

open class PatientPracticeMessageStepDefinitions {

    private lateinit var patientPracticeMessagingPage: PatientPracticeMessagingPage
    private lateinit var patientPracticeMessagingDetailsPage: PatientPracticeMessagingDetailsPage
    private lateinit var errorPage: ErrorPage

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

    @Given("^I have patient practice messages in my inbox$")
    fun thePatientHasPatientPracticeMessagesInTheirInbox() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithPatientPracticeMessaging(SerenityHelpers.getPatient())
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
        clickMessageBySerenityVariable(PatientPracticeMessagingMessageTypes.AVAILABLE_MESSAGE)
    }

    @Then("I see a list of patient practice messages$")
    fun iSeeAListOfPatientPracticeMessages() {
        patientPracticeMessagingPage
                .assertCorrectMessagesDisplayed(
                        SerenityHelpers.getValueOrNull<List<ExpectedMessage>>(
                                PatientPracticeMessagingMessageTypes.EXPECTED_MESSAGES)!!)
    }

    @Then("^the patient to practice inbox page is displayed")
    fun thePatientToPracticePageisDisplayed() {
        patientPracticeMessagingPage.assertDisplayed()
    }

    @Then("I see a message indicating that I have no patient practice messages")
    fun theMessageIndicatingNoPatientPracticeMessagesIsDisplayed() {
        patientPracticeMessagingPage.assertNoMessagesTextDisplayed()
    }

    @Then("I see the appropriate error for patient practice messaging")
    fun iSeeTheAppropriateErrorForPatientPracticeMessaging(){
        errorPage.assertPageHeader("Messages Error")
        errorPage.assertNoSubHeader()
        errorPage.assertHeaderText("There is a problem getting your messages")
        errorPage.assertMessageText("Try again now.")
        errorPage.assertHasButton("Try again")
    }

    @Then("I see the appropriate error for patient practice message")
    fun iSeeTheAppropriateErrorForPatientPracticeMessage(){
        errorPage.assertPageHeader("Message Error")
        errorPage.assertNoSubHeader()
        errorPage.assertHeaderText("There is a problem getting your message")
        errorPage.assertMessageText("Try again now. If the problem continues and you need this information now, " +
                "contact the person directly.")
        errorPage.assertHasButton("Try again")
    }

    @Then("^I see my patient practice message along with the replies from the GP")
    fun iSeeMyPatientPracticeMessageAlongWithTheReplies() {
        val message = SerenityHelpers.getValueOrNull<MessageResponseModel>(
                PatientPracticeMessagingMessageTypes.SELECTED_MESSAGE)!!.Message
        val replies = message.messageReplies
        patientPracticeMessagingDetailsPage.assertRepliesCorrect(replies)
        patientPracticeMessagingDetailsPage.assertSentMessageCorrect(message)
        patientPracticeMessagingDetailsPage.assertSentDateTimeCorrect()
        patientPracticeMessagingDetailsPage.assertReceivedDateTimeCorrect()
        patientPracticeMessagingDetailsPage.assertSentSubjectCorrect(message)
    }

    private fun clickMessageBySerenityVariable(messageToClick: PatientPracticeMessagingMessageTypes) {
        assertNotNull("Expected the value for the serentity variable 'messageToClick' to not be null",
                SerenityHelpers.getValueOrNull<MessageResponseModel>(messageToClick)!!)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingMessageTypes.SELECTED_MESSAGE,
                SerenityHelpers.getValueOrNull<MessageResponseModel>(messageToClick)!!)
        patientPracticeMessagingPage.clickFirstMessage()
    }
}