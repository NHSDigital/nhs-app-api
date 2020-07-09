package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import pages.gpMedicalRecord.MedicalRecordV2Page
import utils.SerenityHelpers

open class V2MedicalRecordDemographicsStepDefinitions {

    private lateinit var medicalRecordV2Page: MedicalRecordV2Page

    @Then("^I see the expected demographics information - Medical Record v2$")
    fun thenISeeExpectedDemographicsV2() {
        val patient = SerenityHelpers.getPatient()
        medicalRecordV2Page.assertDemographicsContent(patient)
    }
}