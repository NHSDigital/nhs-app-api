package features.patientPracticeMessaging.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.patientPracticeMessaging.factories.PracticePatientMessagingFactory
import pages.ErrorDialogPage
import pages.ErrorPage
import utils.SerenityHelpers

class PatientPracticeMessageErrorStepDefinitions {

    private lateinit var errorPage: ErrorPage
    private lateinit var errorDialogPage: ErrorDialogPage

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
        errorPage.assertPageHeader("Cannot access GP messaging")
                .assertHeaderText("This feature has been turned off by your GP Surgery.")
                .assertSubHeaderText("Contact your GP for more information or to access GP services.")
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
                errorDialogPage
                    .assertPageHeader("Cannot show message")
                    .assertPageTitle("Cannot show message")
                    .assertWarningParagraphText("There was an error opening your message.")
                    .assertWarningParagraphText("You can try opening your message again.")
                    .assertHasButton("Try again")
                    .assertWarningParagraphText(
                        "If the problem continues, contact the person or organisation who sent the message.")
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
