package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import pages.gpMedicalRecord.ExaminationsPage

open class V2MedicalRecordExaminationsStepDefinitions {

    private lateinit var examinationsPage: ExaminationsPage

    private val expectedData = arrayOf(
            "06-Jul-2018",
            "O/E - blood pressure reading",
            "120 / 80 mmHg")

    @Then("^I see the expected examinations - Medical Record v2$")
    fun thenISeeExpectedExaminationsV2() {
        examinationsPage.assertExaminationsHeadings()
        examinationsPage.assertExaminationsItems(expectedData)
    }
}