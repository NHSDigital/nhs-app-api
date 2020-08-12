package features.patientPracticeMessaging.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import mocking.patientPracticeMessaging.Recipient
import pages.patientPracticeMessaging.PracticePatientMessagingCreateMessagePage
import utils.SerenityHelpers
import utils.getOrNull
import worker.models.patientPracticeMessaging.CreateMessageRequest

class PatientPracticeMessageCreateStepDefinitions {

    private lateinit var patientPracticePatientMessagingCreateMessagePage: PracticePatientMessagingCreateMessagePage


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

    @Then("^I see validation errors for subject and message$")
    fun iSeeValidationErrorsForSubjectAndMessage(){
        patientPracticePatientMessagingCreateMessagePage.assertValidationErrorsDisplayed()
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
}
