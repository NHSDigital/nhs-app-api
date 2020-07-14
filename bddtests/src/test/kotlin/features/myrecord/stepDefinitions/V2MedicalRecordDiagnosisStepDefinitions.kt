package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import pages.gpMedicalRecord.DiagnosisPage

open class V2MedicalRecordDiagnosisStepDefinitions {

    private lateinit var diagnosisPage: DiagnosisPage

    private val expectedData = arrayOf(
            "20-Feb-2015",
            "Severe hypertension (Nat Inst for Health Clinical Ex 2011)",
            "11-May-2014",
            "[D]Albuminuria",
            "04-Apr-2014",
            "Left ventricular cardiac dysfunction",
            "30-Jan-2011",
            "Left ventricular failure",
            "There are no Past Diagnosis records in the patient's record")

    @Then("^I see the expected diagnosis - Medical Record v2$")
    fun thenISeeExpectedDiagnosisV2() {
        diagnosisPage.assertDiagnosisHeadings()
        diagnosisPage.assertDiagnosisItems(expectedData)
    }
}
