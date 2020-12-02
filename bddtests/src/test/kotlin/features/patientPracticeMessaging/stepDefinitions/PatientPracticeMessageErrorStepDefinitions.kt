package features.patientPracticeMessaging.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import pages.ErrorPage
import utils.SerenityHelpers

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

    @Then("^I click try again")
    fun iClickTryAgain() {
        errorPage.clickButton()
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
                errorPage.assertPageHeader("Message error")
                        .assertNoSubHeader()
                        .assertHeaderText("There is a problem getting your message")
                        .assertMessageText("Try again now. If the problem " +
                                "continues and you need this information now, " +
                                "contact the person directly.")
                        .assertHasButton("Try again")
            }
            "listing" -> {
                errorPage.assertPageHeader("Messages error")
                        .assertNoSubHeader()
                        .assertHeaderText("There is a problem getting your messages")
                        .assertMessageText("Try again now.")
                        .assertHasButton("Try again")
            }
        }
    }
}
