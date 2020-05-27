package features.patientPracticeMessaging.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import pages.ErrorPage
import utils.SerenityHelpers
import worker.models.patientPracticeMessaging.CreateMessageRequest

class PatientPracticeMessageErrorStepDefinitions {

    private lateinit var errorPage: ErrorPage

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

    @Given("^there is an unknown error getting patient practice message details$")
    fun givenThereIsAnErrorGettingPatientPracticeMessagesDetails() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .errorWithPatientPracticeMessagingMessageDetails(SerenityHelpers.getPatient())
    }

    @Given("^there is a bad request deleting the patient practice conversation$")
    fun givenThereIsABadRequestDeletingThePatientPracticeConversation() {
        PracticePatientMessagingFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .errorWithPatientPracticeMessagingConversationDelete(SerenityHelpers.getPatient())
    }

    @Then("^I see the appropriate forbidden error for patient practice messaging$")
    fun iSeeTheAppropriateForbiddenErrorForPatientPracticeMessaging() {
        errorPage.assertPageHeader("Messaging unavailable")
                .assertNoSubHeader()
                .assertHeaderText("You are not currently able to use messaging.")
                .assertMessageText("Contact your GP surgery for more information. For urgent medical advice, " +
                        "go to 111.nhs.uk or call 111.")
    }

    @Then("^I see the appropriate error for (.*) patient practice message\\(s\\)$")
    fun iSeeTheAppropriateErrorForPatientPracticeMessage(action: String) {
        when (action) {
            "deleting" -> {
                errorPage.assertPageHeader("Error deleting conversation")
                        .assertNoSubHeader()
                        .assertHeaderText("Sorry, we could not delete your conversation")
                        .assertMessageText("Try again now.")
                        .assertHasButton("Try again")
            }
            "getting" -> {
                errorPage.assertPageHeader("Message Error")
                        .assertNoSubHeader()
                        .assertHeaderText("There is a problem getting your message")
                        .assertMessageText("Try again now. If the problem " +
                                "continues and you need this information now, " +
                                "contact the person directly.")
                        .assertHasButton("Try again")
            }
            "listing" -> {
                errorPage.assertPageHeader("Messages Error")
                        .assertNoSubHeader()
                        .assertHeaderText("There is a problem getting your messages")
                        .assertMessageText("Try again now.")
                        .assertHasButton("Try again")
            }
        }
    }
}
