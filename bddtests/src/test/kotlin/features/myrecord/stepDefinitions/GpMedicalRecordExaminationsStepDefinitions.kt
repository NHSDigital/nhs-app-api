package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import pages.gpMedicalRecord.ExaminationsPage

open class GpMedicalRecordExaminationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var examinationsPage: ExaminationsPage

    private val expectedData = arrayOf(
            "06-Jul-2018",
            "O/E - blood pressure reading",
            "120 / 80 mmHg")

    @Then("^I see the expected examinations - GP Medical Record$")
    fun thenISeeExpectedExaminationsGpMedicalRecord() {
        examinationsPage.assertExaminationsHeadings()
        examinationsPage.assertExaminationsItems(expectedData)
    }
}