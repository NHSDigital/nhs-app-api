package features.prescriptionsSubmission.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When

import pages.prescription.PrescriptionSuccessPage

open class PrescriptionSuccessStepDefinition {
    lateinit var prescriptionSuccessPage: PrescriptionSuccessPage

    @Then("^I see the Prescription Ordered success page$")
    fun iSeePrescpreptionSuccessPage() {
        prescriptionSuccessPage.checkPrescriptionSuccessMessage()
    }

    @When("^I click the Back to my prescriptions link$")
    fun iClickBackLinkToPrescriptionPage() {
        prescriptionSuccessPage.clickBackLink();
    }
}

