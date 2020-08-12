package features.patientPracticeMessaging.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import pages.patientPracticeMessaging.PatientPracticeMessagingDeletePage
import pages.patientPracticeMessaging.PatientPracticeMessagingDeleteSuccessPage

class PatientPracticeMessageDeleteStepDefinitions {
    private lateinit var patientPracticeMessagingDeletePage: PatientPracticeMessagingDeletePage
    private lateinit var patientPracticeMessagingDeleteSuccessPage: PatientPracticeMessagingDeleteSuccessPage

    @When("^I click delete conversation on the delete page to confirm my decision$")
    fun iClickDeleteConversationOnDeletePage() {
        patientPracticeMessagingDeletePage.clickDeleteConversation()
    }

    @When("^I click go back to patient practice messages$")
    fun iClickToGoBackToPatientPracticeMessages() {
        patientPracticeMessagingDeleteSuccessPage.clickBackToMessages()
    }

    @Then("^I see a page indicating my patient practice message has been deleted$")
    fun iSeeTheDeleteSuccessPage() {
        Thread.sleep(RACE_CONDITION_WAIT)
        patientPracticeMessagingDeleteSuccessPage.assertDisplayed()
    }

    @Then("^I am prompted to confirm my intention to delete the conversation$")
    fun iSeeTheDeleteInfoPage() {
        patientPracticeMessagingDeletePage.assertDisplayed()
    }
}
