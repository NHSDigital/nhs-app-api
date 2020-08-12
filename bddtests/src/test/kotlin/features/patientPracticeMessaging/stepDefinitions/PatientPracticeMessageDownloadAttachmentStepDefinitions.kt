package features.patientPracticeMessaging.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import pages.patientPracticeMessaging.PatientPracticeDownloadAttachmentPage

class PatientPracticeMessageDownloadAttachmentStepDefinitions {
    private lateinit var patientPracticeMessagingDownloadAttachmentPage: PatientPracticeDownloadAttachmentPage

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
}
