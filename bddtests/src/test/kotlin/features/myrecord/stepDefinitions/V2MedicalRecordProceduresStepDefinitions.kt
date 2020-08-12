package features.myrecord.stepDefinitions

import io.cucumber.java.en.Then
import pages.gpMedicalRecord.ProceduresPage

open class V2MedicalRecordProceduresStepDefinitions {

    private lateinit var proceduresPage: ProceduresPage

    private val expectedData = arrayOf(
            "06-Jul-2018", "Lumpectomy NEC",
            "23-Oct-2018", "Administration of first intranasal seasonal influenza vacc",
            "23-Oct-2018", "Tuberculosis (BCG) vaccination",
            "23-Oct-2018", "First diphtheria vaccination",
            "23-Oct-2018", "1st haemophilus B vaccination",
            "23-Oct-2018", "Second pertussis vaccination",
            "23-Oct-2018", "First cholera vaccination",
            "23-Oct-2018", "First diphtheria vaccination",
            "23-Oct-2018", "First tetanus vaccination",
            "23-Oct-2018", "First polio vaccination",
            "23-Oct-2018", "First anthrax vaccination",
            "23-Oct-2018", "First tetanus vaccination",
            "23-Oct-2018", "First pertussis vaccination",
            "23-Oct-2018", "First polio vaccination",
            "23-Oct-2018", "Second diphtheria vaccination",
            "23-Oct-2018", "Second tetanus vaccination",
            "23-Oct-2018", "Second polio vaccination",
            "23-Oct-2018", "2nd haemophilus B vaccination",
            "10-Oct-2018", "Mumps vaccination",
            "Not Known", "Second diphtheria vaccination",
            "Not Known", "Second polio vaccination",
            "Not Known", "Second tetanus vaccination")

    @Then("^I see the expected procedures - Medical Record v2$")
    fun thenISeeExpectedProceduresV2() {
        proceduresPage.assertProceduresHeadings()
        proceduresPage.assertProceduresItems(expectedData)
    }
}
